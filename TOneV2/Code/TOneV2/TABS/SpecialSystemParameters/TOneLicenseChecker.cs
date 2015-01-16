using System;
using System.Collections.Generic;

namespace TABS.SpecialSystemParameters
{
    public class TOneLicenseChecker : BaseXmlDetails
    {
        internal static log4net.ILog log = log4net.LogManager.GetLogger(typeof(TOneLicenseChecker));

        public TABS.LicenseType Type
        {
            get { return (LicenseType)Enum.Parse(typeof(LicenseType), Get("type")); }
            set { Set("type", value.ToString()); }
        } 
        public string Emails
        {
            get { return Get("emails"); }
            set { Set("emails", value); }
        }
        public string FromEmails
        {

            get { return Get("fromEmails"); }
            set { Set("fromEmails", value); }
        }

        public DateTime? LicenseBeginDate
        {
            get { return Get("beginDate") == string.Empty ? (DateTime?)null : DateTime.ParseExact(Get("beginDate"), "yyyy-MM-dd HH:mm:ss", null); }
            set { Set("beginDate", value.Value.ToString("yyyy-MM-dd HH:mm:ss")); }
        }

        public DateTime? LicenseEndDate
        {
            get { return Get("endDate") == string.Empty ? (DateTime?)null : DateTime.ParseExact(Get("endDate"), "yyyy-MM-dd HH:mm:ss", null); }
            set { Set("endDate", value.Value.ToString("yyyy-MM-dd HH:mm:ss")); }
        }

        public int? WarningPeriod
        {
            get { return Get("warningPeriod") == string.Empty ? (int?)null : int.Parse(Get("warningPeriod")); }
            set { Set("warningPeriod", value.Value.ToString()); }
        }

        public long? Minutes
        {
            get { return Get("minutes") == string.Empty ? (long?)null : long.Parse(Get("minutes")); }
            set { Set("minutes", value.Value.ToString()); }
        }
        public double? Margin
        {
            get { return Get("margin") == string.Empty ? (double?)null : double.Parse(Get("margin")); }
            set { Set("margin", value.Value.ToString()); }
        }
        public double? CurrentCDRSum
        {
            get { return Get("currentCDRSum") == string.Empty ? (double?)null : double.Parse(Get("currentCDRSum")); }
            set { Set("currentCDRSum", value.Value.ToString()); }
        }
        public long? CurrentCDRID
        {
            get { return Get("currentCDRID") == string.Empty ? (long?)null : long.Parse(Get("currentCDRID")); }
            set { Set("currentCDRID", value.Value.ToString()); }
        }

        public string Warning
        {
            get { return Get("warning"); }
            set { Set("warning", value); }
        }
        public string MessageBody
        {
            get { return Get("messagebody"); }
            set { Set("messagebody", value); }
        }

        public static List<TOneLicenseChecker> Get(SystemParameter parameter) { return BaseXmlDetails.Get<TOneLicenseChecker>(parameter); }

        public static Exception Save(SystemParameter parameter, List<TOneLicenseChecker> details) { return BaseXmlDetails.Save(parameter, details, SystemParameter.DefaultXml); }

        public static string defaultXml
        {
            get
            {
                return string.Format(@"<?xml version=""1.0"" encoding=""utf-8""?>
                                        <SystemParameter>
                                          <TOneLicenseChecker>
                                              <type>{0}</type> 
                                              <beginDate>{1}</beginDate> 
                                              <emails>{2}</emails> 
                                              <endDate>{3}</endDate> 
                                              <warningPeriod>{4}</warningPeriod> 
                                              <minutes>{5}</minutes> 
                                              <margin>{6}</margin> 
                                              <currentCDRSum>{7}</currentCDRSum>
                                              <currentCDRID>{8}</currentCDRID>
                                              <warning>{9}</warning>
                                              <messagebody>{10}</messagebody>
                                              <fromEmails>{11}</fromEmails>
                                          </TOneLicenseChecker>
                                        </SystemParameter>",
                                                           0
                                                           , DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss")
                                                           , "Support@vanrise.com"
                                                           , DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss")
                                                           , 0
                                                           , 0
                                                           , 0
                                                           , 0
                                                           , 0
                                                           , "not set"
                                                           , "not set"
                                                           , "not set");
            }
        }

