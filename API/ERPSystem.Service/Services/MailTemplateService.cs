using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using ERPSystem.Common.Infrastructure;
using ERPSystem.Common.Resources;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Email;
using ERPSystem.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime.Calendars;

namespace ERPSystem.Service
{
    public interface IMailTemplateService
    {
        MailTemplateModel GetMailTemplateByType(int type);
        MailTemplateModel GetMailTemplateDefault(int type);
        bool EditMailTemplateByType(MailTemplateModel model);
        MailTemplateModel CreateMailTemplateByType(int type);
        string UpdateValueInMailBody(string htmlBody);
        List<MailTemplate> GetAllMailTemplates();
        MailTemplateTypeModel CreateTypeModelByMailTemplate(MailTemplate mailTemplate, Account account, CultureInfo culture);
        string GetLanguage(int accountId);
        string UpdateVariablesToURL(MailTemplate mailTemplate, Dictionary<string, string> variables);
        bool ChangeLinkImageMailTemplate(int companyId);
    }

    public class MailTemplateService : IMailTemplateService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpContext _httpContext;
        private readonly IMailService _mailService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="settingService"></param>
        /// <param name="mailService"></param>
        public MailTemplateService(IConfiguration configuration, IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
           IMailService mailService)
        {
            _configuration = configuration;
            _httpContext = httpContextAccessor.HttpContext;
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }


