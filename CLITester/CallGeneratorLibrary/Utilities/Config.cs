using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace CallGeneratorLibrary.Utilities
{
    public class Config
    {
        private static string _SmtpServer = null;
        public static string SmtpServer
        {
            get
            {
                if (_SmtpServer == null)
                    _SmtpServer = WebConfigurationManager.AppSettings["SmtpServer"];
                return _SmtpServer;
            }
        }

        private static string _SendingEmail = null;
        public static string SendingEmail
        {
            get
            {
                if (_SendingEmail == null)
                    _SendingEmail = WebConfigurationManager.AppSettings["SendingEmail"];
                return _SendingEmail;
            }
        }

        private static string _SiteUrl;
        public static string SiteUrl
        {
            get
            {
                if (_SiteUrl == null)
                    _SiteUrl = WebConfigurationManager.AppSettings["SiteUrl"];
                return _SiteUrl;
            }
        }

        private static string _WebsiteUrl;
        public static string WebsiteUrl
        {
            get
            {
                if (_WebsiteUrl == null)
                    _WebsiteUrl = WebConfigurationManager.AppSettings["WebsiteUrl"];
                return _WebsiteUrl;
            }
        }

        private static string _ConfirmPage;
        public static string ConfirmPage
        {
            get
            {
                if (_ConfirmPage == null)
                    _ConfirmPage = WebConfigurationManager.AppSettings["ConfirmPage"];
                return _ConfirmPage;
            }
        }

        private static string _RequestPassword;
        public static string RequestPassword
        {
            get
            {
                if (_RequestPassword == null)
                    _RequestPassword = WebConfigurationManager.AppSettings["RequestPassword"];
                return _RequestPassword;
            }
        }

        private static string _ProjectsResources;
        public static string ProjectsResources
        {
            get
            {
                if (_ProjectsResources == null)
                    _ProjectsResources = WebConfigurationManager.AppSettings["ProjectsResources"];
                return _ProjectsResources;
            }
        }
    }
}
