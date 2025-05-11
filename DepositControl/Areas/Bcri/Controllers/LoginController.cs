/** @pp
 * rootnamespace: DepositControl
 */
using System;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using DepositControl.Areas.Bcri.Models;
using DNF.Security.Bussines;
using System.Web.Configuration;
using Type = DNF.Type.Bussines.Type;
using Bcri.Core;

using System.Configuration;
using Bcri.Core.Bussines;
using DepositControl.Areas.Bcri.Utility;

namespace DepositControl.Areas.Bcri.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View(); //RedirectToAction("Index", "Home");
        }
        public ActionResult UseThisSession()
        {
            Current.User.SessionId = Session.SessionID;
            Current.User.HasActivity();
            Current.User.Save();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public ActionResult Login(string name, string password, bool remember = true, string ReturnUrl = "")
        {
            User user = DNF.Security.Bussines.User.Dao.GetByName(name);

            if (user == null)
            {
                ViewData["error"] = "User/Password incorrect.";
                return View();
            }
            /*
            if (user.FailLoginCount >= 5)
            {
                ViewData["error"] = "You exceeded the number of tries.Try again in 5 minutes";
                return View();
            } */

            if (!user.Login(password))
            {
                switch (user.State.Code)
                {
                    case "Disable"://2
                        {
                            ViewData["error"] += " Your user has been disabled.\n" +
                                                   "An email was send to your account with the procedure to re-activate your account.";
                            user.PasswordRecover();

                            var message = new MailMessage();
                            message.To.Add(user.Email);
                            message.Subject = "BCRI - Blocked user";
                            if (Request.Url != null)
                                message.Body = $@"Hello {user.Name},<br />
Your BCRI account was blocked please follow this link <a href='{Url.Action("GeneratePassword", "Login", new { key = user.ActivationKey },Request.Url.Scheme)}'> here</a> to enable it again.
< br /> Greetings. <br /> The BCRI team  <br />";
                            SendEmail(message);

                            break;
                        }
                    case "Delete"://3
                        ViewData["error"] += " Your user has been deleted";
                        break;
                    case "New"://4
                        ViewData["error"] += "Wellcome, your user was not validated yet" +
                                               "Please, review your emails and follow the instructions.";
                        break;
                    default:
                        ViewData["error"] =
                            $"User/Password incorrect. \nYou have {(5 - user.FailLoginCount)} tries.\n\n";
                        break;
                }

                return View();
            }
            if (user.Accesses.Count == 0)
            {
                //LogAccion.Dao.AddLog("LogOut"
                //    //, "The user " + Current.User?.Name + " has logged out."
                //    , Current.User?.Name
                //    );

                //se loguea en el DNF
                user.LoginOut();
                // FormsAuthentication.SignOut();
                ViewData["error"] = "The user does not have permission to perform this action. Please communicate with the administrator.";

                return View();
            }

            FormsAuthentication.SetAuthCookie(user.Name, remember);
            Session["User"] = user.Id;
            string decodedUrl = Server.UrlDecode(ReturnUrl);
            if (Url.IsLocalUrl(decodedUrl))
                return Json(new { redirect = decodedUrl }, JsonRequestBehavior.AllowGet);

            LogAccion.Dao.AddLog("LogIn"
                //, "The user " + Current.User?.Name + " has logged in."
                , Current.User?.Name
                );

            //  return Json(new { redirect = Url.Action("Dashboard", "Dashboard", new { area = "Dashboard", id = user.Id }) }, JsonRequestBehavior.AllowGet); 
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public ActionResult GetToken(string username, string password)
        {

            var TokenTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["TokenTimeOutMinutes"]);
            if (TokenTimeOut == 0)
            {
                Response.StatusCode = 401;
                return Content("Tokens aren't enabled in this proyect, plaese set [TokenTimeOutMinutes] in WebConfig to enable");
            }

            User user = DNF.Security.Bussines.User.Dao.GetByName(username);
            if (user == null || (!user.Login(password)))
            {
                Response.StatusCode = 401;

                switch (user?.State.Code)
                {
                    case "Disable"://2
                    case "Delete"://3
                       return Content(" Your user has been " + user.State.Code);
                    case "New"://4
                        return Content("Wellcome, your user was not validated yet, Please, review your emails and follow the instructions.");
                    default:
                        return Content($"User/Password incorrect.");
                        
                }
            }
           
            return Json(new
            {
                Current.User.Id,
                Current.User.FullName,
                Current.User.Name,
                Current.User.Token,
                TokenExpiredDate = Current.User.TokenEmitDate.Value.AddMinutes(TokenTimeOut),
                IsInSimulation = Current.User.IsInSimulation()
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult LogOut()
        {
            try
            {
                //LogAccion.Dao.AddLog("LogOut"
                //    //, "The user " + Current.User?.Name + " has logged out."
                //    , Current.User?.Name
                //    );

                //se loguea en el DNF
                if (Current.User != null) Current.User.LoginOut();
                Session.Clear();
                Session.Abandon();
                FormsAuthentication.SignOut();
                Session["ReportDate"] = null;
                Session.Abandon();

            }
            catch
            {
                // ignored
            }
            finally
            {
                FormsAuthentication.SignOut();
            }
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseActiveDirectory"]))
                return RedirectToAction("SessionEnd", "Out");

            return RedirectToAction("Login", "Login", new { id = 0 });


        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(GeneratePass pass)
        {
            //Current.User.Password = pass.Password;
            //Current.User.Save();

            //return Json(new { ok = true }, JsonRequestBehavior.AllowGet);
            var user = Current.User;

            if (!user.ValidatePassword(pass.ActualPassword))
            {
                ViewData["error"] = "The current password is not correct";
                return View();
            }
            if (pass.ConfirmPassword != pass.Password)
            {
                ViewData["error"] = "Passwords are not equal";
                return View();
            }

            user.Password = pass.Password;
            user.Save();
            ViewData["Ok"] = true;
            return View();
        }

        public ActionResult GeneratePassword(string key)
        {
            
            if (string.IsNullOrEmpty(key))
            {
                ViewBag.ErrorMessage = "User not found.Please try again.";
                return View();
            }
            
            var user = DNF.Security.Bussines.User.Dao.GetByKey(key);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Incorrect password. Please try again.";
                return View();
            }


            GeneratePass generatePass = new GeneratePass
            {
                id = user.Id,
                FullName = user.FullName,
                Name = user.Name
            };
            return View(generatePass);
        }

        [HttpPost]
        public ActionResult GeneratePassword(GeneratePass generatePass)
        {
            User user = DNF.Security.Bussines.User.Dao.Get(generatePass.id);

            if (generatePass.Password != generatePass.ConfirmPassword)
            {
                @ViewData["error"] = "The passwords are not equal";
                return View();
            }

            user.Password = generatePass.Password;
            user.ActivationKey = "";
            user.State = new Type("UserState", "Enable");
            
            user.Save();

            if (user.Login(generatePass.Password))
            {
                FormsAuthentication.SetAuthCookie(user.Name, true);
                LogAccion.Dao.AddLog("LogIn", Current.User?.Name);

                return RedirectToAction("Index", "Home");
            }

            @ViewData["error"] = "An unexpected error happeneed. Please try again later";
            return View();
            


        }

        public ActionResult ResetPassword()
        {
            return View();
        }

        /*
           To send the email using the user: soporte@consensusgroup.net
           MailMessage MUST have:
             -user you want to send  
           MailMessage should have
            -the message body
            -the subject.
        */
        public void SendEmail(MailMessage message)
        {
            var smtp = new SmtpClient();
            //Cast the newtwork credentials in to the NetworkCredential class and use it .
            var credential = (NetworkCredential)smtp.Credentials;

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential(SettingsSiteUtility.SMTPUser, SettingsSiteUtility.SMTPPassword);
            smtp.Host = SettingsSiteUtility.SMTPServerId;
            smtp.Port = SettingsSiteUtility.SMTPPort;
            message.From = new MailAddress(SettingsSiteUtility.SMTPUser);
            message.IsBodyHtml = true;
            message.Body = message.Body;

            try
            {
                smtp.Send(message);
                ViewBag.EmailSended = "true";
            }
            catch (Exception ex)
            {
                @ViewData["error"] = "Error at sending the email. Please try again later";

            }
        }

        [HttpPost]
        public ActionResult ResetPassword(string email)
        {

            var user = DNF.Security.Bussines.User.Dao.GetByName(email);

            if (user == null)
            {
                ViewData["error"] = "The email is not registered.";
                return View();
            }

            user.PasswordRecover();


            MailMessage message = new MailMessage();
            message.To.Add(user.Email);
            message.Subject = "BCRI - Password recovery";
            message.Body = $@"Hello {user.FullName},<br />
If you’ve forgotten your BCRI password please follow this {Environment.NewLine}link <a href='{Url.Action("GeneratePassword", "Login", new { key = user.ActivationKey },Request.Url.Scheme)}'>here</a> to change your pass.
<br />Greetings <br /> <br /> The BCRI team  <br />";
            SendEmail(message);

            //            mailer.PasswordReset(user.Email, user.FullName, user.ActivationKey);
            return View();
        }

        public ActionResult LoginREST(string email, string password)
        {
            string error = null;
            User user = DNF.Security.Bussines.User.Dao.GetByName(email);
            if (user == null)
            {
                error = "Email is empty";
                return Json(new
                {
                    success = false,
                    error = error
                }, JsonRequestBehavior.AllowGet);
            }

            if (!user.Login(password))
            {
                switch (user.State.Code)
                {
                    case "Disable"://2
                        error += " Your user has been disabled.\nAn email was send to your account with the procedure to re-activate your account.";
                        break;
                    case "Delete"://3
                        error += " Your user has been deleted";
                        break;
                    case "New"://4
                        error += "Welcome, your user was not validated yet. Please, review your emails and follow the instructions.";
                        break;
                    default:
                        error = $"User/Password incorrect. \nYou have {(5 - user.FailLoginCount)} tries.\n\n";
                        break;
                }

                return Json(new
                {
                    success = false,
                    error = error
                }, JsonRequestBehavior.AllowGet);
            }
            if (user.Accesses.Count == 0)
            {
                //LogAccion.Dao.AddLog("LogOut"
                //                //, "The user " + Current.User?.Name + " has logged out."
                //                , Current.User?.Name
                //                );

                //se loguea en el DNF
                user.LoginOut();
                // FormsAuthentication.SignOut();
                ViewData["error"] = "The user does not have permission to perform this action. Please communicate with the administrator.";

                return Json(new
                {
                    success = false,
                    error = error
                }, JsonRequestBehavior.AllowGet);
            }

            System.Web.Security.FormsAuthentication.SetAuthCookie(user.Name, false);

            LogAccion.Dao.AddLog("LogIn"
                //, "The user " + Current.User?.Name + " has logged in."
                , Current.User?.Name
                );

            //  return Json(new { redirect = Url.Action("Dashboard", "Dashboard", new { area = "Dashboard", id = user.Id }) }, JsonRequestBehavior.AllowGet); 
            return Json(new
            {
                FullName = Current.User.FullName,
                Id = Current.User.Id,
                Name = Current.User.Name,
            }, JsonRequestBehavior.AllowGet);
        }


    }
}
