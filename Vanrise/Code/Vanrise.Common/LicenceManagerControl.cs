using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Management;
namespace Vanrise.Common
{
    public class LicenceManagerControl
    {
        protected static bool? _Licensed = null;

        static readonly char[] _lk = new char[] { 'T', 'A', 'B', 'S', ' ', 'L', 'i', 'c', 'e', 'n', 's', 'e' };

        public static string ActiveLicense { get { return System.Configuration.ConfigurationSettings.AppSettings[new string(_lk)]; } }
        public static int LicenceNotificationDays { get { return int.Parse(System.Configuration.ConfigurationSettings.AppSettings["LicenceNotifierPeriod"]); } } // in days 
        public static DateTime ExpiryWarningDate { get { return ExpiryDate.Subtract(TimeSpan.FromDays(LicenceNotificationDays)); } }
        public static TimeSpan LicenceCheckFrequency { get { return TimeSpan.Parse(System.Configuration.ConfigurationSettings.AppSettings["LicenceCheckFrequency"].ToString()); } } // in days 
        private static string EncodedLicenseKey { get; set; }
        //static Regex LicenseParserRegEx = new Regex(@"(?<Key>\w\w(-\w\w){15}):(?<Plugins>[=,\w]+)*:(?<Dates>[=,\w]+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        static Regex LicenseParserRegEx = new Regex(@"(?<Key>\w\w(-\w\w){15}):(?<Plugins>[=,\w]+)*:(?<Dates>[=,\w]+):(?<KeyEn>[=,\w]+)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public static DateTime ExpiryDate { get; protected set; }

        public static bool CheckLicence()
        {

            //string TempLicense = GenerateLicense(GetSystemKey());
            var licenseConfig = System.Configuration.ConfigurationManager.AppSettings["Tabs License"];

            string license = licenseConfig == null ? null : licenseConfig.ToString();
            return IsServiceLicensed(license, GetSystemKey());//TABS.Components.NHibernateHelperModule.IsServiceLicensed(license, GetSystemKey());
        }


        public static bool CheckLicence(out string Key)
        {

            //string TempLicense = GenerateLicense(GetSystemKey());
            var licenseConfig = System.Configuration.ConfigurationManager.AppSettings["Tabs License"];

            string license = licenseConfig == null ? null : licenseConfig.ToString();
            Key = GetSystemKey();
            return IsServiceLicensed(license, Key);//TABS.Components.NHibernateHelperModule.IsServiceLicensed(license, GetSystemKey());
        }

        public static bool CheckLicence(string keyName, out string Key)
        {

            //string TempLicense = GenerateLicense(GetSystemKey());
            var licenseConfig = System.Configuration.ConfigurationManager.AppSettings[keyName];

            string license = licenseConfig == null ? null : licenseConfig.ToString();
            Key = GetSystemKey();
            return IsServiceLicensed(license, Key);//TABS.Components.NHibernateHelperModule.IsServiceLicensed(license, GetSystemKey());
        }

        public static string DecreptConnectionString(string OriginalConnectionString)
        {
            if (IsConnectionLicenceEncrypted())
                return SimpleDecode(OriginalConnectionString);
            return OriginalConnectionString;
        }

        public static string GetSystemKeyMatch()
        {
            return GetSystemKeyMatch(GetSystemKey());
        }