        private double? GetMinutesAccumulation()
        {
            double? newSumValue = 0;
            long? newCDRID = this.CurrentCDRID;
            var query = string.Format("SELECT SUM(c.DurationInSeconds)/60.0 AS DurationSum ,MAX(c.CDRID) AS LastCDRId FROM CDR c WITH(NOLOCK) WHERE  c.CDRID > {0}", this.CurrentCDRID);
            System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(query, (System.Data.SqlClient.SqlConnection)DataHelper.GetOpenConnection());
            command.CommandTimeout = 0;
            using (System.Data.SqlClient.SqlDataReader read = command.ExecuteReader())
            {
                while (read.Read())
                {
                    if ((!(read["DurationSum"] is DBNull)) && (!(read["LastCDRId"] is DBNull)))
                    {
                        newSumValue = Convert.ToDouble(read["DurationSum"]);
                        newCDRID = Convert.ToInt64(read["LastCDRId"]);
                    }
                }
            }
            this.CurrentCDRID = newCDRID;
            TABS.SpecialSystemParameters.TOneLicenseChecker.Save(TABS.SystemParameter.TOneLicenseCheckerParameter, new List<TABS.SpecialSystemParameters.TOneLicenseChecker>() { this });
            return newSumValue;
        }

        public bool CheckLicense()
        {
            return this.Type == LicenseType.SLA ? CheckSLALicense() : CheckVolumeLicence();
        }

        private bool CheckVolumeLicence()
        {
            this.CurrentCDRSum += GetMinutesAccumulation();
            var percentageLeft = (1 - this.CurrentCDRSum / this.Minutes) * 100;
            if (percentageLeft <= this.Margin)
            {
                Warning = string.Format(Warning, percentageLeft);
                return false;
            }
            TABS.SpecialSystemParameters.TOneLicenseChecker.Save(TABS.SystemParameter.TOneLicenseCheckerParameter, new List<TABS.SpecialSystemParameters.TOneLicenseChecker>() { this });
            return true;
        }
        private bool CheckSLALicense()
        {
            TimeSpan TimeSpanLeft = this.LicenseEndDate.Value.Subtract(DateTime.Now);
            int? DaysLeft = (int?)TimeSpanLeft.TotalDays;
            if ((DaysLeft <= this.WarningPeriod))
            {
                Warning = string.Format(Warning, DaysLeft);
                return false;
            }
            return true;
        }

        public void CreateLicense(System.Collections.Generic.List<TABS.SpecialSystemParameters.TOneLicenseChecker> license, DateTime _LicenseBeginDate, DateTime _LicenseEndDate, int _WarningPeriod, string _Emails, string _Warning, string _MessageBody, string _FromEmail)
        {
            this.Type = TABS.LicenseType.SLA;
            this.LicenseEndDate = _LicenseEndDate;
            this.LicenseBeginDate = _LicenseBeginDate;
            this.Emails = _Emails;
            this.WarningPeriod = _WarningPeriod;
            this.Warning = _Warning;
            this.MessageBody = _MessageBody;
            this.FromEmails = _FromEmail;
            TABS.SpecialSystemParameters.TOneLicenseChecker.Save(TABS.SystemParameter.TOneLicenseCheckerParameter, license);
        }

        public void CreateLicense(System.Collections.Generic.List<TABS.SpecialSystemParameters.TOneLicenseChecker> license, DateTime _BeginDate, long _minutes, double _percentage, string _emails, long CDRID, string _Warning, string _MessageBody, string _FromEmail)
        {
            this.Type = TABS.LicenseType.Volume;
            this.Emails = _emails;
            this.LicenseBeginDate = _BeginDate;
            this.Minutes = _minutes;
            this.Margin = _percentage;
            this.CurrentCDRID = CDRID;
            this.Warning = _Warning;
            this.MessageBody = _MessageBody;
            this.FromEmails = _FromEmail;
            TABS.SpecialSystemParameters.TOneLicenseChecker.Save(TABS.SystemParameter.TOneLicenseCheckerParameter, license);
        }
    }
}
