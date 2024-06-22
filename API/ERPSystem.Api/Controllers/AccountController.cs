using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.API;
using ERPSystem.DataModel.Login;
using ERPSystem.DataModel.Response;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPSystem.Api.Controllers;

public class AccountController : ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly IConfiguration _configuration;
    private readonly IJwtHandler _jwtHandler;
    private readonly IAccountService _accountService;
    private readonly IUserService _userService;
    private static Regex DECODING_REGEX = new Regex(@"\\u(?<Value>[a-fA-F0-9]{4})", RegexOptions.Compiled);
    private const string PLACEHOLDER = @"#!쀍쀍쀍!#";

    public AccountController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
        IJwtHandler jwtHandler,
        IAccountService accountService, IUserService userService)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _configuration = configuration;
        _jwtHandler = jwtHandler;
        _accountService = accountService;
        _userService = userService;
    }

    /// <summary>
    /// Login by account and password
    /// </summary>
    /// <param name="model">JSON model for login(username, password)</param>
    /// <returns></returns>
    /// <response code="401">Unauthorized: username or password wrong</response>
    /// <response code="422">Unprocessable Entity: Model Body required username and password</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpPost]
    [AllowAnonymous]
    [Route(Constants.Route.ApiLogin)]
    public IActionResult Login([FromBody] LoginModel model, bool haveAddress)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        //Get the account in the system

        var account = !haveAddress ? _accountService.GetAuthenticatedAccount(model): _accountService.GetAuthenticatedAccountByAddress(model);
        if (account == null)
        {
            return new ApiUnauthorizedResult((int)LoginUnauthorized.InvalidCredentials,
                MessageResource.InvalidCredentials);
        }

        var authToken = _accountService.CreateAuthToken(account, updateRefreshToken: false);
        
        
        return Ok(authToken);
    }
    [HttpPost]
    [AllowAnonymous]
    [Route(Constants.Route.ApiLoginByAddress)]
    public IActionResult LoginByAddress([FromBody] LoginByAddressModel model)
    {
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }

        _userService.LoginByAddress(model.Address);
       
        var newModel = new LoginModel();
        newModel.Username = model.Address;
        newModel.Password = "123456789";
        //Get the account in the system
        var account = _accountService.GetAuthenticatedAccountByAddress(newModel);
        if (account == null)
        {
            return new ApiUnauthorizedResult((int)LoginUnauthorized.InvalidCredentials,
                MessageResource.InvalidCredentials);
        }
        var authToken = _accountService.CreateAuthToken(account, updateRefreshToken: false);

        if(account.RoleId == 2)
        {

        _userService.AddHistoryBalance(model.Address, model.Balance);
        }


        return Ok(authToken);
        

    }

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <param name="refreshToken">String of token refreshed</param>
    /// <param name="expiredToken">String timezone by expired of token</param>
    /// <returns></returns>
    /// <response code="401">Unauthorized: refreshToken or expiredToken not valid</response>
    /// <response code="429">Too Many Requests: Quota exceeded. Maximum allowed: 10 per 1s, 500 per 1m, 2000 per 1d.</response>
    [HttpPost]
    [Route(Constants.Route.ApiRefreshToken)]
    public IActionResult RefreshToken([FromBody] TokenRefreshModel model)
    {
        var responseStatus = new ResponseStatus();
        // If refreshToken or expiredToken is null return 
        if (model.RefreshToken == null || model.ExpiredToken == null)
        {
            return new ApiUnauthorizedResult((int)LoginUnauthorized.InvalidToken, MessageResource.InvalidToken);
        }

        // Check if refresh token is valid
        if (_jwtHandler.IsTokenExpired(model.RefreshToken))
        {
            return new ApiUnauthorizedResult((int)LoginUnauthorized.InvalidToken, MessageResource.ExpiredToken);
        }
        // Check if refresh token and token is a valid pair

        ClaimsPrincipal refreshTokenPrincipal = _jwtHandler.GetPrincipalFromExpiredToken(model.RefreshToken);
        ClaimsPrincipal expiredTokenPrincipal = _jwtHandler.GetPrincipalFromExpiredToken(model.ExpiredToken);
        if (refreshTokenPrincipal == null || expiredTokenPrincipal == null)
        {
            return new ApiUnauthorizedResult((int)LoginUnauthorized.InvalidToken, MessageResource.InvalidToken);
        }

        int expiredAccountId = expiredTokenPrincipal.GetAccountId();
        int accountId = refreshTokenPrincipal.GetAccountId();

        if (expiredAccountId != accountId)
        {
            // The expired token and the refresh token is not same pair
            return new ApiUnauthorizedResult((int)LoginUnauthorized.InvalidToken,
                MessageResource.ExpiredTokenAndRefreshTokenMismatched);
        }

        Account account = _accountService.GetById(accountId);
        if (account == null)
        {
            // The refresh token is wrong
            return new ApiUnauthorizedResult((int)LoginUnauthorized.InvalidToken, MessageResource.InvalidToken);
        }

        TokenModel authToken = _accountService.CreateAuthToken(account, updateRefreshToken: false);
        responseStatus.Message = Constants.Message.RefreshTokenSuccess;
        responseStatus.StatusCode = true;
        responseStatus.Data = authToken;

        return Ok(responseStatus);
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Route(Constants.Route.ApiChangePassword)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult ChangePasswordLogin([FromBody] ChangePasswordModel model)
    {
        int accountId = _httpContext.User.GetAccountId();
        Account account = _accountService.GetById(accountId);
        if (!ModelState.IsValid)
        {
            return new ValidationFailedResult(ModelState);
        }
        if (account == null)
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }
        if (!string.IsNullOrEmpty(model.UserName) && model.UserName != account.UserName)
        {
            return new ApiErrorResult(StatusCodes.Status403Forbidden, MessageResource.Forbidden);
        }

        if (string.IsNullOrEmpty(model.UserName))
        {
            model.UserName = account.UserName;
        }
        if (model.NewPassword.Trim().ToLower() != model.ConfirmNewPassword.Trim().ToLower())
        {
            return new ApiErrorResult(StatusCodes.Status400BadRequest, AccountResource.ConfirmPasswordNotMatch);
        }
        account.Password = SecurePasswordHasher.Hash(model.ConfirmNewPassword);
        account.UpdatedOn = DateTime.UtcNow;
        
        // var password = _accountService.GetDecryptPW(account);
        if (!SecurePasswordHasher.Verify(model.Password, account.Password))
        {
            return new ApiErrorResult(StatusCodes.Status422UnprocessableEntity, AccountResource.msgWrongPassword);
        }
        _accountService.ChangePassword(account);

        return new ApiSuccessResult(StatusCodes.Status200OK, AccountResource.msgChangePassSuccess);
    }


    /// <summary>
    /// Check string is contains unicode
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool IsUnicode(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }
        var asciiBytesCount = Encoding.ASCII.GetByteCount(input);
        var unicodeBytesCount = Encoding.UTF8.GetByteCount(input);
        return asciiBytesCount != unicodeBytesCount;
    }
    /// <summary>
    /// Unicode Decoding
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public string UnicodeToString(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        return DECODING_REGEX.Replace(
            str.Replace(@"\\", PLACEHOLDER),
            new MatchEvaluator(CapText)).Replace(PLACEHOLDER, @"\\");
    }
    static string CapText(Match m)
    { 
        // Get the matched string.
        return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
    }
    /// <summary>
    /// Get image static from link file local
    /// </summary>
    /// <param name="rootFolder"></param>
    /// <param name="companyCode"></param>
    /// <param name="date"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route(Constants.Route.ApiImageStatic)]
    [AllowAnonymous]
    public IActionResult GetFileFromPath(string rootFolder, string companyCode, string date, string fileName)
    {
        try
        {
            if (IsUnicode(fileName))
            {
                fileName = UnicodeToString(fileName);
            }

            string link = $"images/{rootFolder}/{fileName}";
            if (System.IO.File.Exists(link))
            {
                var bytes = System.IO.File.ReadAllBytes(link);
                string extension = "";
                if (fileName.Contains("."))
                {
                    extension = fileName.Split(".").Last();
                }

                switch (extension)
                {
                    case "jpg":
                    case "jpeg":
                    case "png":
                    {
                        return File(bytes, $"image/{extension}");
                    }
                    case "mp4":
                    case "avi":
                    case "webm":
                    {
                        return File(bytes, $"video/{extension}");
                    }
                    default:
                        return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
                }
            }
        }
        catch
        {
            return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
        }
        
        return new ApiErrorResult(StatusCodes.Status404NotFound, MessageResource.ResourceNotFound);
    }

}