        public static string GetSystemKeyMatch(string systemKey)
        {
            return Encode("TABS:" + systemKey + ":TABS");
        }


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
                string encodedEncryptionValue = SimpleDecode(encodedEncryptionKey);
                IsEncrypted = encodedEncryptionValue == "Encrypted" ? true : false;
            }
            return IsEncrypted;
        }

        public static bool IsServiceLicensed(string ActiveLicense, string Key)
        {
            //lock (typeof(NHibernateHelperModule))
            //{
            try
            {
                Match match = LicenseParserRegEx.Match(ActiveLicense);
                if (match.Success)
                {
                    EncodedLicenseKey = match.Groups["Key"].Value;
                    string encodedDates = match.Groups["Dates"].Value;
                    // even if the systemkeymatch is true, the dates must respect licencedate 
                    string decodedDates = SimpleDecode(encodedDates);
                    DateTime validFromDate = DateTime.MinValue, validToDate = DateTime.MinValue, today = DateTime.Today;
                    GetLicenceDates(decodedDates, out validFromDate, out validToDate);
                    ExpiryDate = validToDate;
                    bool isDateValid = today >= validFromDate && today <= validToDate;
                    return isDateValid && EncodedLicenseKey.Equals(GetSystemKeyMatch(Key));

                    //// thread to check licence 
                    //System.Threading.Thread LicenceCheckRenewalThread = new System.Threading.Thread(new System.Threading.ThreadStart(SendNotification));
                    //LicenceCheckRenewalThread.Start();
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }

            //}

        }
        /// <summary>
        /// Encode a Text string (very simple encoding and does not depend on a key)
        /// </summary>
        public static string SimpleEncode(string text, bool insertLineBreaks)
        {
            byte[] utf8 = System.Text.Encoding.UTF8.GetBytes(text);
            char[] base64 = new char[utf8.Length * 3];
            int converted = System.Convert.ToBase64CharArray(utf8, 0, utf8.Length, base64, 0, insertLineBreaks ? Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None);
            return new string(base64.Take(converted).Reverse().ToArray());
        }
        /// Encode a Text string (very simple encoding and does not depend on a key)
        /// </summary>
        public static string SimpleEncode(string text)
        {
            return SimpleEncode(text, false);
        }

        /// <summary>
        /// Decode an simply encoded string 
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static string SimpleDecode(string encoded)
        {
            char[] base64 = encoded.ToCharArray().Reverse().ToArray();
            byte[] utf8 = System.Convert.FromBase64CharArray(base64, 0, base64.Length);
            return System.Text.Encoding.UTF8.GetString(utf8);
        }

        public static void GetLicenceDates(string datekey, out DateTime fromdate, out DateTime todate)
        {
            int FY = int.Parse(datekey.Substring(0, 4));
            int FM = int.Parse(datekey.Substring(4, 2));
            int FD = int.Parse(datekey.Substring(6, 2));
            int TY = int.Parse(datekey.Substring(8, 4));
            int TM = int.Parse(datekey.Substring(12, 2));
            int TD = int.Parse(datekey.Substring(14, 2));

            fromdate = new DateTime(FY, FM, FD);
            todate = new DateTime(TY, TM, TD);


        }


        public static string Encode(string password)
        {
            //Declarations
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            originalBytes = UTF8Encoding.Default.GetBytes(password);
            encodedBytes = md5.ComputeHash(originalBytes);

            //Convert encoded bytes back to a 'readable' string
            return BitConverter.ToString(encodedBytes);
        }
        public static string GetSystemKey()
        {
            string id = string.Empty;
            id = Environment.MachineName;
            id += "\r\n" + AppDomain.CurrentDomain.BaseDirectory;
            id += GetCPUSerialNumber();
            //System.Net.NetworkInformation.NetworkInterface[] interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

            //foreach (System.Net.NetworkInformation.NetworkInterface netInterface in interfaces)
            //{
            //    string typeName = netInterface.NetworkInterfaceType.ToString().ToLower();
            //    if (typeName.Contains("ethernet") || typeName.Contains("wireless"))
            //        id += "\r\n" + netInterface.NetworkInterfaceType + " :: " + netInterface.GetPhysicalAddress().ToString();
            //}

            id = Encode(id);

            return id;
        }
        private static String GetCPUSerialNumber()
        {
            String serial = String.Empty;
            var search = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystemProduct");
            var mobos = search.Get();

            foreach (var m in mobos)
            {
                serial = m["uuid"].ToString();
            }
            return serial;
        }
    }
}
