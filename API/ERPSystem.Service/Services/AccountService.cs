using System.Globalization;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Claims;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.Login;
using ERPSystem.Repository;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ERPSystem.Service.Services;

public interface IAccountService
{
    Account GetAuthenticatedAccount(LoginModel model);
    Account GetAuthenticatedAccountByAddress(LoginModel model);
    Account? GetById(int accountId);
    Account? GetAccountByUserName(string userName);
    void ChangePassword(Account account);
    string GetDecryptPW(Account account);
    TokenModel CreateAuthToken(Account account, bool updateRefreshToken = true);
    Dictionary<string, object> GetInit();
    void EncryptAccountPw(Account account);
    List<Account> GetAccountByRoleId(int roleId);
    List<Account> GetAccountByRoleIds(List<int> roleIds);
    Dictionary<string, string> GetUserRabbitCredential();
}

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IJwtHandler _jwtHandler;
    private readonly JwtOptionsModel _options;
    private readonly IRoleService _roleService;
    private readonly IConfiguration _configuration;

    public AccountService(IUnitOfWork unitOfWork, IOptions<JwtOptionsModel> options, IJwtHandler jwtHandler, IRoleService roleService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _roleService = roleService;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<AccountService>();
        _jwtHandler = jwtHandler;
        _options = options?.Value;
        _configuration = configuration;
    }

    public Account? GetById(int accountId)
    {
        var account = _unitOfWork.AccountRepository.GetById(accountId);
        return account;
    }

    public Account? GetAccountByUserName(string userName)
    {
        return _unitOfWork.AppDbContext.Account
            .FirstOrDefault(m => m.UserName.ToLower() == userName.ToLower());
    }

    public void ChangePassword(Account account)
    {
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    _unitOfWork.AccountRepository.Update(account);
                    _unitOfWork.Save();
                    transaction.Commit();
                    // EncryptAccountPw(account);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        });
    }

    public void EncryptAccountPw(Account account)
    {
        string encPassword = "";
        try
        {
            encPassword = _unitOfWork.AppDbContext.Account.Where(m => m.Id == account.Id)
                .Select(m => _unitOfWork.AppDbContext.Enc("normal", m.Password, "")).FirstOrDefault() ?? string.Empty;

            if (!string.IsNullOrEmpty(encPassword))
            {
                var updateQuery = DbHelper.MakeEncQuery("Account", "Password", account.Password, account.Id);
                var result = _unitOfWork.AppDbContext.Database.ExecuteSqlRaw(updateQuery);

                _unitOfWork.Save();
            }
        }
        catch (Exception e)
        {
            var updateQuery = DbHelper.MakeEncQuery("Account", "Password", account.Password, account.Id);

            // 2022-04-08
            Console.WriteLine($"[ERROR] Encrypt error");
            Console.WriteLine($"{e.Message}");
            Console.WriteLine($"{e.InnerException?.Message}");
            Console.WriteLine($"Encrypted password : {encPassword}");
            Console.WriteLine($"Update Query : {updateQuery}");
        }
    }

    public List<Account> GetAccountByRoleId(int roleId)
    {
        return _unitOfWork.AccountRepository.GetByRoleId(roleId).ToList();
    }
    public List<Account> GetAccountByRoleIds(List<int> roleIds)
    {
        return _unitOfWork.AccountRepository.GetByRoleIds(roleIds).ToList();
    }

    public TokenModel CreateAuthToken(Account account, bool updateRefreshToken = true)
    {
        Console.WriteLine("Create auth token for account {0}", account.UserName);
        var user = _unitOfWork.UserRepository.GetUserByAccountId(account.Id);
        var role = _unitOfWork.RoleRepository.GetById(account.RoleId);
        var claims = new[]
        {
            new Claim(Constants.ClaimName.UserName, account.UserName),
            new Claim(Constants.ClaimName.AccountId, account.Id.ToString()),
            new Claim(Constants.ClaimName.UserId, user?.Id.ToString() ?? "0"),
            new Claim(Constants.ClaimName.Role, role?.Type.ToString() ?? RoleType.DynamicRole.ToString()),
            new Claim(Constants.ClaimName.Timezone, account.Timezone),
            new Claim(Constants.ClaimName.Language, account.Language)
        };
        var token = _jwtHandler.BuildToken(claims);

        var refreshTokenClaims = new[]
        {
            new Claim(Constants.ClaimName.UserName, account.UserName),
            new Claim(Constants.ClaimName.AccountId, account.Id.ToString()),
        };

        string refreshToken = GetRefreshTokenByUserName(account.UserName);

        // Check if the  refresh token is valid, if not create new one
        bool invalidRefrestoken = false;
        if (!string.IsNullOrEmpty(refreshToken))
        {
            try
            {
                ClaimsPrincipal refreshTokenPrincipal = _jwtHandler.GetPrincipalFromExpiredToken(refreshToken);
                var accountId = refreshTokenPrincipal.GetAccountId();
                if (accountId != account.Id)
                {
                    invalidRefrestoken = true;
                }
            }
            catch (Exception)
            {
                invalidRefrestoken = true;
            }
        }

        if (updateRefreshToken || _jwtHandler.IsTokenExpired(refreshToken) || invalidRefrestoken || string.IsNullOrEmpty(refreshToken))
        {
            refreshToken = _jwtHandler.BuildRefreshToken(refreshTokenClaims);
            AddTokenAndRefreshToken(refreshToken, account);
        }

        //var deptId = user?.DepartmentId ?? 0;
        //var deptName = deptId == 0 ? "" : _departmentService.GetById(deptId).DepartName;
        var deptName = user != null ? (user.Department != null ? user.Department.Name : "") : "";
        var fullName = user == null ? "" : user.Name;

        TokenModel result = new TokenModel
        {
            Status = 1,
            AuthToken = token,
            RefreshToken = refreshToken,
            FullName = fullName,
            DepartmentName = deptName,
            Permissions = role != null ? _roleService.GetPermissionsByRoleId(role.Id) : new Dictionary<string, Dictionary<string, bool>>(),
            QueueService = GetUserRabbitCredential(),
            Role = role?.Name ?? "",
            AccountId = account.Id,
            UserId = user?.Id ?? 0,
            ExpireAccessToken = _options.ExpiryMinutes,
            UserTimeZone = account.Timezone,
            UserLanguage = account.Language,
        };
        return result;
    }

    private string GetRefreshTokenByUserName(string userName)
    {
        return _unitOfWork.AccountRepository.GetRefreshTokenByUserName(userName);
    }

    public Account GetAuthenticatedAccount(LoginModel model)
    {
        var account = _unitOfWork.AppDbContext.Account
            .Include(m => m.User)
            .FirstOrDefault(m => m.UserName.ToLower() == model.Username.ToLower());

        if (account != null)
        {
            // Check password
            if (SecurePasswordHasher.Verify(model.Password, account.Password))
            {
                return account;
            }
        }

        return null;
    }
    public Account GetAuthenticatedAccountByAddress(LoginModel model)
    {
        var account = _unitOfWork.AppDbContext.Account
            .Include(m => m.User)
            .FirstOrDefault(m => m.UserName.ToLower() == model.Username.ToLower());

        if (account != null)
        {
           
            return account;
        }

        return null;
    }

    private void AddTokenAndRefreshToken(string refreshToken, Account model)
    {
        try
        {
            _unitOfWork.AccountRepository.AddRefreshToken(refreshToken, model);
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
    }

    public string GetDecryptPW(Account account)
    {
        var password = account.Password;

        try
        {
            var decPassword = _unitOfWork.AppDbContext.Account
                .Where(m => m.UserName.ToLower().Equals(account.UserName.ToLower()))
                .Select(m => _unitOfWork.AppDbContext.Dec("normal", m.Password, "", 0)).FirstOrDefault();

            if (!string.IsNullOrEmpty(decPassword)) password = decPassword;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }

        return password;
    }

    public Dictionary<string, object> GetInit()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();

        // account type
        var roleList = _unitOfWork.RoleRepository.Gets();
        List<EnumModel> type = new List<EnumModel>();
        foreach(var role in roleList)
        {
            type.Add(new EnumModel { Id = role.Id, Name = role.Name });
        }
        result.Add("roles", type);

        return result;
    }

    public Dictionary<string, string> GetUserRabbitCredential()
        {
            var host = _configuration.GetSection("QueueConnectionSettingsGame:Host");
            var virtualHost = _configuration.GetSection("QueueConnectionSettingsGame:VirtualHost");
            var port = _configuration.GetSection("QueueConnectionSettingsGame:Port");
            var username = _configuration.GetSection("QueueConnectionSettingsGame:UserName");
            var password = _configuration.GetSection("QueueConnectionSettingsGame:Password");

            Dictionary<string, string> queueService = new Dictionary<string, string>();
            queueService.Add(host.Key, host.Value);
            queueService.Add(port.Key, port.Value);
            queueService.Add(virtualHost.Key, virtualHost.Value);
            queueService.Add(username.Key, username.Value);
            queueService.Add(password.Key, password.Value);
         
            return queueService;
        }
}