using Entity;
using ExamOnlineSystem.Common;
using ExamOnlineSystem.Models;
using Facebook;
using System.Web.Security;
using System.Web.UI.WebControls;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Drive.v2;
using Google.Apis.Gmail.v1;
using Google.Apis.AnalyticsReporting.v4;

using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;



namespace ExamOnlineSystem.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        private Uri RedirectUriGoogle
        {
            get
            {
                var uriBulder = new UriBuilder(Request.Url);
                uriBulder.Query = null;
                uriBulder.Fragment = null;
                uriBulder.Path = Url.Action("GoogleCallback");
                return uriBulder.Uri;
            }
        }

        private Uri RedirectUriFacebook
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUriFacebook.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        /*public ActionResult LoginGoogle()
        {
            var go = new ClientSecrets();
            go.ClientId = ConfigurationManager.AppSettings["GoAppId"];
            go.ClientSecret = ConfigurationManager.AppSettings["GoAppSecret"];
        }*/

        //public class AppFlowMetadata : Users
        //{
        //    public override string GetUserId(Controller controller)
        //    {
        //        // In this sample we use the session to store the user identifiers.
        //        // That's not the best practice, because you should have a logic to identify
        //        // a user. You might want to use "OpenID Connect".
        //        // You can read more about the protocol in the following link:
        //        // https://developers.google.com/accounts/docs/OAuth2Login.
        //        var user = controller.Session["user"];
        //        if (user == null)
        //        {
        //            user = Guid.NewGuid();
        //            controller.Session["user"] = user;
        //        }
        //        return user.ToString();

        //    }


        //    public static IAuthorizationCodeFlow Flow1 { get; } = new Google.Apis.Auth.OAuth2.Flows.GoogleAuthorizationCodeFlow(new Google.Apis.Auth.OAuth2.Flows.GoogleAuthorizationCodeFlow.Initializer
        //    {
        //        ClientSecrets = new ClientSecrets
        //        {
        //            ClientId = ConfigurationManager.AppSettings["GoAppId"],
        //            ClientSecret = ConfigurationManager.AppSettings["GoAppSecret"]
        //        },
        //        Scopes = "email",
        //        DataStore = new Google.Apis.Util.Store.FileDataStore("Drive.Api.Auth.Store")
        //    });
        //}

        public class AppFlowMetadata : FlowMetadata
        {
            private static readonly IAuthorizationCodeFlow flow =
                new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = "315813138266-hjbmfedsma8q0hg2t5bhk41khgu93but.apps.googleusercontent.com",
                        ClientSecret = "ZTYK6BVht8dWhaKVfaRIOTUK"
                    },
                    Scopes = new[] { GmailService.Scope.MailGoogleCom },
                    DataStore = new FileDataStore("MailGoogleCom.Api.Auth.Store")
                });

            public override string GetUserId(Controller controller)
            {
                // In this sample we use the session to store the user identifiers.
                // That's not the best practice, because you should have a logic to identify
                // a user. You might want to use "OpenID Connect".
                // You can read more about the protocol in the following link:
                // https://developers.google.com/accounts/docs/OAuth2Login.
                var user = controller.Session["user"];
                if (user == null)
                {
                    user = Guid.NewGuid();
                    controller.Session["user"] = user;
                }
                return user.ToString();

            }

            public override IAuthorizationCodeFlow Flow
            {
                get { return flow; }
            }
        }

        public ActionResult LoginGoogle(System.Threading.CancellationToken cancellationToken)
        {
            var result = new AuthorizationCodeMvcApp(this, new AppFlowMetadata()).
                AuthorizeAsync(cancellationToken).Result;

            if (result.Credential != null)
            {
                var service = new DriveService(new Google.Apis.Services.BaseClientService.Initializer
                {
                    HttpClientInitializer = result.Credential,
                    ApplicationName = "ASP.NET MVC Sample"
                });

                // YOUR CODE SHOULD BE HERE..
                // SAMPLE CODE:
                var list = service.Files.List().ExecuteAsync().Result;
                ViewBag.Message = "FILE COUNT IS: " + list.Items.Count();
                return View();
            }
            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }

        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUriFacebook.AbsoluteUri,
                code = code
            });
            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fields=first_name,middle_name,last_name,id,email,birthday");
                string email = string.IsNullOrEmpty(me.email) ? "" : me.email;
                DateTime? birthday = string.IsNullOrEmpty(me.birthday) ? null : Convert.ToDateTime(me.birthday);
                string firstName = string.IsNullOrEmpty(me.first_name) ? "" : me.first_name;
                string middelName = string.IsNullOrEmpty(me.middle_name) ? "" : me.middle_name;
                string lastName = string.IsNullOrEmpty(me.last_name) ? "" : me.last_name;

                if (string.IsNullOrEmpty(email))
                {
                    return HttpNotFound();
                }
                var user = new Users
                {
                    Email = email,
                    UserName = email,
                    Name = firstName + " " + middelName + " " + lastName,
                    CreateDate = DateTime.Now,
                    Birthday = birthday,
                    Password = Guid.NewGuid().ToString()
                };
                var resultInsert = new DAL.UserContext().InsertForFacebook(user);
                if (resultInsert > 0)
                {
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserId = user.Id;
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                }
            }
            return Redirect("/");
        }

        // GET: /Admin/Login
        public ActionResult LogIn()
        {
            return View();
        }
        //Post: /Admin/Login
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var users = new DAL.UserContext();
                var result = users.Login(model.UserName, model.Password);
                if (result)
                {
                    var User = users.GetByUserName(model.UserName);
                    var userSession = new UserLogin();
                    userSession.UserName = User.UserName;
                    userSession.UserId = User.Id;
                    Session.Add(CommonConstants.USER_SESSION, userSession);
                    return RedirectToAction("Index", "HomeAdmin");
                }
                else
                {
                    ModelState.AddModelError("", "Vui lòng kiểm tra lại Tài khoản.");
                }
            }
            return View("Login");
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(RegisterModel model)
        {

            if (ModelState.IsValid)
            {
                var db = new DAL.UserContext();
                if (db.IsExistUserName(model.UserName))
                {
                    ModelState.AddModelError("", "Tên Đăng nhập đã tồn tại.");
                }
                else if (db.IsEmail(model.Email))

                {
                    ModelState.AddModelError("", "Email đã được sử dụng.");
                }
                else if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "Mật khẩu không khớp");
                }
                else
                {
                    var user = new Users();
                    user.UserName = model.UserName;
                    user.Name = model.Name;
                    user.Email = model.Email;
                    user.Password = model.Password;
                    user.Birthday = model.Birthday;

                    var result = db.Insert(user);
                    if (result > 0)
                    {
                        ViewBag.Success = "Đăng kí thành công";
                        model = new RegisterModel();
                        return RedirectToAction("Index", "HomeAdmin");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đăng kí thất bại");
                    }
                }
            }
            return View(model);
        }
        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Account/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("vandat1095@gmail.com", "Server");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "vandat"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]

        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            string message = "";
            bool status = false;
            if (ModelState.IsValid)
            {
                //Verify Email ID
                //Generate Reset password link 
                //Send Email 
                var db = new DAL.UserContext();

                if (db.IsEmail(model.Email))
                {
                    Random random = new Random();
                    long resetCode = random.Next(100000);
                    SendVerificationLinkEmail(model.Email, resetCode, "ResetPassword");
                    model.ResetPasswordCode = resetCode;
                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                    //in our model class in part 1
                    var user = db.GetByEmail(model.Email);
                    user.ResetPasswordCode = model.ResetPasswordCode;
                    db.Update(user);
                    message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        private void SendVerificationLinkEmail(string email, long resetCode, string v)
        {
            throw new NotImplementedException();
        }

        public void SendMail(string toEmailAddress, string subject, string content)
        {
            var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
            var fromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"].ToString();
            var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
            var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
            var smtpPort = ConfigurationManager.AppSettings["SMTPPort"].ToString();

            bool enabledSsl = bool.Parse(ConfigurationManager.AppSettings["EnabledSSL"].ToString());

            string body = content;
            MailMessage message = new MailMessage(new MailAddress(fromEmailAddress, fromEmailDisplayName), new MailAddress(toEmailAddress));
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;

            var client = new SmtpClient();
            client.Credentials = new NetworkCredential(fromEmailAddress, fromEmailPassword);
            client.Host = smtpHost;
            client.EnableSsl = enabledSsl;
            client.Port = !string.IsNullOrEmpty(smtpPort) ? Convert.ToInt32(smtpPort) : 0;
            client.Send(message);
        }
        public ActionResult ResetPassword(long resetPassword)
        {

            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (resetPassword == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                var db = new DAL.UserContext();
                var user = new Users();
                if (user.ResetPasswordCode == resetPassword)
                {
                    if (db.IsResetPasswordCodeExist(user.ResetPasswordCode))
                    {
                        ResetPasswordModel model = new ResetPasswordModel();
                        model.ResetPasswordCode = resetPassword;
                        return View(model);
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }
                /*if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }*/

            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            var db = new DAL.UserContext();
            if (ModelState.IsValid)
            {
                if (db.IsResetPasswordCodeExist(model.ResetPasswordCode))
                {
                    var user = new Users();

                    user.Password = model.NewPassword;
                    if (model.NewPassword != model.ConfirmPassword)
                    {
                        ModelState.AddModelError("", "Mật khẩu không khớp.");
                    }
                    else
                    {
                        var result = db.Update(user);
                        if (result > 0)
                        {
                            message = "Reset Password thành công.";
                            model = new ResetPasswordModel();
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            message = "Reset Thất bại";
                        }
                    }
                }
                else
                {
                    message = "Something invalid";
                }
            }

            ViewBag.Message = message;
            return View(model);
        }
    }

    public class AuthCallbackController : Google.Apis.Auth.OAuth2.Mvc.Controllers.AuthCallbackController
    {
        protected override Google.Apis.Auth.OAuth2.Mvc.FlowMetadata FlowData
        {
            get { return new UserController.AppFlowMetadata(); }
        }
    }

}
