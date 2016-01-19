using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Fzero.CDRImport.Data.SQL
{
    internal class CDRDBTimeRange
    {
        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }
    }
    public class PartitionedCDRDataManager : BaseSQLDataManager
    {
        static int s_MaxNumberOfReadThreads;
        public PartitionedCDRDataManager()
            : base("FraudAnalysis_CDRConnStringTemplateKey")
        {

        }

        static PartitionedCDRDataManager()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["FraudAnalysis_CDRReadMaxConcurrentRead"], out s_MaxNumberOfReadThreads))
                s_MaxNumberOfReadThreads = 1;
        }

        internal static int MaxNumberOfReadThreads
        {
            get
            {
                return s_MaxNumberOfReadThreads;
            }
        }

        internal static DateTime GetDBFromTime(DateTime cdrTime)
        {
            return cdrTime.Date;
        }

        internal static IEnumerable<CDRDBTimeRange> GetDBTimeRanges(DateTime fromTime, DateTime toTime)
        {
            List<CDRDBTimeRange> dbTimeRanges = new List<CDRDBTimeRange>();
            for (DateTime date = fromTime; date < toTime; date = date.AddHours(1))
            {
                DateTime dbToTime = date.AddHours(1);
                if(dbToTime > toTime)
                dbToTime = toTime;
                dbTimeRanges.Add(new CDRDBTimeRange
                {
                    FromTime = date,
                    ToTime = dbToTime
                });
            }
            return dbTimeRanges;
        }

        protected string GetNormalCDRTableName(DateTime cdrTime)
        {
            return string.Format("NormalCDR_{0:yyyyMMdd_HH}00", cdrTime);
        }

        internal string DatabaseName
        {
            set
            {
                _databaseName = value;
            }
        }

        string _databaseName;
        
        protected override string GetConnectionString()
        {
            if (_databaseName == null)
                throw new NullReferenceException("_databaseName");
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder(base.GetConnectionString());
            connStringBuilder.InitialCatalog = _databaseName;
            return connStringBuilder.ConnectionString;
        }


        internal void CreateDatabase(DateTime fromTime, out string databaseName, out DateTime toTime )
        {
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder(base.GetConnectionString());
            databaseName = String.Format(connStringBuilder.InitialCatalog, fromTime.ToString("yyyy_MM_dd"));
            this.DatabaseName = databaseName;
            Vanrise.Data.SQL.MasterDatabaseDataManager masterDatabaseManager = new MasterDatabaseDataManager(GetConnectionString());            
            masterDatabaseManager.DropDatabaseWithForceIfExists(databaseName);
            masterDatabaseManager.CreateDatabase(databaseName, ConfigurationManager.AppSettings["FraudAnalysis_CDRDBDataFileDirectory"], ConfigurationManager.AppSettings["FraudAnalysis_CDRDBLogFileDirectory"]);
            toTime = fromTime.AddDays(1);
            DateTime currentHour = fromTime;
            while(currentHour < toTime)
            {
                ExecuteNonQueryText(String.Format(NORMALCDR_CREATETABLE_QUERYTEMPLATE, GetNormalCDRTableName(currentHour)), null);
                currentHour = currentHour.AddHours(1);
            }

        }

        #region Constants

        const string NORMALCDR_CREATETABLE_QUERYTEMPLATE = @"CREATE TABLE [{0}](
	                                                            [MSISDN] [varchar](30) NOT NULL,
	                                                            [IMSI] [varchar](20) NULL,
	                                                            [ConnectDateTime] [datetime] NOT NULL,
	                                                            [Destination] [varchar](40) NULL,
	                                                            [DurationInSeconds] [numeric](13, 4) NOT NULL,
	                                                            [DisconnectDateTime] [datetime] NULL,
	                                                            [CallClassID] [int] NULL,
	                                                            [IsOnNet] [bit] NULL,
	                                                            [CallTypeID] [int] NOT NULL,
	                                                            [SubscriberTypeID] [int] NULL,
	                                                            [IMEI] [varchar](20) NULL,
	                                                            [BTS] [varchar](50) NULL,
	                                                            [Cell] [varchar](50) NULL,
	                                                            [SwitchID] [int] NULL,
	                                                            [UpVolume] [decimal](18, 2) NULL,
	                                                            [DownVolume] [decimal](18, 2) NULL,
	                                                            [CellLatitude] [decimal](18, 8) NULL,
	                                                            [CellLongitude] [decimal](18, 8) NULL,
	                                                            [ServiceTypeID] [int] NULL,
	                                                            [ServiceVASName] [varchar](50) NULL,
	                                                            [InTrunkID] [int] NULL,
	                                                            [OutTrunkID] [int] NULL,
	                                                            [ReleaseCode] [varchar](50) NULL,
	                                                            [MSISDNAreaCode] [varchar](10) NULL,
	                                                            [DestinationAreaCode] [varchar](10) NULL
                                                            ) ON [PRIMARY]

                                                            CREATE CLUSTERED INDEX [IX_{0}_MSISDN] ON [{0}] 
                                                            (
	                                                            [MSISDN] ASC
                                                            )";

        #endregion

    }
}