        /// <summary>
        /// get default string of body object EmailTemplate store in wwwroot
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string GetDefaultBodyEmailTemplate(string fileName, List<string> variables)
        {
            var pathToTemplateFile = _mailService.GetPathToTemplateFile(fileName);
            BodyBuilder builder = new BodyBuilder();

            try
            {
                using (StreamReader sourceReader = System.IO.File.OpenText(pathToTemplateFile))
                {
                    builder.HtmlBody = sourceReader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                return null;
            }

            string htmlMail = builder.HtmlBody;
            for (int i = 0; i < variables.Count; i++)
            {
                htmlMail = htmlMail.Replace("{" + i + "}", variables[i]);
            }

            // Variable LogoImage
            htmlMail = htmlMail.Replace("cid:logoImageId", "{{" + Constants.VariableEmailTemplate.LogoImageId + "}}");
            htmlMail = htmlMail.Replace("cid:qrImageId", "{{" + Constants.VariableEmailTemplate.ImageQRCode + "}}");
            htmlMail = htmlMail.Replace("cid:androidImageId",
                "{{" + Constants.VariableEmailTemplate.AndroidIcon + "}}");
            htmlMail = htmlMail.Replace("cid:iosImageId", "{{" + Constants.VariableEmailTemplate.IosIcon + "}}");
            htmlMail = htmlMail.Replace("cid:fireCrackerImageId",
                "{{" + Constants.VariableEmailTemplate.FireCrackerIcon + "}}");
            htmlMail = htmlMail.Replace("cid:getItOnGooglePlayImageId",
                "{{" + Constants.VariableEmailTemplate.GetItOnGooglePlayIcon + "}}");
            htmlMail = htmlMail.Replace("cid:downloadOnAppStoreImageId",
                "{{" + Constants.VariableEmailTemplate.DownloadOnAppStoreIcon + "}}");
            htmlMail = htmlMail.Replace("cid:companyLogoImageId",
                "{{" + Constants.VariableEmailTemplate.CompanyLogo + "}}");
            htmlMail = htmlMail.Replace("cid:timeIconImageId", "{{" + Constants.VariableEmailTemplate.TimeIcon + "}}");
            htmlMail = htmlMail.Replace("cid:locationIconImageId",
                "{{" + Constants.VariableEmailTemplate.LocationIcon + "}}");
            htmlMail = htmlMail.Replace("cid:doorIconImageId", "{{" + Constants.VariableEmailTemplate.DoorIcon + "}}");
            htmlMail = htmlMail.Replace("cid:phoneIconImageId",
                "{{" + Constants.VariableEmailTemplate.PhoneIcon + "}}");

            return htmlMail;
        }



        /// <summary>
        /// map data to variable {{var}}
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="visit"></param>
        /// <param name="attendanceLeave"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public string MapValueMailTemplateForVariable(int variable)
        {
            switch (variable)
            {
              
               
              
                case (int)Constants.VariableEmailTemplate.LogoImageId:
                {
                    return "cid:logoImageId";
                }
            
                case (int)Constants.VariableEmailTemplate.AndroidIcon:
                {
                    return "cid:androidImageId";
                }
                case (int)Constants.VariableEmailTemplate.IosIcon:
                {
                    return "cid:iosImageId";
                }
                case (int)Constants.VariableEmailTemplate.FireCrackerIcon:
                {
                    return "cid:fireCrackerImageId";
                }
                case (int)Constants.VariableEmailTemplate.GetItOnGooglePlayIcon:
                {
                    return "cid:getItOnGooglePlayImageId";
                }
                case (int)Constants.VariableEmailTemplate.DownloadOnAppStoreIcon:
                {
                    return "cid:downloadOnAppStoreImageId";
                }
                case (int)Constants.VariableEmailTemplate.CompanyLogo:
                {
                    return "cid:companyLogoImageId";
                }
                case (int)Constants.VariableEmailTemplate.TimeIcon:
                {
                    return "cid:timeIconImageId";
                }
                case (int)Constants.VariableEmailTemplate.LocationIcon:
                {
                    return "cid:locationIconImageId";
                }
                case (int)Constants.VariableEmailTemplate.DoorIcon:
                {
                    return "cid:doorIconImageId";
                }
                case (int)Constants.VariableEmailTemplate.PhoneIcon:
                {
                    return "cid:phoneIconImageId";
                }
                default:
                {
                    return "{{" + Enum.GetName(typeof(Constants.VariableEmailTemplate), variable) + "}}";
                }
            }
        }

        /// <summary>
        /// update list variables {{}} in mail body
        /// </summary>
        /// <param name="htmlBody"></param>
        /// <param name="visit"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public string UpdateValueInMailBody(string htmlBody)
        {
            string pattern = "{{(.*?)}}";
            var variables = Regex.Matches(htmlBody, pattern, RegexOptions.Singleline);
            foreach (var variable in variables)
            {
                string name = variable.ToString().Substring(2, variable.ToString().Length - 4);
                try
                {
                    int indexVariable = (int)Enum.Parse(typeof(Constants.VariableEmailTemplate), name.Trim());
                    htmlBody = htmlBody.Replace(variable.ToString(), MapValueMailTemplateForVariable(indexVariable));
                }
                catch (Exception ex)
                {
                    _ = ex.StackTrace;
                    continue;
                }
            }

            return htmlBody;
        }

       

        public string InsertValueDefaultForVariables(List<int> variables)
        {
            Dictionary<string, string> dictionaryVariables = new Dictionary<string, string>();
            foreach (var variable in variables)
            {
                switch (variable)
                {
                    case (int) Constants.VariableEmailTemplate.LogoImageId:
                    {
                        var pathToLogoImage = _mailService.GetPathToImageFile("logo.png");
                        var b64String = _mailService.ConvertImageToBase64(pathToLogoImage);
                        var logoImageUrl = Constants.Image.DefaultHeaderPng + b64String;

                        // // save image logo
                        // if (logoImageUrl.IsTextBase64())
                        // {
                        //     string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        //     string path = $"{Constants.Settings.DefineFolderImages}/setting/{Constants.Settings.Logo}.jpg";
                        //     bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        //     logoImageUrl = $"{hostApi}/static/{path}";
                        // }
                        dictionaryVariables.Add(Constants.VariableEmailTemplate.LogoImageId.ToString(), logoImageUrl);
                        break;
                    }
                    case (int) Constants.VariableEmailTemplate.EmailWelcome:
                    {
                        dictionaryVariables.Add(Constants.VariableEmailTemplate.EmailWelcome.ToString(),"");
                        break;
                    }
                    case (int) Constants.VariableEmailTemplate.UserName:
                    {
                        dictionaryVariables.Add(Constants.VariableEmailTemplate.UserName.ToString(), "User Name Test");
                        break;
                    }
                    case (int) Constants.VariableEmailTemplate.UserNameWelcome:
                    {
                        dictionaryVariables.Add(Constants.VariableEmailTemplate.UserNameWelcome.ToString(), "User Name Test");
                        break;
                    }
                 
                    case (int) Constants.VariableEmailTemplate.ContactVisitTarget:
                    {
                        dictionaryVariables.Add(Constants.VariableEmailTemplate.ContactVisitTarget.ToString(), "Visit Target, 0123456789, demasterpro@duali.com");
                        break;
                    }
                   
                    case (int) Constants.VariableEmailTemplate.AndroidIcon:
                    {
                        var pathToAndroidImage = _mailService.GetPathToImageFile("android_icon.png");
                        var b64String = _mailService.ConvertImageToBase64(pathToAndroidImage);
                        var logoImageUrl = Constants.Image.DefaultHeaderPng + b64String;

                        // if (logoImageUrl.IsTextBase64())
                        // {
                        //     string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        //     string path = $"{Constants.Settings.DefineFolderImages}/setting/{Guid.NewGuid().ToString()}.jpg";
                        //     bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        //     logoImageUrl = $"{hostApi}/static/{path}";
                        // }

                        dictionaryVariables.Add(Constants.VariableEmailTemplate.AndroidIcon.ToString(), logoImageUrl);
                        break;
                    }
                    case (int) Constants.VariableEmailTemplate.IosIcon:
                    {
                        var pathToAndroidImage = _mailService.GetPathToImageFile("ios_icon.png");
                        var b64String = _mailService.ConvertImageToBase64(pathToAndroidImage);
                        var logoImageUrl = Constants.Image.DefaultHeaderPng + b64String;

                        // if (logoImageUrl.IsTextBase64())
                        // {
                        //     string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        //     string path = $"{Constants.Settings.DefineFolderImages}/setting/{Guid.NewGuid().ToString()}.jpg";
                        //     bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        //     logoImageUrl = $"{hostApi}/static/{path}";
                        // }

                        dictionaryVariables.Add(Constants.VariableEmailTemplate.IosIcon.ToString(), logoImageUrl);
                        break;
                    }
                    case (int) Constants.VariableEmailTemplate.FireCrackerIcon:
                    {
                        var pathToAndroidImage = _mailService.GetPathToImageFile("fire_cracker_icon.png");
                        var b64String = _mailService.ConvertImageToBase64(pathToAndroidImage);
                        var logoImageUrl = Constants.Image.DefaultHeaderPng + b64String;

                        // if (logoImageUrl.IsTextBase64())
                        // {
                        //     string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        //     string path = $"{Constants.Settings.DefineFolderImages}/setting/{Guid.NewGuid().ToString()}.jpg";
                        //     bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        //     logoImageUrl = $"{hostApi}/static/{path}";
                        // }

                        dictionaryVariables.Add(Constants.VariableEmailTemplate.FireCrackerIcon.ToString(), logoImageUrl);
                        break;
                    }
                    case (int) Constants.VariableEmailTemplate.GetItOnGooglePlayIcon:
                    {
                        var pathToAndroidImage = _mailService.GetPathToImageFile("get_it_on_google_play_icon.png");
                        var b64String = _mailService.ConvertImageToBase64(pathToAndroidImage);
                        var logoImageUrl = Constants.Image.DefaultHeaderPng + b64String;

                        // if (logoImageUrl.IsTextBase64())
                        // {
                        //     string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        //     string path = $"{Constants.Settings.DefineFolderImages}/setting/{Guid.NewGuid().ToString()}.jpg";
                        //     bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        //     logoImageUrl = $"{hostApi}/static/{path}";
                        // }

                        dictionaryVariables.Add(Constants.VariableEmailTemplate.GetItOnGooglePlayIcon.ToString(), logoImageUrl);
                        break;
                    }
                    case (int) Constants.VariableEmailTemplate.DownloadOnAppStoreIcon:
                    {
                        var pathToAndroidImage = _mailService.GetPathToImageFile("download_on_app_store_icon.png");
                        var b64String = _mailService.ConvertImageToBase64(pathToAndroidImage);
                        var logoImageUrl = Constants.Image.DefaultHeaderPng + b64String;

                        // if (logoImageUrl.IsTextBase64())
                        // {
                        //     string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        //     string path = $"{Constants.Settings.DefineFolderImages}/setting/{Guid.NewGuid().ToString()}.jpg";
                        //     bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        //     logoImageUrl = $"{hostApi}/static/{path}";
                        // }

                        dictionaryVariables.Add(Constants.VariableEmailTemplate.DownloadOnAppStoreIcon.ToString(), logoImageUrl);
                        break;
                    }
                  
                    case (int) Constants.VariableEmailTemplate.CompanyLogo:
                    {
                        string logoImageUrl = "";
                        try
                        {
                            var pathToLogoImage = _mailService.GetPathToImageFile("logo.png");
                            var b64String = _mailService.ConvertImageToBase64(pathToLogoImage);
                            logoImageUrl = Constants.Image.DefaultHeaderPng + b64String;
                        }
                        catch (Exception e)
                        {
                        }

                        // if (logoImageUrl.IsTextBase64())
                        // {
                        //     string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        //     string path = $"{Constants.Settings.DefineFolderImages}/setting/{Guid.NewGuid().ToString()}.jpg";
                        //     bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        //     logoImageUrl = $"{hostApi}/static/{path}";
                        // }

                        dictionaryVariables.Add(Constants.VariableEmailTemplate.CompanyLogo.ToString(), logoImageUrl);
                        break;
                    }
                    case (int) Constants.VariableEmailTemplate.TimeIcon:
                    {
                        var pathToAndroidImage = _mailService.GetPathToImageFile("time_icon.png");
                        var b64String = _mailService.ConvertImageToBase64(pathToAndroidImage);
                        var logoImageUrl = Constants.Image.DefaultHeaderPng + b64String;

                        // if (logoImageUrl.IsTextBase64())
                        // {
                        //     string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        //     string path = $"{Constants.Settings.DefineFolderImages}/setting/{Guid.NewGuid().ToString()}.jpg";
                        //     bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        //     logoImageUrl = $"{hostApi}/static/{path}";
                        // }

                        dictionaryVariables.Add(Constants.VariableEmailTemplate.TimeIcon.ToString(), logoImageUrl);
                        break;
                    }
                    case (int) Constants.VariableEmailTemplate.LocationIcon:
                    {
                        var pathToAndroidImage = _mailService.GetPathToImageFile("location_icon.png");
                        var b64String = _mailService.ConvertImageToBase64(pathToAndroidImage);
                        var logoImageUrl = Constants.Image.DefaultHeaderPng + b64String;

                        // if (logoImageUrl.IsTextBase64())
                        // {
                        //     string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        //     string path = $"{Constants.Settings.DefineFolderImages}/setting/{Guid.NewGuid().ToString()}.jpg";
                        //     bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        //     logoImageUrl = $"{hostApi}/static/{path}";
                        // }

                        dictionaryVariables.Add(Constants.VariableEmailTemplate.LocationIcon.ToString(), logoImageUrl);
                        break;
                    }
                    case (int) Constants.VariableEmailTemplate.DoorIcon:
                    {
                        var pathToAndroidImage = _mailService.GetPathToImageFile("door_icon.png");
                        var b64String = _mailService.ConvertImageToBase64(pathToAndroidImage);
                        var logoImageUrl = Constants.Image.DefaultHeaderPng + b64String;

                        // if (logoImageUrl.IsTextBase64())
                        // {
                        //     string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        //     string path = $"{Constants.Settings.DefineFolderImages}/setting/{Guid.NewGuid().ToString()}.jpg";
                        //     bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        //     logoImageUrl = $"{hostApi}/static/{path}";
                        // }

                        dictionaryVariables.Add(Constants.VariableEmailTemplate.DoorIcon.ToString(), logoImageUrl);
                        break;
                    }
                    case (int) Constants.VariableEmailTemplate.PhoneIcon:
                    {
                        var pathToAndroidImage = _mailService.GetPathToImageFile("phone_icon.png");
                        var b64String = _mailService.ConvertImageToBase64(pathToAndroidImage);
                        var logoImageUrl = Constants.Image.DefaultHeaderPng + b64String;

                        // if (logoImageUrl.IsTextBase64())
                        // {
                        //     string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        //     string path = $"{Constants.Settings.DefineFolderImages}/setting/{Guid.NewGuid().ToString()}.jpg";
                        //     bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        //     logoImageUrl = $"{hostApi}/static/{path}";
                        // }

                        dictionaryVariables.Add(Constants.VariableEmailTemplate.PhoneIcon.ToString(), logoImageUrl);
                        break;
                    }
                    default:
                    {
                        dictionaryVariables.Add(Enum.GetName(typeof(Constants.VariableEmailTemplate), variable), Enum.GetName(typeof(Constants.VariableEmailTemplate), variable));
                        break;
                    }
                }
            }

            return JsonConvert.SerializeObject(dictionaryVariables);
        }

        /// <summary>
        /// Get list of mail template in company
        /// </summary>
        /// <returns></returns>
        public List<MailTemplate> GetAllMailTemplates()
        {
            return _unitOfWork.MailTemplateRepository.GetAll().ToList();
        }

        /// <summary>
        /// Create mail type model with mail template
        /// </summary>
        /// <param name="mailTemplate"></param>
        /// <param name="account"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public MailTemplateTypeModel CreateTypeModelByMailTemplate(MailTemplate mailTemplate, Account account, CultureInfo culture)
        {
            var model = new MailTemplateTypeModel()
            {
                Type = mailTemplate.Type,
                Name = Enum.GetName(typeof(Constants.TypeEmailTemplate), mailTemplate.Type),
                Title = MailTemplateResource.ResourceManager.GetString("title" + Enum.GetName(typeof(Constants.TypeEmailTemplate), mailTemplate.Type), culture),
                Description = MailTemplateResource.ResourceManager.GetString("description" + Enum.GetName(typeof(Constants.TypeEmailTemplate), mailTemplate.Type), culture),
                UpdatedOn = mailTemplate.UpdatedOn
            };

            // account update mail template
            if (account != null)
            {
                var user = _unitOfWork.UserRepository.GetUserByAccountId(account.Id);
                if (user == null)
                    model.UpdatedBy = account.UserName;
                else
                    model.UpdatedBy = user.Name;
            }

            return model;
        }

        public string PreviewHtmlBodyMail(string body, Dictionary<string, string> variables)
        {
            string result = body;
            foreach (var variable in variables)
            {
                result = result.Replace("{{" + variable.Key + "}}", variable.Value);
            }

            return result;
        }

        /// <summary>
        /// Get a mail template by type of company
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MailTemplateModel GetMailTemplateByType(int type)
        {
            var mailTemplate = _unitOfWork.MailTemplateRepository.GetMailTemplateByType(type);
            if (mailTemplate == null) return null;
           

            var model = new MailTemplateModel()
            {
                Id = mailTemplate.Id,
                Type = mailTemplate.Type,
                Subject = mailTemplate.Subject,
                Body = mailTemplate.Body,
                IsEnable = mailTemplate.IsEnable,
                Variables = mailTemplate.Variables,
                CreatedBy = mailTemplate.CreatedBy,
                CreatedOn = mailTemplate.CreatedOn,
                UpdatedBy = mailTemplate.UpdatedBy,
                UpdatedOn = mailTemplate.UpdatedOn
            };

            // description variables
            if (!string.IsNullOrEmpty(mailTemplate.Variables))
            {
                var companyLanguage = GetLanguage(_httpContext.User.GetAccountId());
                var culture = new CultureInfo(companyLanguage);
                JObject variables = JObject.Parse(mailTemplate.Variables);
                var descriptionVariable = new Dictionary<string, string>();
                foreach (var property in variables.Properties())
                {
                    descriptionVariable.Add(property.Name, MailTemplateResource.ResourceManager.GetString("description" + property.Name, culture));
                }
                model.DetailVariables = JsonConvert.SerializeObject(descriptionVariable);
            }
            return model;
        }

        public bool EditMailTemplateByType(MailTemplateModel model)
        {
            bool isSuccess = false;
            _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
            {
                using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        MailTemplate mailTemplate = _unitOfWork.AppDbContext.MailTemplate.FirstOrDefault(m => m.Type == model.Type);
                        if (mailTemplate == null)
                        {
                            throw new Exception("Not found");
                        }

                        if (!string.IsNullOrEmpty(model.Body)) mailTemplate.Body = model.Body;
                        if (!string.IsNullOrEmpty(model.Subject)) mailTemplate.Subject = model.Subject;
                        if (!string.IsNullOrEmpty(model.Variables)) mailTemplate.Variables = UpdateVariablesToURL(mailTemplate, JsonConvert.DeserializeObject<Dictionary<string, string>>(model.Variables));
                        mailTemplate.IsEnable = model.IsEnable;
                        mailTemplate.UpdatedBy = _httpContext.User.GetAccountId();
                        mailTemplate.UpdatedOn = DateTime.Now;
                    
                        _unitOfWork.MailTemplateRepository.Update(mailTemplate);
                        _unitOfWork.Save();
                        transaction.Commit();
                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        _ = ex.StackTrace;

                        transaction.Rollback();
                        isSuccess = false;
                    }
                }
            });
            return isSuccess;
        }

        public MailTemplateModel CreateMailTemplateByType(int type)
        {
            var mailTemplate = _unitOfWork.MailTemplateRepository.GetMailTemplateByType(type);
            
            if (mailTemplate == null)
            {
                var model = GetMailTemplateDefault(type);
                if (model == null) return null;

                int accountId = _httpContext.User.GetAccountId();

                model.CreatedBy = accountId;
                model.CreatedOn = DateTime.UtcNow;
                model.UpdatedBy = accountId;
                model.UpdatedOn = DateTime.UtcNow;
                // model.HtmlPreview = PreviewHtmlBodyMail(model.Body, JsonConvert.DeserializeObject<Dictionary<string,string>>(model.Variables));

                _unitOfWork.MailTemplateRepository.Add(new MailTemplate()
                {
                    Type = type,
                    Subject = model.Subject,
                    Body = model.Body,
                    Variables = model.Variables,
                    CreatedBy = accountId,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedBy = accountId,
                    UpdatedOn = DateTime.UtcNow,
                });
                _unitOfWork.Save();

                return model;
            }
            else
            {
                var model = new MailTemplateModel()
                {
                    Id = mailTemplate.Id,
                    Type = mailTemplate.Type,
                    Subject = mailTemplate.Subject,
                    Body = mailTemplate.Body,
                    Variables = mailTemplate.Variables,
                    CreatedBy = mailTemplate.CreatedBy,
                    CreatedOn = mailTemplate.CreatedOn,
                    UpdatedBy = mailTemplate.UpdatedBy,
                    UpdatedOn = mailTemplate.UpdatedOn
                };
                // model.HtmlPreview = PreviewHtmlBodyMail(model.Body, JsonConvert.DeserializeObject<Dictionary<string,string>>(model.Variables));
                return model;
            }
        }

        public MailTemplateModel GetMailTemplateDefault(int type)
        {
            string body = "";
            string subject = "";
            string variables = "";
            switch (type)
            {
                
                case (int) Constants.TypeEmailTemplate.WelcomeUserEmail:
                {
                    GetMailTemplate_WelcomeUserEmail(out subject, out variables, out body);
                    break;
                }
                default: return null;
            }

            var model = new MailTemplateModel()
            {
                Type = type,
                Subject = subject,
                Body = body,
                Variables = variables,
            };
            
            // get description variable
            var companyLanguage = GetLanguage(_httpContext.User.GetAccountId());
            var culture = new CultureInfo(companyLanguage);
            JObject variablesJObject = JObject.Parse(model.Variables);
            var descriptionVariable = new Dictionary<string, string>();
            foreach (var property in variablesJObject.Properties())
            {
                descriptionVariable.Add(property.Name, MailTemplateResource.ResourceManager.GetString("description" + property.Name, culture));
            }
            model.DetailVariables = JsonConvert.SerializeObject(descriptionVariable);

            return model;
        }

        public string GetLanguage(int accountId)
        {
            Account account = null;
            if(accountId != 0) account = _unitOfWork.AccountRepository.GetById(accountId);
            
            if (account != null && !string.IsNullOrEmpty(account.Language))
            {
                return account.Language;
            }
            else
            {
                return "en-US";
            }
        }
       

        #region Get mail template default


        private void GetMailTemplate_WelcomeUserEmail(out string subject, out string variables, out string body)
        {
            var companyLanguage = GetLanguage(0);
            var culture = new CultureInfo(companyLanguage);
            var supportMail = _mailService.GetSupportMailAddress();
            subject = MailContentResource.ResourceManager.GetString("SubjectCompanyAccount", culture);
            string customerSupport = String.Format(MailContentResource.ResourceManager.GetString("BodyCustomerSupport", culture), MailContentResource.ResourceManager.GetString("BodyWorkingTimeInfo", culture), supportMail);
            string replyMessage = String.Format(MailContentResource.ResourceManager.GetString("BodyReplyMessage", culture), supportMail);

            var downloadLink_Andoid = Constants.Link.Android_Download;
            var downloadLink_IOS = Constants.Link.IOS_Download;

            var frontendURL = _mailService.GetFrontEndURL();
            if (frontendURL == null) throw new Exception(MessageResource.NullFrontEndURL);
            var resetLink = frontendURL + "/reset-password/" + "{{" + Constants.VariableEmailTemplate.Token + "}}";
            string password = String.Format(MailContentResource.ResourceManager.GetString("BodyWelcomeUserPassword", culture), resetLink);
            string welcome = MailContentResource.ResourceManager.GetString("BodyWelcome", culture);
            string welcomeGreeting = String.Format(MailContentResource.ResourceManager.GetString("BodyWelcomeUserGreeting", culture), "{{" + Constants.VariableEmailTemplate.UserNameWelcome + "}}");
            string downloadExplain = String.Format(MailContentResource.ResourceManager.GetString("BodyWelcomeDownloadExplain", culture));
            string downloadButton_Android = MailContentResource.ResourceManager.GetString("BodyWelcomeAndroidDownload", culture);
            string downloadButton_IOS = MailContentResource.ResourceManager.GetString("BodyWelcomeIOSDownload", culture);

            variables = InsertValueDefaultForVariables(new List<int>()
            {
                (int) Constants.VariableEmailTemplate.Token,
                (int) Constants.VariableEmailTemplate.LogoImageId,
                // (int) Constants.VariableEmailTemplate.AndroidIcon,
                // (int) Constants.VariableEmailTemplate.IosIcon,
                (int) Constants.VariableEmailTemplate.UserNameWelcome,
                (int) Constants.VariableEmailTemplate.EmailWelcome,
                (int) Constants.VariableEmailTemplate.PasswordDefault,
                (int) Constants.VariableEmailTemplate.FireCrackerIcon,
                (int) Constants.VariableEmailTemplate.GetItOnGooglePlayIcon,
                (int) Constants.VariableEmailTemplate.DownloadOnAppStoreIcon,
                (int) Constants.VariableEmailTemplate.CompanyLogo,
            });

            body = GetDefaultBodyEmailTemplate("Welcome_User_Email.html", new List<string>()
            {
                downloadLink_Andoid,
                downloadLink_IOS,
                "Download Link",
                "{{" + Constants.VariableEmailTemplate.EmailWelcome + "}}",
                password,
                welcome,
                welcomeGreeting,
                downloadExplain,
                downloadButton_Android,
                downloadButton_IOS,
                "{{" + Constants.VariableEmailTemplate.PasswordDefault + "}}",
                "",
                customerSupport,
                replyMessage,
                "",
            });
        }

      
        public string UpdateVariablesToURL(MailTemplate mailTemplate, Dictionary<string, string> variables)
        {
            Dictionary<string, string> dictionaryVariables = new Dictionary<string, string>();
            foreach (var variable in variables)
            {
                if (variable.Value != null && variable.Value is string stringValue)
                {
                    // Check if the string is base64-encoded
                    if (variable.Key.Equals(Constants.VariableEmailTemplate.LogoImageId) || variable.Key.Equals(Constants.VariableEmailTemplate.CompanyLogo))
                    { 
                        if (stringValue.Contains(Constants.Image.DefaultHeaderPng))
                        {
                            var logoImageUrl = variable.Value;
                            // string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                            // string path = $"{Constants.Settings.DefineFolderImages}/setting/{Constants.Settings.Logo}.jpg";
                            // bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                            // logoImageUrl = $"{hostApi}/static/{path}";

                            dictionaryVariables.Add(variable.Key, logoImageUrl);
                        }

                    } 
                    else if (stringValue.Contains(Constants.Image.DefaultHeaderPng))
                    {

                        var logoImageUrl = variable.Value;
                        // string hostApi = _configuration.GetSection(Constants.Settings.DefineConnectionApi).Get<string>();
                        // string path = $"{Constants.Settings.DefineFolderImages}/setting/{Guid.NewGuid().ToString()}.jpg";
                        // bool isSaveImage = FileHelpers.SaveFileImage(logoImageUrl, path);
                        // logoImageUrl = $"{hostApi}/static/{path}";

                        dictionaryVariables.Add(variable.Key, logoImageUrl);
                    }
                    else 
                    {
                        dictionaryVariables.Add(variable.Key, variable.Value);
                    }
                } 
            }
            return JsonConvert.SerializeObject(dictionaryVariables);
        }


        public bool ChangeLinkImageMailTemplate(int companyId)
        {
            bool isSuccess = false;
            _unitOfWork.AppDbContext.Database.CreateExecutionStrategy().Execute(() =>
            {
                using (var transaction = _unitOfWork.AppDbContext.Database.BeginTransaction())
                {
                    try
                    {
                        List<MailTemplate> mailTemplateList = _unitOfWork.MailTemplateRepository.Gets().ToList();

                        foreach (var mailTemplate in mailTemplateList)
                        {
                            
                            if (mailTemplate != null)
                            {
                                mailTemplate.Variables = UpdateVariablesToURL(mailTemplate, JsonConvert.DeserializeObject<Dictionary<string, string>>(mailTemplate.Variables));
                                _unitOfWork.MailTemplateRepository.Update(mailTemplate);
                                _unitOfWork.Save();
                            }
                        }

                        transaction.Commit();
                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        _ = ex.StackTrace;

                        transaction.Rollback();
                        isSuccess = false;
                    }
                }
            });
            return isSuccess;
        }

        #endregion
    }

}