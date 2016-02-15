using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data.SQL
{
    internal class CDRDBTimeRange
    {
        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }
    }

    public class PartitionedCDRDataManager : BaseSQLDataManager, IBulkApplyDataManager<CDR>
    {
        static string[] s_cdrColumns = new string[] {
            "MSISDN"
          ,"IMSI"
          ,"ConnectDateTime"
          ,"Destination"
          ,"DurationInSeconds"
          ,"DisconnectDateTime"
          ,"CallClassID"
          ,"IsOnNet"
          ,"CallTypeID"
          ,"SubscriberTypeID"
          ,"IMEI"
          ,"BTS"
          ,"Cell"
          ,"SwitchID"
          ,"UpVolume"
          ,"DownVolume"
          ,"CellLatitude"
          ,"CellLongitude"
          ,"ServiceTypeID"
          ,"ServiceVASName"
          ,"InTrunkID"
          ,"OutTrunkID"
          ,"ReleaseCode"
          ,"MSISDNAreaCode"
          ,"DestinationAreaCode"
        };


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
                if (dbToTime > toTime)
                    dbToTime = toTime;
                dbTimeRanges.Add(new CDRDBTimeRange
                {
                    FromTime = date,
                    ToTime = dbToTime
                });
            }
            return dbTimeRanges;
        }

        protected string GetCDRTableName(DateTime cdrTime)
        {
            return string.Format("CDR_{0:yyyyMMdd_HH}00", cdrTime);
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


        internal void CreateDatabase(DateTime fromTime, out string databaseName, out DateTime toTime)
        {
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder(base.GetConnectionString());
            databaseName = String.Format(connStringBuilder.InitialCatalog, fromTime.ToString("yyyy_MM_dd"));
            this.DatabaseName = databaseName;
            Vanrise.Data.SQL.MasterDatabaseDataManager masterDatabaseManager = new MasterDatabaseDataManager(GetConnectionString());
            masterDatabaseManager.DropDatabaseWithForceIfExists(databaseName);
            masterDatabaseManager.CreateDatabase(databaseName, ConfigurationManager.AppSettings["FraudAnalysis_CDRDBDataFileDirectory"], ConfigurationManager.AppSettings["FraudAnalysis_CDRDBLogFileDirectory"]);
            toTime = fromTime.AddDays(1);
            DateTime currentHour = fromTime;
            while (currentHour < toTime)
            {
                string cdrTableName = GetCDRTableName(currentHour);
                ExecuteNonQueryText(String.Format(CDR_CREATETABLE_QUERYTEMPLATE, cdrTableName), null);
                ExecuteNonQueryText(String.Format(CDR_CREATEINDEXES_QUERYTEMPLATE, cdrTableName), null);
                currentHour = currentHour.AddHours(1);
            }

        }

        #region Constants

        internal const string CDR_CREATETABLE_QUERYTEMPLATE = @"CREATE TABLE {0} (
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
                                                            ) ON [PRIMARY]";

        private const string CDR_CREATEINDEXES_QUERYTEMPLATE = @"CREATE CLUSTERED INDEX [IX_{0}_MSISDN] ON [{0}] 
                                                                        (
	                                                                        [MSISDN] ASC
                                                                        )";

        #endregion


        public object InitialiazeStreamForDBApply()
        {
            return new PartitionedCDRDBApplyStream { StreamsByTableNames = new ConcurrentDictionary<string, StreamForBulkInsert>() };
        }

        public void WriteRecordToStream(CDR record, object dbApplyStream)
        {
            PartitionedCDRDBApplyStream allStreams = dbApplyStream as PartitionedCDRDBApplyStream;
            string tableName = GetCDRTableName(record.ConnectDateTime);
            StreamForBulkInsert matchStream;
            if(!allStreams.StreamsByTableNames.TryGetValue(tableName, out matchStream))
            {
                StreamForBulkInsert newStream = base.InitializeStreamForBulkInsert();
                if (allStreams.StreamsByTableNames.TryAdd(tableName, newStream))
                    matchStream = newStream;
                else
                {
                    newStream.Close();
                    matchStream = allStreams.StreamsByTableNames[tableName];
                }
            }
            matchStream.WriteRecord("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}^{8}^{9}^{10}^{11}^{12}^{13}^{14}^{15}^{16}^{17}^{18}^{19}^{20}^{21}^{22}^{23}^{24}",
                                     record.MSISDN
                                   , record.IMSI
                                   , record.ConnectDateTime
                                   , record.Destination
                                   , record.DurationInSeconds
                                   , record.DisconnectDateTime
                                   , record.CallClassId
                                   , record.IsOnNet ? 1 : 0
                                   , (int)record.CallType
                                   , (int?)record.SubscriberType
                                   , record.IMEI
                                   , record.BTS
                                   , record.Cell
                                   , record.SwitchId
                                   , record.UpVolume
                                   , record.DownVolume
                                   , record.CellLatitude
                                   , record.CellLongitude
                                   , record.ServiceTypeId
                                   , record.ServiceVASName
                                   , record.InTrunkId
                                   , record.OutTrunkId
                                   , record.ReleaseCode
                                   , record.MSISDNAreaCode
                                   , record.DestinationAreaCode
                                    );
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            PartitionedCDRDBApplyStream allStreams = dbApplyStream as PartitionedCDRDBApplyStream;
            foreach(var entry in allStreams.StreamsByTableNames)
            {
                entry.Value.Close();
            }
            return allStreams;
        }

        public void ApplyCDRsToDB(object preparedCDRs)
        {
            PartitionedCDRDBApplyStream allStreams = preparedCDRs as PartitionedCDRDBApplyStream;
            //foreach (var entry in allStreams.StreamsByTableNames)
            Parallel.ForEach(allStreams.StreamsByTableNames, (entry) =>
            {
                InsertBulkToTable(new StreamBulkInsertInfo
                {
                    TableName = entry.Key,
                    ColumnNames = s_cdrColumns,
                    Stream = entry.Value,
                    TabLock = false,
                    KeepIdentity = false,
                    FieldSeparator = '^'
                });
            });
        }

        public void LoadCDR(DateTime fromTime, IEnumerable<string> numberPrefixes, Action<CDR> onCDRReady)
        {
            string filter = null;
            if (numberPrefixes != null && numberPrefixes.Count() > 0)
            {
                StringBuilder filterBuilder = new StringBuilder();
                foreach(var numberPrefix in numberPrefixes)
                {
                    if (filterBuilder.Length == 0)
                        filterBuilder.Append(" WHERE ");
                    else
                        filterBuilder.Append(" OR ");
                    filterBuilder.AppendFormat(" MSISDN LIKE '{0}%' ", numberPrefix);
                }
                filter = filterBuilder.ToString();
            } 

            string query = String.Format( @"SELECT {0} FROM {1} WITH(NOLOCK) {2}", CDR_COLUMNS, GetCDRTableName(fromTime), filter);
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    onCDRReady(CDRMapper(reader));
                }
            }, null);
        }

        public void InsertCDRsByMSISDNToTempTable(string tempTableName, string msisdn, DateTime fromTime, DateTime toTime)
        {
            StringBuilder queryBuilder = new StringBuilder();
            string query = String.Format(@"INSERT INTO {0}
                                        SELECT {1} 
                                        FROM {2} WITH(NOLOCK)
                                        WHERE [MSISDN] = @MSISDN", tempTableName, CDR_COLUMNS, GetCDRTableName(fromTime));

            ExecuteNonQueryText(query, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@MSISDN", msisdn));
                });
        }

        #region Private Methods

        internal CDR CDRMapper(IDataReader reader)
        {
            var cdr = new CDR();
            cdr.CallType = GetReaderValue<CallType>(reader, "CallTypeID");
            cdr.BTS = reader["BTS"] as string;
            cdr.ConnectDateTime = (DateTime)reader["ConnectDateTime"];
            cdr.IMSI = reader["IMSI"] as string;
            cdr.DurationInSeconds = GetReaderValue<decimal>(reader, "DurationInSeconds");
            cdr.DisconnectDateTime = GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
            cdr.CallClassId = GetReaderValue<int?>(reader, "CallClassID");
            cdr.IsOnNet = GetReaderValue<bool>(reader, "IsOnNet");
            cdr.SubscriberType = GetReaderValue<SubscriberType?>(reader, "SubscriberTypeID");
            cdr.IMEI = reader["IMEI"] as string;
            cdr.Cell = reader["Cell"] as string;
            cdr.UpVolume = GetReaderValue<Decimal?>(reader, "UpVolume");
            cdr.DownVolume = GetReaderValue<Decimal?>(reader, "DownVolume");
            cdr.CellLatitude = GetReaderValue<Decimal?>(reader, "CellLatitude");
            cdr.CellLongitude = GetReaderValue<Decimal?>(reader, "CellLongitude");
            cdr.InTrunkId = GetReaderValue<int?>(reader, "InTrunkID");
            cdr.OutTrunkId = GetReaderValue<int?>(reader, "OutTrunkID");
            cdr.ServiceTypeId = GetReaderValue<int?>(reader, "ServiceTypeID");
            cdr.ServiceVASName = reader["ServiceVASName"] as string;
            cdr.ReleaseCode = reader["ReleaseCode"] as string;
            cdr.MSISDNAreaCode = reader["MSISDNAreaCode"] as string;
            cdr.DestinationAreaCode = reader["DestinationAreaCode"] as string;
            cdr.Destination = reader["Destination"] as string;
            cdr.MSISDN = reader["MSISDN"] as string;
            return cdr;
        }

        #endregion

        #region Constants

        const string CDR_COLUMNS = @" [MSISDN] ,[IMSI] ,[ConnectDateTime] ,[Destination] ,
		[DurationInSeconds] ,[DisconnectDateTime] ,[CallClassID]  ,[IsOnNet] ,
		[CallTypeID] ,[SubscriberTypeID] ,[IMEI]
		,[BTS]  ,[Cell]  ,[SwitchId]  ,[UpVolume]  ,[DownVolume] ,
		[CellLatitude]  ,[CellLongitude]  ,[InTrunkID]  ,[OutTrunkID]  ,[ServiceTypeID]  ,[ServiceVASName] 
		, [ReleaseCode], MSISDNAreaCode, DestinationAreaCode";

        #endregion

        #region Private Classes

        private class PartitionedCDRDBApplyStream
        {
            public ConcurrentDictionary<string, StreamForBulkInsert> StreamsByTableNames { get; set; }
        }

        #endregion
    }
}
