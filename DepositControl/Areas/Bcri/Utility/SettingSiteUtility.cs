using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using NuGet;

namespace DepositControl.Areas.Bcri.Utility
{
    public static class SettingsSiteUtility
    {

        //public static string Aplication => WebConfigurationManager.AppSettings["Aplication"] ?? "BPAIR";
        //public static string TimeOut => WebConfigurationManager.AppSettings["TimeOut"] ?? "999999";
        //public static string MultiCompany => WebConfigurationManager.AppSettings["MultiCompany"] ?? "false";
        //public static string ClientValidationEnabled => WebConfigurationManager.AppSettings["ClientValidationEnabled"] ?? "true";
        //public static string UnobtrusiveJavaScriptEnabled => WebConfigurationManager.AppSettings["UnobtrusiveJavaScriptEnabled"] ?? "true";
        //public static string UseActiveDirectory => WebConfigurationManager.AppSettings["UseActiveDirectory"] ?? "false";
        //public static string ActiveDirectoryPath => WebConfigurationManager.AppSettings["ActiveDirectoryPath"] ?? "LDAP://mza.consensus";
        //public static string ActiveDirectoryImpersonalizeDomUserPass => WebConfigurationManager.AppSettings["ActiveDirectoryImpersonalizeDomUserPass"] ?? "mza;bitg;123456a*";
        //public static string RunMigration => WebConfigurationManager.AppSettings["RunMigration"] ?? "0";

        /// <summary>
        ///     Obtiene un valor configurable para la aplicacion. Si se define un default value se retornara este si no existe la configuracion indicada 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static string GetConfigValue(string key, string defaultValue = null)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                return ConfigurationManager.AppSettings[key];
            }
            else
            {
                if (defaultValue != null)
                {                                     
                    //Aqui se tendria que grabar la configuracion con el valor por defecto pedido
                }

                return defaultValue;
            }
        }

        private static string GetNugetPackageVersion(string package, string defaultValue = null)
        {
            string path = HttpContext.Current.Server.MapPath(@"~\packages.config");
            var packageReferences = new PackageReferenceFile(path).GetPackageReferences();
            if (packageReferences.Count() >= 1)
            {
                var dllReference = packageReferences.Where(x => x.Id == package).FirstOrDefault();
                return dllReference?.Version.ToString() ?? defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }

        private static bool GetConfigAsBoolean(string key, bool? defaultValue = null)
        {
            var value = SettingsSiteUtility.GetConfigValue(key, defaultValue == null ? null : defaultValue.Value.ToString().ToLower());
            
            return Convert.ToBoolean(value);            
        }

        public static bool AllowDoubleSession => GetConfigAsBoolean("AllowDoubleSession", true);          
        public static bool AllowDoubleTab =>  GetConfigAsBoolean("AllowDoubleTab", true);
        public static bool AllowUrlNavigation => GetConfigAsBoolean("AllowUrlNavigation", true);
        public static bool LoadingScreenOnJqAjax => GetConfigAsBoolean("LoadingScreenOnJqAjax", false);
        public static bool UserChooseCulture => GetConfigAsBoolean("UserChooseCulture", false);
        public static string BCRIWebVersion => GetNugetPackageVersion("Bcri" + ".Web", "1.0.0.0");//Esto se hizo asi porque sino el power shell nos reemplaza el Bcri.web por el namespace del proyecto destino
        public static string HeaderUsed => GetConfigValue("HeaderUsed", "DefaultHeader");
        public static string FooterUsed => GetConfigValue("FooterUsed", "DefaultFooter");
        public static string DefaultCurrency => GetConfigValue("DefaultCurrencyFormat", "USD");
        public static string SMTPUser => GetConfigValue("SMTPUser");
        public static string SMTPPassword => GetConfigValue("SMTPPassword");
        public static string SMTPServerId => GetConfigValue("SMTPServerId");
        public static int SMTPPort => Convert.ToInt32(GetConfigValue("SMTPPort", "587"));
    }
}
