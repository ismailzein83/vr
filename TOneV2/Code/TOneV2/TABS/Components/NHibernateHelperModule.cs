using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TABS.Components
{
    public class NHibernateHelperModule : System.Web.IHttpModule
    {
        public static bool? _TOneLicensed = null;
        protected static bool? _Licensed = null;
        static readonly char[] _dlc = new char[] { 'T', 'A', 'B', '$', 'D', 'E', 'V', 'E', 'L', 'O', 'P', 'E', 'R' };
        internal static readonly string _dl = string.Format("{{#$}}{{$}}#{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}#{{$}}{{#$}}", _dlc[0], _dlc[1], _dlc[2], _dlc[3], _dlc[4], _dlc[5], _dlc[6], _dlc[7], _dlc[8], _dlc[9], _dlc[10], _dlc[11], _dlc[12]);
        static readonly char[] _lk = new char[] { 'T', 'A', 'B', 'S', ' ', 'L', 'i', 'c', 'e', 'n', 's', 'e' };

        public static string ActiveLicense { get { return System.Configuration.ConfigurationSettings.AppSettings[new string(_lk)]; } }
        public static int LicenceNotificationDays { get { return int.Parse(System.Configuration.ConfigurationSettings.AppSettings["LicenceNotifierPeriod"]); } } // in days 
        public static DateTime ExpiryWarningDate { get { return IsDeveloperLicense ? DateTime.MaxValue : ExpiryDate.Subtract(TimeSpan.FromDays(LicenceNotificationDays)); } }
        public static TimeSpan LicenceCheckFrequency { get { return TimeSpan.Parse(System.Configuration.ConfigurationSettings.AppSettings["LicenceCheckFrequency"].ToString()); } } // in days 

        public static event EventHandler BeginRequest;
        public static event EventHandler EndRequest;

        //static Regex LicenseParserRegEx = new Regex(@"(?<Key>\w\w(-\w\w){15}):(?<Plugins>[=,\w]+)*:(?<Dates>[=,\w]+):(?<CDPN>[=,\w]+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture); 
        //static Regex LicenseParserRegEx = new Regex(@"(?<Key>\w\w(-\w\w){15}):(?<Plugins>[=,\w]+)*:(?<Dates>[=,\w]+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture); 
        static Regex LicenseParserRegEx = new Regex(@"(?<Key>\w\w(-\w\w){15}):(?<Plugins>[=,\w]+)*:(?<Dates>[=,\w]+):(?<KeyEn>[=,\w]+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public static DateTime ExpiryDate { get; protected set; }

        private static string[] _LicensedPlugins;
        public static string[] LicensedPlugins
        {
            get
            {
                if (!IsDeveloperLicense && _LicensedPlugins == null)
                {
                    Match match = LicenseParserRegEx.Match(ActiveLicense);
                    if (match.Success && match.Groups["Plugins"].Success)
                    {
                        string encodedPlugins = match.Groups["Plugins"].Value;
                        string decodedPlugins = WebHelperLibrary.Utility.SimpleDecode(encodedPlugins);
                        _LicensedPlugins = decodedPlugins.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                        _LicensedPlugins = new string[] { };
                }
                return _LicensedPlugins;
            }
        }
        private static string EncodedLicenseKey { get; set; }
        public static void SendNotification()
        {
            while (true)
            {
                if (DateTime.Now > ExpiryWarningDate)
                {
                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();

                    SpecialSystemParameters.SmtpInfo info = new TABS.SpecialSystemParameters.SmtpInfo();

                    message.To.Add("support@vanrise.com");
                    message.Subject = "T.One Licence renewal notifier";
                    message.IsBodyHtml = true;

                    TimeSpan remainingTime = ExpiryDate.Subtract(DateTime.Now);

                    message.Body = string.Format(
                            @"<b>Licence for {0} Expires On {1:yyyy-MM-dd}</b>.<br/>
                                There are {2} days, {3} hours, {4} minutes remaining for the active license.<br/>
                                Please consider to renew the T.One license<br/>"
                        , TABS.CarrierAccount.SYSTEM
                        , ExpiryDate
                        , remainingTime.Days
                        , remainingTime.Hours
                        , remainingTime.Minutes
                        );

                    message.From = new System.Net.Mail.MailAddress(info.Default_From);

                    Exception ex;
                    TABS.Components.EmailSender.Send(message, out ex);
                }

                System.Threading.Thread.Sleep(LicenceCheckFrequency);
            }
        }
        public static bool IsDeveloperLicense
        {
            get
            {
                return ActiveLicense.Equals(_dl);
            }
        }

        //public static bool IsCDPNIncluded
        //{
        //    get
        //    {
        //        if (IsDeveloperLicense) return true;
        //        Match match = LicenseParserRegEx.Match(ActiveLicense);
        //        string Encodedcdpn = match.Groups["CDPN"].Value;
        //        string Decodedcdpn = WebHelperLibrary.Utility.SimpleDecode(Encodedcdpn);
        //        return Decodedcdpn == "no" ? false : true;
        //    }
        //}


        public static bool IsConnectionLicenceEncrypted()
        {
            if (String.IsNullOrEmpty(ActiveLicense))
                return true;
            bool IsEncrypted = false;
            Match match = LicenseParserRegEx.Match(ActiveLicense);
            if (match.Success && match.Groups["KeyEn"].Success)
            {
                string encodedEncryptionKey = match.Groups["KeyEn"].Value;
                // even if the systemkeymatch is true, the dates must respect licencedate 
                string encodedEncryptionValue = WebHelperLibrary.Utility.SimpleDecode(encodedEncryptionKey);
                IsEncrypted = encodedEncryptionValue == "Encrypted" ? true : false;
            }
            return IsEncrypted;
        }
        internal static bool Licensed
        {
            get
            {
                lock (typeof(NHibernateHelperModule))
                {
                    if (!_Licensed.HasValue)
                    {
                        try
                        {
                            if (ActiveLicense.Equals(_dl))
                            {
                                ExpiryDate = new DateTime(2022, 12, 31);
                                _Licensed = true;
                                return _Licensed.Value;
                            }

                            Match match = LicenseParserRegEx.Match(ActiveLicense);
                            if (match.Success)
                            {
                                EncodedLicenseKey = match.Groups["Key"].Value;
                                string encodedDates = match.Groups["Dates"].Value;
                                // even if the systemkeymatch is true, the dates must respect licencedate 
                                string decodedDates = WebHelperLibrary.Utility.SimpleDecode(encodedDates);
                                DateTime validFromDate = DateTime.MinValue, validToDate = DateTime.MinValue, today = DateTime.Today;
                                WebHelperLibrary.Utility.GetLicenceDates(decodedDates, out validFromDate, out validToDate);
                                ExpiryDate = validToDate;
                                bool isDateValid = today >= validFromDate && today <= validToDate;
                                _Licensed = isDateValid && EncodedLicenseKey.Equals(GetSystemKeyMatch());

                                // thread to check licence 
                                System.Threading.Thread LicenceCheckRenewalThread = new System.Threading.Thread(new System.Threading.ThreadStart(SendNotification));
                                LicenceCheckRenewalThread.Start();
                            }
                            else
                                _Licensed = false;
                        }
                        catch
                        {
                            _Licensed = false;
                        }
                    }
                }
                return _Licensed.Value;
            }
        }
        public static bool GetTOneLicensed(string path)
        {
            lock (typeof(NHibernateHelperModule))
            {
                if (!_TOneLicensed.HasValue)
                {
                    try
                    {
                        if (ActiveLicense.Equals(_dl))
                        {
                            ExpiryDate = new DateTime(2022, 12, 31);
                            _TOneLicensed = true;
                            return _TOneLicensed.Value;
                        }

                        Match match = LicenseParserRegEx.Match(ActiveLicense);
                        if (match.Success)
                        {
                            EncodedLicenseKey = match.Groups["Key"].Value;
                            string encodedDates = match.Groups["Dates"].Value;
                            // even if the systemkeymatch is true, the dates must respect licencedate 
                            string decodedDates = WebHelperLibrary.Utility.SimpleDecode(encodedDates);
                            DateTime validFromDate = DateTime.MinValue, validToDate = DateTime.MinValue, today = DateTime.Today;
                            WebHelperLibrary.Utility.GetLicenceDates(decodedDates, out validFromDate, out validToDate);
                            ExpiryDate = validToDate;
                            bool isDateValid = today >= validFromDate && today <= validToDate;
                            _TOneLicensed = isDateValid && EncodedLicenseKey.Equals(GetToneSystemKeyMatch(path));

                            // thread to check licence 
                            System.Threading.Thread LicenceCheckRenewalThread = new System.Threading.Thread(new System.Threading.ThreadStart(SendNotification));
                            LicenceCheckRenewalThread.Start();
                        }
                        else
                            _TOneLicensed = false;
                    }
                    catch
                    {
                        _TOneLicensed = false;
                    }
                }
            }
            return !_TOneLicensed.HasValue ? GetTOneLicensed1(path) : _TOneLicensed.Value;
        }
        //In case the Background worker make the value _TOneLicensed=null while the GetTOneLicensed is running
        private static bool GetTOneLicensed1(string path)
        {
            return GetTOneLicensed(path);
        }
        #region IHttpModule Members

        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(NHibernateHelperModule));

        public NHibernateHelperModule()
        {
            log.Info("NHibernate Helper Module Initialized");
        }

        public void Dispose()
        {
            log.Info("NHibernate Helper Module Disposed");
        }

        public void Init(System.Web.HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            try
            {
                if (BeginRequest != null)
                    BeginRequest(sender, e);
            }
            catch
            {

            }
            // No valid license
            if (!Licensed)
            {
                System.Web.HttpContext.Current.Response.Write(
                        string.Format(@"
                            <html>
                                <head><title>Invalid T.One License</title></head>
                                <body>
                                    <b>Invalid License</b>.<br/>
                                    Please install a license for your T.One Application Server.<br/>
                                    You need to provide the following:<br/>
                                    <b>{0}</b>
                                </body>
                            </html>"
                            , GetSystemKey())
                        );
                System.Web.HttpContext.Current.Response.End();
            }
            else
            // Licensed
            {
                if (System.Web.HttpContext.Current.Request.RawUrl.Contains("/-Console-/"))
                {
                    Console.Handle(System.Web.HttpContext.Current.Request, System.Web.HttpContext.Current.Response);
                }
            }
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            try
            {
                if (EndRequest != null)
                    EndRequest(sender, e);
            }
            catch
            {

            }
            //newly added
            //System.Web.HttpContext context = System.Web.HttpContext.Current;
            //NHibernate.ISession currentSession = context.Items[DataConfiguration.CurrentSessionKey] as NHibernate.ISession;

            //if (currentSession != null)
            //{
            //    try
            //    {
            //        currentSession.Clear();
            //        currentSession.Close();
            //        currentSession.Dispose();
            //        currentSession = null;

            //        GC.Collect();
            //    }
            //    catch (Exception ex){log.Error("Error Disposing Session", ex);}
            //    context.Items.Remove(DataConfiguration.CurrentSessionKey);
            //}
            //end newly added

            NHibernate.ISession CurrentSession = TABS.Components.HybridWebSessionContext.Unbind(TABS.DataConfiguration.Default.SessionFactory);
            if (CurrentSession != null)
            {
                if (CurrentSession.IsOpen)
                {
                    CurrentSession.Clear();
                    CurrentSession.Close();
                }
                CurrentSession.Dispose();
                CurrentSession = null;
                GC.Collect();
            }
            return;
            System.Threading.Thread currentThread = System.Threading.Thread.CurrentThread;
            if (DataConfiguration.ThreadSessions.ContainsKey(currentThread))
            {
                NHibernate.ISession currentSession = DataConfiguration.ThreadSessions[currentThread];
                try
                {
                    // if (currentSession.IsDirty()) currentSession.Flush();
                    if (currentSession.IsOpen)
                    {
                        currentSession.Clear();
                        currentSession.Close();
                    }
                    currentSession.Dispose();
                    currentSession = null;
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    log.Error("Error Disposing Session", ex);
                }
                DataConfiguration.ThreadSessions.Remove(currentThread);
            }
        }

        #endregion

        public static string GetToneSystemKey(string path)
        {
            string id = string.Empty;

            //id = System.Web.HttpContext.Current.Server.MachineName;

            //id += "\r\n" + System.Web.HttpContext.Current.Server.MapPath("~/");
            id = path;
            System.Net.NetworkInformation.NetworkInterface[] interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

            foreach (System.Net.NetworkInformation.NetworkInterface netInterface in interfaces)
            {
                string typeName = netInterface.NetworkInterfaceType.ToString().ToLower();
                if (typeName.Contains("ethernet") || typeName.Contains("wireless"))
                    id += "\r\n" + netInterface.NetworkInterfaceType + " :: " + netInterface.GetPhysicalAddress().ToString();
            }

            id = SecurityEssentials.PasswordEncryption.Encode(id);

            return id;
        }

        public static string GetToneSystemKeyMatch(string path)
        {
            return GetSystemKeyMatch(GetToneSystemKey(path));
        }

        public static string GetSystemKey()
        {
            string id = string.Empty;

            id = System.Web.HttpContext.Current.Server.MachineName;

            id += "\r\n" + System.Web.HttpContext.Current.Server.MapPath("~/");

            System.Net.NetworkInformation.NetworkInterface[] interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

            foreach (System.Net.NetworkInformation.NetworkInterface netInterface in interfaces)
            {
                string typeName = netInterface.NetworkInterfaceType.ToString().ToLower();
                if (typeName.Contains("ethernet") || typeName.Contains("wireless"))
                    id += "\r\n" + netInterface.NetworkInterfaceType + " :: " + netInterface.GetPhysicalAddress().ToString();
            }

            id = SecurityEssentials.PasswordEncryption.Encode(id);

            return id;
        }

        public static string GetSystemKeyMatch(string systemKey)
        {
            return SecurityEssentials.PasswordEncryption.Encode("TABS:" + systemKey + ":TABS");
        }

        public static string GetSystemKeyMatch()
        {
            return GetSystemKeyMatch(GetSystemKey());
        }
    }

    public class Interceptor : NHibernate.EmptyInterceptor
    {
        public override object Instantiate(string clazz, NHibernate.EntityMode entityMode, object id)
        {
            return base.Instantiate(clazz, entityMode, id);
        }
        //public override object Instantiate(Type clazz, object id)
        //{
        //    if (clazz == typeof(TABS.CarrierAccount) && TABS.CarrierAccount._All != null)
        //    {
        //        TABS.CarrierAccount account = null;
        //        if (TABS.CarrierAccount.All.TryGetValue(id.ToString(), out account))
        //            return account;
        //    }
        //    else if (clazz == typeof(TABS.CarrierProfile) && TABS.CarrierProfile._All != null)
        //    {
        //        TABS.CarrierProfile profile = null;
        //        if (TABS.CarrierProfile.All.TryGetValue((int)id, out profile))
        //            return profile;
        //    }
        //    else if (clazz == typeof(TABS.Switch) && TABS.Switch._All != null)
        //    {
        //        TABS.Switch zwitch = null;
        //        if (TABS.Switch.All.TryGetValue((int)id, out zwitch))
        //            return zwitch;
        //    }
        //    else if (clazz == typeof(TABS.Currency) && TABS.Currency._All != null)
        //    {
        //        TABS.Currency currency = null;
        //        if (TABS.Currency.All.TryGetValue(id.ToString(), out currency))
        //            return currency;
        //    }
        //    else if (clazz == typeof(TABS.CodeGroup) && TABS.CodeGroup._All != null)
        //    {
        //        TABS.CodeGroup codeGroup = null;
        //        if (TABS.CodeGroup.All.TryGetValue(id.ToString(), out codeGroup))
        //            return codeGroup;
        //    }
        //    return base.Instantiate(clazz, id);
        //}
    }
}
