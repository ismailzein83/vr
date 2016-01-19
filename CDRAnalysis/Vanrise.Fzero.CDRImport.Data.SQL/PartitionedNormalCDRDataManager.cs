using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data.SQL
{
    internal class PartitionedNormalCDRDataManager : PartitionedCDRDataManager, IBulkApplyDataManager<CDR>
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

        public object InitialiazeStreamForDBApply()
        {
            return new PartitionedNormalCDRDBApplyStream { StreamsByTableNames = new ConcurrentDictionary<string, StreamForBulkInsert>() };
        }

        public void WriteRecordToStream(CDR record, object dbApplyStream)
        {
            PartitionedNormalCDRDBApplyStream allStreams = dbApplyStream as PartitionedNormalCDRDBApplyStream;
            string tableName = GetNormalCDRTableName(record.ConnectDateTime);
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
            PartitionedNormalCDRDBApplyStream allStreams = dbApplyStream as PartitionedNormalCDRDBApplyStream;
            foreach(var entry in allStreams.StreamsByTableNames)
            {
                entry.Value.Close();
            }
            return allStreams;
        }

        public void ApplyCDRsToDB(object preparedCDRs)
        {
            PartitionedNormalCDRDBApplyStream allStreams = preparedCDRs as PartitionedNormalCDRDBApplyStream;
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
            //string filter = !String.IsNullOrEmpty(numberPrefix) ? string.Format("WHERE LEFT(MSISDN, {0}) LIKE '%{1}'", (9 + numberPrefix.Length), numberPrefix) : null;
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
            //!String.IsNullOrEmpty(numberPrefix) ? string.Format("WHERE MSISDN LIKE '241820{0}%' OR MSISDN LIKE '241830{0}%'", numberPrefix) : null;

            string query = String.Format(QUERY_GETCDRS, GetNormalCDRTableName(fromTime), filter);
            ExecuteReaderText(query, (reader) =>
            {
                while (reader.Read())
                {
                    onCDRReady(NormalCDRMapper(reader));
                }
            }, null);
        }

        #region Private Methods

        internal CDR NormalCDRMapper(IDataReader reader)
        {
            var normalCDR = new CDR();
            normalCDR.CallType = GetReaderValue<CallType>(reader, "CallTypeID");
            normalCDR.BTS = reader["BTS"] as string;
            normalCDR.ConnectDateTime = (DateTime)reader["ConnectDateTime"];
            normalCDR.IMSI = reader["IMSI"] as string;
            normalCDR.DurationInSeconds = GetReaderValue<decimal>(reader, "DurationInSeconds");
            normalCDR.DisconnectDateTime = GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
            normalCDR.CallClassId = GetReaderValue<int?>(reader, "CallClassID");
            normalCDR.IsOnNet = GetReaderValue<bool>(reader, "IsOnNet");
            normalCDR.SubscriberType = GetReaderValue<SubscriberType?>(reader, "SubscriberTypeID");
            normalCDR.IMEI = reader["IMEI"] as string;
            normalCDR.Cell = reader["Cell"] as string;
            normalCDR.UpVolume = GetReaderValue<Decimal?>(reader, "UpVolume");
            normalCDR.DownVolume = GetReaderValue<Decimal?>(reader, "DownVolume");
            normalCDR.CellLatitude = GetReaderValue<Decimal?>(reader, "CellLatitude");
            normalCDR.CellLongitude = GetReaderValue<Decimal?>(reader, "CellLongitude");
            normalCDR.InTrunkId = GetReaderValue<int?>(reader, "InTrunkID");
            normalCDR.OutTrunkId = GetReaderValue<int?>(reader, "OutTrunkID");
            normalCDR.ServiceTypeId = GetReaderValue<int?>(reader, "ServiceTypeID");
            normalCDR.ServiceVASName = reader["ServiceVASName"] as string;
            normalCDR.ReleaseCode = reader["ReleaseCode"] as string;
            normalCDR.MSISDNAreaCode = reader["MSISDNAreaCode"] as string;
            normalCDR.DestinationAreaCode = reader["DestinationAreaCode"] as string;
            normalCDR.Destination = reader["Destination"] as string;
            normalCDR.MSISDN = reader["MSISDN"] as string;
            return normalCDR;
        }

        #endregion

        #region Constants

        const string QUERY_GETCDRS = @"SELECT  cdrs.[MSISDN] ,cdrs.[IMSI] ,cdrs.[ConnectDateTime] ,cdrs.[Destination] ,
		cdrs.[DurationInSeconds] ,cdrs.[DisconnectDateTime] ,cdrs.[CallClassID]  ,cdrs.[IsOnNet] ,
		cdrs.[CallTypeID] ,cdrs.[SubscriberTypeID] ,cdrs.[IMEI]
		,cdrs.[BTS]  ,cdrs.[Cell]  ,cdrs.[SwitchId]  ,cdrs.[UpVolume]  ,cdrs.[DownVolume] ,
		cdrs.[CellLatitude]  ,cdrs.[CellLongitude]  ,cdrs.[InTrunkID]  ,cdrs.[OutTrunkID]  ,cdrs.[ServiceTypeID]  ,cdrs.[ServiceVASName] 
		, cdrs.[ReleaseCode], cdrs.MSISDNAreaCode, cdrs.DestinationAreaCode
FROM {0} cdrs WITH(NOLOCK)
{1}
";

        #endregion

        #region Private Classes

        private class PartitionedNormalCDRDBApplyStream

        {
            public ConcurrentDictionary<string, StreamForBulkInsert> StreamsByTableNames { get; set; }
        }

        #endregion
    }
}
