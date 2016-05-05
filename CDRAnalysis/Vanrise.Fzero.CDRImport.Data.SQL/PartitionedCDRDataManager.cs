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

        protected CDRDatabaseSettings _databaseSettings;
        internal CDRDatabaseSettings DatabaseSettings
        {
            set
            {
                _databaseSettings = value;
            }
        }
        public PartitionedCDRDataManager()
            : base("FraudAnalysis_CDRConnStringTemplateKey")
        {

        }

        internal static DateTime GetDBFromTime(DateTime cdrTime)
        {
            return cdrTime.Date;
        }

        internal static IEnumerable<CDRDBTimeRange> GetDBTimeRanges(DateTime fromTime, DateTime toTime)
        {
            if (toTime == default(DateTime))
                toTime = DateTime.Today.AddDays(1);
            List<CDRDBTimeRange> dbTimeRanges = new List<CDRDBTimeRange>();
            for (DateTime date = fromTime.Date; date < toTime; date = date.AddDays(1))
            {
                DateTime rangeStart = date;
                if (rangeStart < fromTime)//could only happens in FIRST iteration
                    rangeStart = fromTime;

                DateTime rangeEnd = date.AddDays(1);
                if (rangeEnd > toTime)//could only happens in LAST iteration
                    rangeEnd = toTime;

                dbTimeRanges.Add(new CDRDBTimeRange
                {
                    FromTime = rangeStart,
                    ToTime = rangeEnd
                });
            }
            return dbTimeRanges;
        }

        protected string GetCDRTableName(DateTime cdrTime, string number, bool createIfNotExists)
        {
            int prefixLength = this._databaseSettings.PrefixLength;
            string numberPrefix;
            if (number.Length == prefixLength)
                numberPrefix = number;
            else if (number.Length > prefixLength)
                numberPrefix = number.Substring(0, prefixLength);
            else
                numberPrefix = number.PadRight(prefixLength, '_');

            if (this._databaseSettings.CDRNumberPrefixes.Contains(numberPrefix))
                return BuildCDRTableName(numberPrefix);
            else if (createIfNotExists)
            {
                DateTime dbFromTime = GetDBFromTime(cdrTime);
                CDRDatabaseDataManager cdrDatabaseDataManager = new CDRDatabaseDataManager();
                CDRDatabaseInfo dataBaseInfo = cdrDatabaseDataManager.GetWithLock(dbFromTime);
                if (dataBaseInfo == null)
                    throw new NullReferenceException("dataBaseInfo");
                string cdrTableName = BuildCDRTableName(numberPrefix);
                if (!dataBaseInfo.Settings.CDRNumberPrefixes.Contains(numberPrefix))
                {
                    ExecuteNonQueryText(String.Format(CDR_CREATETABLE_QUERYTEMPLATE, cdrTableName), null);
                    ExecuteNonQueryText(String.Format(CDR_CREATEINDEXES_QUERYTEMPLATE, cdrTableName), null);
                    dataBaseInfo.Settings.CDRNumberPrefixes.Add(numberPrefix);
                    cdrDatabaseDataManager.UpdateSettingsAndUnlock(dbFromTime, dataBaseInfo.Settings);
                }
                else
                {
                    cdrDatabaseDataManager.Unlock(dbFromTime);
                }
                cdrDatabaseDataManager.SetCacheExpired();
                this._databaseSettings = dataBaseInfo.Settings;
                return cdrTableName;
            }
            else
                return null;
        }

        private string BuildCDRTableName(string numberPrefix)
        {
            return string.Format("NormalCDR_{0}", numberPrefix);
        }

        protected string GetCDRTableName(DateTime cdrTime)
        {
            throw new NotSupportedException();
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
            CDRDatabaseDataManager cdrDatabaseDataManager = new CDRDatabaseDataManager();
            var lastDatabase = cdrDatabaseDataManager.GetLastReadyDatabase();
            if (lastDatabase != null && lastDatabase.Settings.PrefixLength == _databaseSettings.PrefixLength)
            {
                foreach (var prefix in lastDatabase.Settings.CDRNumberPrefixes)
                {
                    string cdrTableName = BuildCDRTableName(prefix);
                    ExecuteNonQueryText(String.Format(CDR_CREATETABLE_QUERYTEMPLATE, cdrTableName), null);
                    ExecuteNonQueryText(String.Format(CDR_CREATEINDEXES_QUERYTEMPLATE, cdrTableName), null);
                    this._databaseSettings.CDRNumberPrefixes.Add(prefix);
                }
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
            string tableName = GetCDRTableName(record.ConnectDateTime, record.MSISDN, true);
            StreamForBulkInsert matchStream;
            if (!allStreams.StreamsByTableNames.TryGetValue(tableName, out matchStream))
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
            foreach (var entry in allStreams.StreamsByTableNames)
            {
                entry.Value.Close();
            }
            return allStreams;
        }

        public void ApplyCDRsToDB(object preparedCDRs)
        {
            PartitionedCDRDBApplyStream allStreams = preparedCDRs as PartitionedCDRDBApplyStream;
            foreach (var entry in allStreams.StreamsByTableNames)
            {
                InsertBulkToTable(new StreamBulkInsertInfo
                {
                    TableName = entry.Key,
                    ColumnNames = s_cdrColumns,
                    Stream = entry.Value,
                    TabLock = true,
                    KeepIdentity = false,
                    FieldSeparator = '^'
                });
            }
        }

        public void LoadCDR(DateTime fromTime, DateTime toTime, string numberPrefix, Action<CDR> onCDRReady)
        {
            string cdrTableName = GetCDRTableName(fromTime, numberPrefix, false);
            if (cdrTableName == null)
                return;
            string query = String.Format(@"SELECT {0} FROM {1} WITH(NOLOCK) 
                                            WHERE MSISDN LIKE '{2}%' AND [ConnectDateTime] >= @FromTime AND [ConnectDateTime] < @ToTime 
                                            ORDER BY MSISDN", CDR_COLUMNS, cdrTableName, numberPrefix);
            ExecuteReaderText(query,
                (reader) =>
                {
                    while (reader.Read())
                    {
                        onCDRReady(CDRMapper(reader));
                    }
                },
            (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromTime", fromTime));
                cmd.Parameters.Add(new SqlParameter("@ToTime", toTime));
            });
        }

        public void InsertCDRsByMSISDNToTempTable(string tempTableName, string msisdn, DateTime fromTime, DateTime toTime)
        {
            string tableName = GetCDRTableName(fromTime, msisdn, false);
            if (tableName != null)
            {
                StringBuilder queryBuilder = new StringBuilder();
                string query = String.Format(@"INSERT INTO {0}
                                        SELECT {1} 
                                        FROM {2} WITH(NOLOCK)
                                        WHERE [MSISDN] = @MSISDN AND [ConnectDateTime] >= @FromTime AND [ConnectDateTime] < @ToTime"
                    , tempTableName, CDR_COLUMNS, tableName);

                ExecuteNonQueryText(query, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@MSISDN", msisdn));
                    cmd.Parameters.Add(new SqlParameter("@FromTime", fromTime));
                    cmd.Parameters.Add(new SqlParameter("@ToTime", toTime));
                });
            }

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
            cdr.SubscriberType = reader["SubscriberTypeID"] != DBNull.Value ? (SubscriberType)reader["SubscriberTypeID"] : default(SubscriberType?);
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
