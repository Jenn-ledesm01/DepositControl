using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using DNF.Security.Bussines;
using System.Security.Principal;
using System.Net.Http;
using DepositControl.Areas.Bcri.Models;

namespace DepositControl
{
    public static class SecurityUtility
    {
        public static void WebConfigEncrypt()
        {
            NameValueCollection section = (NameValueCollection)ConfigurationManager.GetSection("secureAppSettings");
            if (section == null) return;

            Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
            ConfigurationSection configSection = config.GetSection("connectionStrings");
            ConfigurationSection configSectionApp = config.GetSection("appSettings");

            if (bool.Parse(section["WebEncrypt"]))
            {
                if (configSection.SectionInformation.IsProtected)
                    return;
                /* Si no esta encriptado, lo encripto */
                configSection.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                configSectionApp.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                config.Save();
            }
            else
            {   
                if (!configSection.SectionInformation.IsProtected)
                    return;
                /* Si esta encriptado, lo desencripto */
                configSection.SectionInformation.UnprotectSection();
                configSectionApp.SectionInformation.UnprotectSection();
                config.Save();
            }
        }
       
        public static bool LoginUserFromToken(NameValueCollection Headers)
        {

            var tokenKey = Headers.AllKeys.FirstOrDefault(x => x.ToLower() == "token");
            if (tokenKey == null)
                return false;

            var token = Headers[tokenKey]?.Trim();
            if(string.IsNullOrWhiteSpace(token) || token.Length != 36)
                return false;

            var user = User.Dao.GetByToken(token);

            return (user?.LoginToken() == true);
        }
        public static bool LoginUserFromCookie(HttpCookieCollection cookies)
        {
            if (FormsAuthentication.CookiesSupported != true) return false;
            if (string.IsNullOrEmpty(cookies[FormsAuthentication.FormsCookieName]?.Value)) return false;
            var cookieValue = cookies[FormsAuthentication.FormsCookieName].Value;
            try
            {
                FormsAuthenticationTicket formsAuthenticationTicket = FormsAuthentication.Decrypt(cookieValue);

                if (string.IsNullOrEmpty(formsAuthenticationTicket?.Name)) return false;
                string username = formsAuthenticationTicket.Name;

                if (Current.User != null && Current.User?.Name == username) return true;

                Current.User = User.Dao.GetByName(username);
                bool loged = (Current.User != null && Current.User.Login()); //aca hace el login
                if (loged)
                {
                    LogAccion.Dao.AddLog("LogIn"
                        //, $"The user {Current.User?.Name} has logged in."
                        , Current.User?.Name
                        );
                }
                return loged;
            }
            catch (CryptographicException cex)
            {
                FormsAuthentication.SignOut();
            }
            return false;
        }

        public static string GetParentAccess(Access access, bool first = true)
        {
            // se remplaza por acces.Path
            if (access == null) return "";

            if (access.Parent?.IsLoad == false)
                access.Parent = Access.Dao.Get(access.Parent.Id);

            if (first) return GetParentAccess(access, false) + access.Name + "]";

            else if (access.Parent != null) return GetParentAccess(access.Parent, false) + access.Parent.Name + "]->" + "[";
            else return "[";
        }


    }
}
