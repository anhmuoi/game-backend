using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataModel.Email;
using ERPSystem.Common.Resources;
using System.Threading;
using ERPSystem.DataAccess.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using System.Globalization;
using ERPSystem.Repository;
using ERPSystem.Service.Infrastructure;
using Newtonsoft.Json;
using ERPSystem.Common;

namespace ERPSystem.Service
{
    public interface IMailService
    {
        bool SendMail(string to, string[] bcc, string subject, string body, Stream attachment = null,
            string fileName = null, string fileType = null);

        bool SendMail(MailMessage message, Stream attachment = null, string fileName = null, string fileType = null);
       
        string GetSupportMailAddress();
        string GetPathToTemplateFile(string fileName);
        string GetPathToImageFile(string fileName);
        string ConvertImageToBase64(string pathToImageFile);
        string GetFrontEndURL();
    }
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private IHostingEnvironment _env;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = ApplicationVariables.LoggerFactory.CreateLogger<MailService>();
            _env = ApplicationVariables.Env;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="to"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachment"></param>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public bool SendMail(string to, string[] bcc, string subject, string body
            , Stream attachment = null, string fileName = null, string fileType = null)
        {
            try
            {
                var mailSettings = _configuration.GetSection(Constants.Settings.MailSettings).Get<MailSettings>();

                using (var client = new SmtpClient(mailSettings.Host))
                {
                    client.Host = mailSettings.Host;
                    if (!string.IsNullOrEmpty(mailSettings.Port))
                    {
                        client.Port = int.Parse(mailSettings.Port);
                    }

                    client.EnableSsl = Convert.ToBoolean(mailSettings.EnableSsl);
                    client.UseDefaultCredentials = Convert.ToBoolean(mailSettings.DefaultCredentials);
                    client.Credentials = new System.Net.NetworkCredential(mailSettings.UserName, mailSettings.Password);

                    Attachment att = null;
                    if (attachment != null)
                    {
                        att = new Attachment(attachment, fileName, fileType);
                    }

                    using (var msg = new MailMessage())
                    {
                        var view = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8,
                            "text/html");
                        msg.From = new MailAddress(mailSettings.UserName, mailSettings.From);
                        msg.Sender = new MailAddress(mailSettings.UserName, mailSettings.From);
                        msg.AlternateViews.Add(view);
                        msg.IsBodyHtml = true;
                        msg.SubjectEncoding = Encoding.UTF8;
                        msg.BodyEncoding = Encoding.UTF8;
                        msg.Subject = subject;
                        msg.Body = body;

                        if (att != null)
                            msg.Attachments.Add(att);


                        if (!String.IsNullOrEmpty(to))
                        {
                            try
                            {
                                msg.To.Add(to);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                                _logger.LogError("Mail address : " + to);

                                return false;
                            }
                        
                            try
                            {
                                if (bcc != null && bcc.Any())
                                {
                                    foreach (var email in bcc)
                                    {
                                        msg.Bcc.Add(email);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                            }

                            client.Send(msg);
                        } else
                        {
                            _logger.LogWarning("Email with subject:\"" + subject + "\" missing To address.");
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send E-mail.");
                _logger.LogError("Email address : " + to);
                _logger.LogError("Subject : " + subject);
                _logger.LogError($"{ex.Message}:{Environment.NewLine} {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="to"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachment"></param>
        /// <param name="fileName"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public bool SendMail(MailMessage message, Stream attachment = null, string fileName = null, string fileType = null)
        {
            try
            {
                var mailSettings = _configuration.GetSection(Constants.Settings.MailSettings).Get<MailSettings>();

                using (var client = new SmtpClient(mailSettings.Host))
                {
                    client.Host = mailSettings.Host;
                    if (!string.IsNullOrEmpty(mailSettings.Port))
                    {
                        client.Port = int.Parse(mailSettings.Port);
                    }

                    client.EnableSsl = Convert.ToBoolean(mailSettings.EnableSsl);
                    client.UseDefaultCredentials = Convert.ToBoolean(mailSettings.DefaultCredentials);
                    client.Credentials = new System.Net.NetworkCredential(mailSettings.UserName, mailSettings.Password);

                    if (attachment != null)
                    {
                        Attachment att = new Attachment(attachment, fileName, fileType);
                        message.Attachments.Add(att);
                    }

                    message.From = new MailAddress(mailSettings.UserName, mailSettings.From);
                    message.Sender = new MailAddress(mailSettings.UserName, mailSettings.From);
                    message.IsBodyHtml = true;
                    message.SubjectEncoding = Encoding.UTF8;
                    message.BodyEncoding = Encoding.UTF8;

                    client.Send(message);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send E-mail.");
                _logger.LogError("Email address : " + message.To);
                _logger.LogError("Subject : " + message.Subject);
                _logger.LogError($"{ex.Message}:{Environment.NewLine} {ex.StackTrace}");
                return false;
            }
        }

        

        /// <summary>
        /// get path of template file
        /// </summary>
        /// <param name="fileName"> name of template file </param>
        /// <returns> template file path </returns>
        public string GetPathToTemplateFile(string fileName)
        {
            var webRoot = _env.WebRootPath;

            var pathToFile = webRoot
                    + Path.DirectorySeparatorChar.ToString()
                    + "Templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "EmailTemplate"
                    + Path.DirectorySeparatorChar.ToString()
                    + fileName;

            return pathToFile;
        }

        /// <summary>
        /// get path of image file
        /// </summary>
        /// <param name="fileName"> name of image file </param>
        /// <returns> image file path </returns>
        public string GetPathToImageFile(string fileName)
        {
            var webRoot = _env.WebRootPath;

            var pathToFile = webRoot
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplate"
                            + Path.DirectorySeparatorChar.ToString()
                            + "imagesEmail"
                            + Path.DirectorySeparatorChar.ToString()
                            + fileName;

            return pathToFile;
        }

        /// <summary>
        /// Convert Image file path to Base64
        /// </summary>
        /// <param name="pathToImageFile"></param>
        /// <returns> converted image to Base64 </returns>
        public string ConvertImageToBase64(string pathToImageFile)
        {
            var bytes = System.IO.File.ReadAllBytes(pathToImageFile);
            var b64String = Convert.ToBase64String(bytes);

            return b64String;
        }

        /// <summary>
        /// get front-end URL
        /// </summary>
        /// <returns> front-end URL </returns>
        public string GetFrontEndURL()
        {
            var frontendURL = _configuration.GetSection("WebApp:Host").Value;

            if (string.IsNullOrEmpty(frontendURL))
                return null;

            if (frontendURL.Equals("localhost"))
                frontendURL = "http://" + frontendURL;

            return frontendURL;
        }
        
        /// <summary>
        /// Get Email address for customer support
        /// </summary>
        /// <returns> support Email address </returns>
        public string GetSupportMailAddress()
        {
            var mailDevelopSettings = _configuration.GetSection(Constants.Settings.MailDevelopSettings).Get<MailDevelopSettings>();

            var supportMail = mailDevelopSettings.To;

            return supportMail;
        }
   
    }
}
