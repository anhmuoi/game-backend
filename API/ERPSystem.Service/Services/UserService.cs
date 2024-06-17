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
using ERPSystem.DataModel.User;
using ERPSystem.Repository;
using ERPSystem.Service.Handler;
using ERPSystem.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using AutoMapper;
using System.Linq.Dynamic.Core;
using Newtonsoft.Json.Linq;
using MimeKit;

namespace ERPSystem.Service.Services;

public interface IUserService
{
    User? GetById(int userId);
    Account? GetAccountById(int accountId);
    Account? GetAccountByUserName(string userName);
    Account? GetAccountByEmail(string email);
    User? GetUserByUserName(string userName);
    User? GetUserByAccountId(int accountId);
    void UpdateProfile(AccountEditModel model);
    void UpdateAvatar(int userId, string path);
    Dictionary<string, object> GetInit();
    List<UserListModel> GetPaginated(string search, List<int> status, List<int> departmentIds, int pageNumber, int pageSize, string sortColumn, string sortDirection, out int totalRecords, out int recordsFiltered);
    int AddUser(UserAddModel model);
    bool UpdateUser(UserEditModel model);
    bool UserOutGroup(int id, int groupId);
    bool LoginByAddress(string address);
    bool DeleteMultiUsers(List<int> ids);
    bool IsExistedUserName(string name, int ignoredId);
    bool IsExistedEmail(string email, int ignoredId);
    bool IsExistedUserPhone(string phone, int ignoredId);
    void ChangePassword(Account account);
    void SendResetAccountMail(Account account, string email);
    void SendJoinGroupEmail(Account account, string email, User userRequest, Department department);
    void SendConfirmJoinGroupEmail(Account account, string email, Department department, bool confirm);
    List<string> CheckDepartmentManagerByUserId(List<int> userIds);
}

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IJwtHandler _jwtHandler;
    private readonly JwtOptionsModel _options;
    private readonly IMapper _mapper;
    private readonly IAccountService _accountService;
    private readonly IMailTemplateService _mailTemplateService;
    private readonly IMailService _mailService;


    public UserService(IUnitOfWork unitOfWork, IOptions<JwtOptionsModel> options, IJwtHandler jwtHandler, IMapper mapper, IAccountService accountService, IMailTemplateService mailTemplateService, IMailService mailService)
    {
        _unitOfWork = unitOfWork;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<UserService>();
        _jwtHandler = jwtHandler;
        _options = options?.Value;
        _mapper = mapper;
        _accountService = accountService;
        _mailTemplateService = mailTemplateService;
        _mailService = mailService;

    }

    public bool IsExistedUserName(string name, int ignoredId)
    {
            var user = _unitOfWork.UserRepository.GetByUserName(name);
            return user != null && user.Id != ignoredId;
    }
    public bool IsExistedEmail(string email, int ignoredId)
    {
        var account = _unitOfWork.AccountRepository.Gets().FirstOrDefault(m=> m.UserName == email);
        if (account != null)
        {

            var user = _unitOfWork.UserRepository.GetUserByAccountId(account.Id);
            return user != null && user.Id != ignoredId;
        }
        return false;
    }
    public bool IsExistedUserPhone(string phone, int ignoredId)
    {
            var user = _unitOfWork.UserRepository.GetByPhone(phone);
            return user != null && user.Id != ignoredId;
    }
    public User? GetById(int userId)
    {
        var user = _unitOfWork.UserRepository.Gets().Include(m => m.Account).FirstOrDefault(m => m.Id==userId);
        return user;
    }
    public User? GetUserByAccountId(int accountId)
    {
        var user = _unitOfWork.UserRepository.Gets().Include(m => m.Account).FirstOrDefault(m => m.Account.Id==accountId);
        return user;
    }
    public Account? GetAccountById(int accountId)
    {
        var account = _unitOfWork.AccountRepository.GetById(accountId);
        return account;
    }

    public User? GetUserByUserName(string userName)
    {
        return _unitOfWork.AppDbContext.User
            .FirstOrDefault(m => m.Name.ToLower() == userName.ToLower());
    }
    public Account? GetAccountByUserName(string userName)
    {
        return _unitOfWork.AppDbContext.Account
            .FirstOrDefault(m => m.UserName.ToLower() == userName.ToLower());
    }
    public Account? GetAccountByEmail(string email)
    {
        var user = _unitOfWork.UserRepository.Gets().FirstOrDefault(m => m.Email.ToLower() == email.ToLower());
        if(user == null)
        {
            return  null;
        }
        else 
        {

            return _unitOfWork.AppDbContext.Account
                .FirstOrDefault(m => user.AccountId == m.Id);
        }
    }
    
    public Dictionary<string, object> GetInit()
    {
        Dictionary<string, object> result = new Dictionary<string, object>();

        // account type
        var roleList = _unitOfWork.RoleRepository.Gets(x => !x.IsDeleted);
        List<EnumModelRole> type = new List<EnumModelRole>();
        foreach(var role in roleList)
        {
            type.Add(new EnumModelRole { Id = role.Id, Name = role.Name, IsDefault = role.IsDefault });
        }
        result.Add("roles", type);
        // department
        var departments = _unitOfWork.DepartmentRepository.Gets();
        List<EnumModel> departmentList = new List<EnumModel>();
        foreach(var depart in departments)
        {
            departmentList.Add(new EnumModel { Id = depart.Id, Name = depart.Name });
        }
        result.Add("departments", departments);
        // user status
        var status = EnumHelper.ToEnumList<Status>();
        result.Add("userStatus", status.OrderBy(m => m.Name));

        return result;
    }

    public List<UserListModel> GetPaginated(string search, List<int> status, List<int> departmentIds, int pageNumber, int pageSize, string sortColumn, string sortDirection, out int totalRecords, out int recordsFiltered)
    {
        var data = _unitOfWork.UserRepository.Gets();
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(search))
        {
            data = data.Where(m => m.Name.ToLower().Contains(search.ToLower()));
        }
        if (departmentIds is { Count: > 0 })
        {
            data = data.Where(m => departmentIds.Contains(m.DepartmentId.Value));
        }
        if (status is { Count: > 0 })
        {
            data = data.Where(m => status.Contains(m.Status));
        }
        

        recordsFiltered = data.Count();
        data = data.OrderBy($"{sortColumn} {sortDirection}");
        data = data.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        data = data.Include(m => m.Account);

        return data.AsEnumerable<User>().Select(_mapper.Map<UserListModel>).ToList();
    }

    public int AddUser(UserAddModel model)
    {
        int userId = 0;
        
        // Add user
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    User user = _mapper.Map<User>(model);
                    // if (string.IsNullOrEmpty(model.Email))
                    // {
                    //     user.Email = model.UserName;
                    // }
                    user.WalletAddress = model.UserName;
                    if(user.DepartmentId == 0)
                    {
                        user.DepartmentId = null;
                    }    
                    _unitOfWork.UserRepository.Add(user);
                    _unitOfWork.Save();
                    
                    // add account
                    var account = new Account();
                    account.UserName = model.UserName;
                    account.RoleId = model.RoleId;
                    account.Timezone = model.Timezone;
                    account.Language = model.Language;
                    account.Password = model.Password;
                    account.CreatedBy = model.CreatedBy;
                    account.CreatedOn = DateTime.UtcNow;
                    account.User = user;
                  
                    if(!string.IsNullOrEmpty(model.Password))
                    {
                        account.Password = SecurePasswordHasher.Hash(model.Password);
                    }
                    
                    _unitOfWork.AccountRepository.Add(account);
                    _unitOfWork.Save();
                    
                    user.AccountId = account.Id;
                    _unitOfWork.UserRepository.Update(user);
                    _unitOfWork.Save();
                    
                    userId = user.Id;
                    var token = _accountService.CreateAuthToken(account);
                    SendWelcomeMail(account.UserName, user.Name, token.AuthToken);
                    _unitOfWork.FolderRepository.CreateRootFolderForUser(userId);
                    _unitOfWork.Save();


                    
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                }
            }
        });
        
        
        return userId;
    }

    public bool UpdateUser(UserEditModel model)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    // edit user information
                    var user = _unitOfWork.UserRepository.GetById(model.Id);
                    if (user == null)
                        throw new Exception($"Can not get user by id = {model.Id}");

                    _mapper.Map(model, user);

                    if(user.DepartmentId == 0)
                    {
                        user.DepartmentId = null;
                    }    
                    _unitOfWork.UserRepository.Update(user);
                    _unitOfWork.Save();


                    // update account
                    if(user.AccountId != null)
                    {

                        var account = _unitOfWork.AccountRepository.GetById(user.AccountId.Value);
                        
                        account.UserName = model.UserName;
                        account.Password = account.Password;
                        account.RoleId = model.RoleId;
                        account.Timezone = model.Timezone;
                        account.Language = model.Language;
                        account.UpdatedBy = model.UpdatedBy;
                        account.UpdatedOn = DateTime.UtcNow;
                        account.User = user;

                        if(!string.IsNullOrEmpty(model.Password))
                        {
                            account.Password = SecurePasswordHasher.Hash(model.Password);
                        }

                        _unitOfWork.AccountRepository.Update(account);
                        _unitOfWork.Save();
                        
                        user.Account = account;
                        _unitOfWork.UserRepository.Update(user);
                        _unitOfWork.Save();
                    }

                    
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });
        
        return result;
    }
    public bool UserOutGroup(int id, int groupId)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    // edit user information
                    var user = _unitOfWork.UserRepository.GetById(id);
                    if (user == null)
                        throw new Exception($"Can not get user by id = {id}");


                    user.DepartmentId = null;
                      
                    _unitOfWork.UserRepository.Update(user);
                    _unitOfWork.Save();


                    
                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });
        
        return result;
    }

    public bool DeleteMultiUsers(List<int> ids)
    {
        bool result = false;
        _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
        {
            using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
            {
                try
                {
                    var users = _unitOfWork.UserRepository.Gets().Where(m => ids.Contains(m.Id)).ToList();
                    foreach (var user in users)
                    {
                        // delete root folder user
                        DeleteDocumentWhenDeleteUser(user.Id);

                        // delete user
                        _unitOfWork.UserRepository.Delete(m => m.Id == user.Id);
                        _unitOfWork.Save();

                        // delete account
                        _unitOfWork.AccountRepository.Delete(m => m.Id == user.AccountId);
                        _unitOfWork.Save();
                    }

                    transaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + ex.StackTrace);
                    transaction.Rollback();
                    result = false;
                }
            }
        });

        return result;
    }
    
    /// <summary>
    /// Update account profile
    /// Update : password, language, timezone
    /// </summary>
    /// <param name="model"></param>
    public void UpdateProfile(AccountEditModel model)
    {
        try
        {
            var account = _unitOfWork.AccountRepository.GetById(model.Id);

            
            account.Password = account.Password;
            account.Timezone = model.Timezone ?? account.Timezone;
            account.Language = model.Language ?? account.Language;
            account.UpdatedBy = model.UpdatedBy;
            account.UpdatedOn = DateTime.UtcNow;

            if(!string.IsNullOrEmpty(model.Password))
            {
                account.Password = SecurePasswordHasher.Hash(model.Password);
            }

            _unitOfWork.AccountRepository.Update(account);
            _unitOfWork.Save();

        }
        catch (Exception)
        {
            throw;
        }
    }
    public void UpdateAvatar(int userId, string path)
    {
        try
        {
            var user = _unitOfWork.UserRepository.GetById(userId);

            user.Avatar = path;
            
            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.Save();

        }
        catch (Exception)
        {
            throw;
        }
    }

    /// <summary>
    /// Change the password
    /// </summary>
    /// <param name="account"></param>
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

                    

                    _unitOfWork.Save();
                    transaction.Commit();

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        });
    }
    
    public void SendWelcomeMail(string email, string userName, string token)
    {
        
        if (email.IsEmailValid())
        {

            var supportMail = _mailService.GetSupportMailAddress();

            var subject = MailContentResource.SubjectCompanyAccount;

            var downloadLink_Andoid = Constants.Link.Android_Download;
            var downloadLink_IOS = Constants.Link.IOS_Download;

            var frontendURL = _mailService.GetFrontEndURL();
            if (frontendURL == null) throw new Exception(MessageResource.NullFrontEndURL);

            var resetLink = frontendURL + "/reset-password/" + token;
            var thread = new Thread(delegate ()
            {
                var pathToTemplateFile = _mailService.GetPathToTemplateFile("Welcome_Email.html");

                var pathToLogoImage = _mailService.GetPathToImageFile("logo.png");
                var pathToFireCracker = _mailService.GetPathToImageFile("fire_cracker_icon.png");
                var pathToAndroidImage = _mailService.GetPathToImageFile("get_it_on_google_play_icon.png");
                var pathToIOSImage = _mailService.GetPathToImageFile("download_on_app_store_icon.png");

                BodyBuilder builder = new BodyBuilder();

                try
                {
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToTemplateFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The process failed: {0}", e.ToString());
                }

                string userName = email;
                string password = String.Format(MailContentResource.BodyWelcomeManagerPassword, resetLink);
                string welcome = MailContentResource.BodyWelcome;
                string welcomeGreeting = String.Format(MailContentResource.BodyWelcomeAdminGreeting);
                string loginButton = MailContentResource.BodyWelcomeLogin;
                string downloadButton_Android = MailContentResource.ResourceManager.GetString("BodyWelcomeAndroidDownload");
                string downloadButton_IOS = MailContentResource.ResourceManager.GetString("BodyWelcomeIOSDownload");
                string customerSupport = String.Format(MailContentResource.BodyCustomerSupport,
                                                    MailContentResource.BodyWorkingTimeInfo, supportMail);
                string replyMessage = String.Format(MailContentResource.BodyReplyMessage, supportMail);

                string downloadExplaination = MailContentResource.ResourceManager.GetString("BodyDownloadExplain");

                string mailBody = string.Format(builder.HtmlBody,
                                                frontendURL,
                                                "Portal URL",
                                                userName,
                                                password,
                                                welcome,
                                                welcomeGreeting,
                                                loginButton,
                                                downloadExplaination,
                                                downloadLink_Andoid,
                                                downloadLink_IOS,
                                                "",
                                                "",
                                                "Download Link",
                                                downloadButton_Android,
                                                downloadButton_IOS,
                                                customerSupport,
                                                replyMessage,
                                                "");

                AlternateView atvHtml = AlternateView.CreateAlternateViewFromString(mailBody, null, MediaTypeNames.Text.Html);
                LinkedResource logoInline = new LinkedResource(new MemoryStream(System.IO.File.ReadAllBytes(pathToLogoImage)), MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "logoImageId",
                    TransferEncoding = TransferEncoding.Base64
                };
                atvHtml.LinkedResources.Add(logoInline);
                LinkedResource fireCrackerInline = new LinkedResource(new MemoryStream(System.IO.File.ReadAllBytes(pathToFireCracker)), MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "fireCrackerImageId",
                    TransferEncoding = TransferEncoding.Base64
                };
                atvHtml.LinkedResources.Add(fireCrackerInline);
                LinkedResource androidInline = new LinkedResource(new MemoryStream(System.IO.File.ReadAllBytes(pathToAndroidImage)), MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "getItOnGooglePlayImageId",
                    TransferEncoding = TransferEncoding.Base64
                };
                atvHtml.LinkedResources.Add(androidInline);
                LinkedResource iosInline = new LinkedResource(new MemoryStream(System.IO.File.ReadAllBytes(pathToIOSImage)), MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "downloadOnAppStoreImageId",
                    TransferEncoding = TransferEncoding.Base64
                };
                atvHtml.LinkedResources.Add(iosInline);

                MailMessage mailMessage = new MailMessage();
                mailMessage.AlternateViews.Add(atvHtml);

                mailMessage.Subject = subject;
                mailMessage.To.Add(email);

                _mailService.SendMail(mailMessage);

                //_mailService.SendMail(email, null, subject, mailBody);

            });
            thread.Start();

        }
    }
    public void SendJoinGroupEmail(Account account, string email, User userRequest, Department department)
    {
        
       if (email.IsEmailValid())
        {
            string language = account.Language;


            var culture = new CultureInfo(language);

            var supportMail = _mailService.GetSupportMailAddress();

            var subject = MailContentResource.ResourceManager.GetString("SubjectJoinGroup", culture);

            var frontendURL = _mailService.GetFrontEndURL();
            if (frontendURL == null) throw new Exception(MessageResource.NullFrontEndURL);

            var user = GetUserByAccountId(account.Id);
            var userName = user.Name;
            var token = _accountService.CreateAuthToken(account).AuthToken;

            var resetLink = frontendURL + "/join-group/" + userRequest.Id +"/" + department.Id +"/"+ token;

            var thread = new Thread(delegate ()
            {
                var pathToTemplateFile = _mailService.GetPathToTemplateFile("Join_Group_Email.html");

                var pathToLogoImage = _mailService.GetPathToImageFile("logo.png");
                //var b64String = _mailService.ConvertImageToBase64(pathToLogoImage);
                //var imageUrl = "data:image/png;base64," + b64String;

                BodyBuilder builder = new BodyBuilder();

                try
                {
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToTemplateFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The process failed: {0}", e.ToString());
                }

                string password = String.Format(MailContentResource.ResourceManager.GetString("BodyJoinGroup", culture), resetLink);
                var contents = string.Format(MailContentResource.ResourceManager.GetString("BodyJoinGroupInfo", culture), userName, userRequest.Name, password);
                string customerSupport = String.Format(MailContentResource.ResourceManager.GetString("BodyCustomerSupport", culture),
                                                    MailContentResource.ResourceManager.GetString("BodyWorkingTimeInfo", culture), supportMail);
                string replyMessage = String.Format(MailContentResource.ResourceManager.GetString("BodyReplyMessage", culture), supportMail);

                string mailBody = string.Format(builder.HtmlBody,
                                                contents,
                                                customerSupport,
                                                replyMessage,
                                                "");

                AlternateView atvHtml = AlternateView.CreateAlternateViewFromString(mailBody, null, MediaTypeNames.Text.Html);

                LinkedResource logoInline = new LinkedResource(new MemoryStream(System.IO.File.ReadAllBytes(pathToLogoImage)), MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "logoImageId",
                    TransferEncoding = TransferEncoding.Base64
                };
                atvHtml.LinkedResources.Add(logoInline);

                MailMessage mailMessage = new MailMessage();
                mailMessage.AlternateViews.Add(atvHtml);

                mailMessage.Subject = subject;
                mailMessage.To.Add(email);

                _mailService.SendMail(mailMessage);

                //_mailService.SendMail(email, null, subject, mailBody);

            });
            thread.Start();
        }
    }
    public void SendConfirmJoinGroupEmail(Account account, string email, Department department, bool confirm)
    {
        
       if (email.IsEmailValid())
        {
            string language = account.Language;


            var culture = new CultureInfo(language);

            var supportMail = _mailService.GetSupportMailAddress();

            var subject = MailContentResource.ResourceManager.GetString("SubjectConfirmJoinGroup", culture);

            var frontendURL = _mailService.GetFrontEndURL();
            if (frontendURL == null) throw new Exception(MessageResource.NullFrontEndURL);

            var user = GetUserByAccountId(account.Id);
            var userName = user.Name;
            var token = _accountService.CreateAuthToken(account).AuthToken;


            var thread = new Thread(delegate ()
            {
                var pathToTemplateFile = _mailService.GetPathToTemplateFile("Confirm_Join_Group_Email.html");

                var pathToLogoImage = _mailService.GetPathToImageFile("logo.png");
                //var b64String = _mailService.ConvertImageToBase64(pathToLogoImage);
                //var imageUrl = "data:image/png;base64," + b64String;

                BodyBuilder builder = new BodyBuilder();

                try
                {
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToTemplateFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The process failed: {0}", e.ToString());
                }

                
                var contents = confirm ?  string.Format(MailContentResource.ResourceManager.GetString("BodyConfirmJoinGroupInfo", culture), userName, department.Name) : string.Format(MailContentResource.ResourceManager.GetString("BodyRejectJoinGroupInfo", culture), userName, department.Name);
                string customerSupport = String.Format(MailContentResource.ResourceManager.GetString("BodyCustomerSupport", culture),
                                                    MailContentResource.ResourceManager.GetString("BodyWorkingTimeInfo", culture), supportMail);
                string replyMessage = String.Format(MailContentResource.ResourceManager.GetString("BodyReplyMessage", culture), supportMail);

                string mailBody = string.Format(builder.HtmlBody,
                                                contents,
                                                customerSupport,
                                                replyMessage,
                                                "");

                AlternateView atvHtml = AlternateView.CreateAlternateViewFromString(mailBody, null, MediaTypeNames.Text.Html);

                LinkedResource logoInline = new LinkedResource(new MemoryStream(System.IO.File.ReadAllBytes(pathToLogoImage)), MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "logoImageId",
                    TransferEncoding = TransferEncoding.Base64
                };
                atvHtml.LinkedResources.Add(logoInline);

                MailMessage mailMessage = new MailMessage();
                mailMessage.AlternateViews.Add(atvHtml);

                mailMessage.Subject = subject;
                mailMessage.To.Add(email);

                _mailService.SendMail(mailMessage);

                //_mailService.SendMail(email, null, subject, mailBody);

            });
            thread.Start();
        }
    }

    /// <summary>
    /// Send reset account email
    /// </summary>
  
    public void SendResetAccountMail(Account account, string email)
    {

        if (email.IsEmailValid())
        {
            string language = account.Language;


            var culture = new CultureInfo(language);

            var supportMail = _mailService.GetSupportMailAddress();

            var subject = MailContentResource.ResourceManager.GetString("SubjectResetAccount", culture);

            var frontendURL = _mailService.GetFrontEndURL();
            if (frontendURL == null) throw new Exception(MessageResource.NullFrontEndURL);

            var user = GetUserByAccountId(account.Id);
            var userName = user.Name;
            var token = _accountService.CreateAuthToken(account).AuthToken;

            var resetLink = frontendURL + "/reset-password/" + token;

            var thread = new Thread(delegate ()
            {
                var pathToTemplateFile = _mailService.GetPathToTemplateFile("Plain_Email.html");

                var pathToLogoImage = _mailService.GetPathToImageFile("logo.png");
                //var b64String = _mailService.ConvertImageToBase64(pathToLogoImage);
                //var imageUrl = "data:image/png;base64," + b64String;

                BodyBuilder builder = new BodyBuilder();

                try
                {
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToTemplateFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The process failed: {0}", e.ToString());
                }

                string password = String.Format(MailContentResource.ResourceManager.GetString("BodyResetPassword", culture), resetLink);
                var contents = string.Format(MailContentResource.ResourceManager.GetString("BodyResetAccount", culture), userName, password);
                string customerSupport = String.Format(MailContentResource.ResourceManager.GetString("BodyCustomerSupport", culture),
                                                    MailContentResource.ResourceManager.GetString("BodyWorkingTimeInfo", culture), supportMail);
                string replyMessage = String.Format(MailContentResource.ResourceManager.GetString("BodyReplyMessage", culture), supportMail);

                string mailBody = string.Format(builder.HtmlBody,
                                                contents,
                                                customerSupport,
                                                replyMessage,
                                                "");

                AlternateView atvHtml = AlternateView.CreateAlternateViewFromString(mailBody, null, MediaTypeNames.Text.Html);

                LinkedResource logoInline = new LinkedResource(new MemoryStream(System.IO.File.ReadAllBytes(pathToLogoImage)), MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "logoImageId",
                    TransferEncoding = TransferEncoding.Base64
                };
                atvHtml.LinkedResources.Add(logoInline);

                MailMessage mailMessage = new MailMessage();
                mailMessage.AlternateViews.Add(atvHtml);

                mailMessage.Subject = subject;
                mailMessage.To.Add(email);

                _mailService.SendMail(mailMessage);

                //_mailService.SendMail(email, null, subject, mailBody);

            });
            thread.Start();
        }

    }

    public List<string> CheckDepartmentManagerByUserId(List<int> userIds)
    {
        var departmentManagerIds = _unitOfWork.DepartmentRepository
            .Gets(m => m.DepartmentManagerId.HasValue)
            .Select(m => m.DepartmentManagerId.Value).AsEnumerable();
        
        return _unitOfWork.UserRepository
            .Gets(m => userIds.Contains(m.Id) && m.AccountId.HasValue && departmentManagerIds.Contains(m.AccountId.Value))
            .Include(m => m.Department)
            .AsEnumerable()
            .Select(m => $"{m.Name} - {m.Department?.Name}").ToList();
    }
    
    private void DeleteDocumentWhenDeleteUser(int userId)
    {
        var folderIds = _unitOfWork.UserFolderRepository.Gets(m => m.UserId == userId).Select(m => m.FolderId).AsEnumerable();
        var fileIds = _unitOfWork.UserFolderRepository.Gets(m => m.UserId == userId).Select(m => m.FolderId).AsEnumerable();
        
        _unitOfWork.FolderRepository.Delete(m => folderIds.Contains(m.Id));
        _unitOfWork.FileRepository.Delete(m => fileIds.Contains(m.Id));
        _unitOfWork.Save();

        FileHelpers.DeleteFolder($"{Constants.MediaConfig.BaseFolderData}/{userId}");
    }

    public bool LoginByAddress(string address)
    {
        var user = _unitOfWork.UserRepository.GetByUserName(address);
        if(user == null)
        {
            // create user 
            var newUser = new UserAddModel();
            newUser.Avatar = "";
            newUser.Email = address;
            newUser.UserName = address;
            newUser.Name = address;
            newUser.Password = "123456789";
            newUser.Phone = "";
            newUser.DepartmentId = 0;
            newUser.Position = "";
            newUser.Status = 0;
            newUser.RoleId = 1;
            newUser.Timezone = "Etc/UTC";
            newUser.Language = "en-US";
            newUser.CreatedBy = 1;

            AddUser(newUser);
            return false;
        } else
        {
            return true;
        }
    }
}