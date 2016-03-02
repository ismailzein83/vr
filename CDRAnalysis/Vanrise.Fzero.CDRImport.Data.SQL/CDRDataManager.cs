using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data.SQL
{
    public class CDRDataManager : BaseSQLDataManager, ICDRDataManager
    {
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static CDRDataManager()
        {
            _columnMapper.Add("CallClassName", "CallClassID");
            _columnMapper.Add("CallTypeName", "CallTypeID");
            _columnMapper.Add("SubscriberTypeName", "SubscriberTypeID");
            _columnMapper.Add("CellId", "Cell");
            _columnMapper.Add("UpVolume", "UpVolume");
            _columnMapper.Add("DownVolume", "DownVolume");
            _columnMapper.Add("ServiceType", "ServiceTypeID");
            _columnMapper.Add("ServiceVASName", "ServiceVASName");
        }

        public CDRDataManager()
            : base("CDRDBConnectionString")
        {
        }

        public void SaveCDRsToDB(List<CDR> cdrs)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (CDR cdr in cdrs)
            {
                WriteRecordToStream(cdr, dbApplyStream);
            }

            ApplyCDRsToDB(FinishDBApplyStream(dbApplyStream));
        }

        public object InitialiazeStreamForDBApply()
        {
            return new CDRDBApplyStream { PartitionedStreamsByDBFromTime = new ConcurrentDictionary<DateTime, PartitionedCDRStream>() };
        }

        public void WriteRecordToStream(CDR record, object dbApplyStream)
        {
            CDRDBApplyStream cdrDBApplyStream = dbApplyStream as CDRDBApplyStream;
            var dbFromTime = PartitionedCDRDataManager.GetDBFromTime(record.ConnectDateTime);
            PartitionedCDRStream matchStream;
            if(!cdrDBApplyStream.PartitionedStreamsByDBFromTime.TryGetValue(dbFromTime, out matchStream))
            {
                PartitionedCDRStream newStream = new PartitionedCDRStream { DataManager = PartitionedCDRDataManagerFactory.GetCDRDataManager<PartitionedCDRDataManager>(dbFromTime, false) };
                newStream.DBApplyStream = newStream.DataManager.InitialiazeStreamForDBApply();
                if (cdrDBApplyStream.PartitionedStreamsByDBFromTime.TryAdd(dbFromTime, newStream))
                    matchStream = newStream;
                else
                {
                    newStream.DataManager.FinishDBApplyStream(newStream.DBApplyStream);
                    matchStream = cdrDBApplyStream.PartitionedStreamsByDBFromTime[dbFromTime];
                }
            }
            matchStream.DataManager.WriteRecordToStream(record, matchStream.DBApplyStream);

        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            CDRDBApplyStream cdrDBApplyStream = dbApplyStream as CDRDBApplyStream;
            foreach (var entry in cdrDBApplyStream.PartitionedStreamsByDBFromTime.Values)
            {
                entry.DataManager.FinishDBApplyStream(entry.DBApplyStream);
            }
            return cdrDBApplyStream;
        }

        public void ApplyCDRsToDB(object preparedCDRs)
        {
            CDRDBApplyStream cdrDBApplyStream = preparedCDRs as CDRDBApplyStream;
            foreach(var stream in cdrDBApplyStream.PartitionedStreamsByDBFromTime.Values)
            {
                stream.DataManager.ApplyCDRsToDB(stream.DBApplyStream);
            };
        }

        public void LoadCDR(string numberPrefix, DateTime from, DateTime to, int? batchSize, Action<CDR> onCDRReady)
        {
            var dbTimeRanges = PartitionedCDRDataManager.GetDBTimeRanges(from, to);
            foreach (var dbTimeRange in dbTimeRanges)
            {
                var dataManager = PartitionedCDRDataManagerFactory.GetCDRDataManager<PartitionedCDRDataManager>(dbTimeRange.FromTime, true);
                if (dataManager != null)
                {
                    dataManager.LoadCDR(dbTimeRange.FromTime, numberPrefix, onCDRReady);
                }
            }
        }


        public BigResult<CDR> GetCDRs(Vanrise.Entities.DataRetrievalInput<CDRQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                string queryTempTableNotExists = string.Format(@"IF NOT OBJECT_ID('{0}', N'U') IS NOT NULL
	                                                        BEGIN
                                                                SELECT 1
                                                            END 
                                                            ELSE
                                                            BEGIN
                                                                SELECT 0
                                                            END", tempTableName);
                if(Convert.ToBoolean(ExecuteScalarText(queryTempTableNotExists, null)))
                {
                    var dbTimeRanges = PartitionedCDRDataManager.GetDBTimeRanges(input.Query.FromDate, input.Query.ToDate);
                    if(dbTimeRanges != null)
                    {
                        ExecuteNonQueryText(String.Format(PartitionedCDRDataManager.CDR_CREATETABLE_QUERYTEMPLATE, tempTableName), null);
                        foreach(var dbTimeRange in dbTimeRanges)
                        {
                            var dataManager = PartitionedCDRDataManagerFactory.GetCDRDataManager<PartitionedCDRDataManager>(dbTimeRange.FromTime, true);
                            if (dataManager != null)
                            {
                                dataManager.InsertCDRsByMSISDNToTempTable(tempTableName, input.Query.MSISDN, dbTimeRange.FromTime, dbTimeRange.ToTime);
                            }
                        }
                    }
                }
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, (new PartitionedCDRDataManager()).CDRMapper, _columnMapper);
        }

        #region Private Classes

        private class PartitionedCDRStream
        {
            public PartitionedCDRDataManager DataManager { get; set; }

            public Object DBApplyStream { get; set; }
        }

        private class CDRDBApplyStream
        {
            public ConcurrentDictionary<DateTime, PartitionedCDRStream> PartitionedStreamsByDBFromTime { get; set; }
        }

        #endregion


        public IEnumerable<string> GetNumberPrefixes(DateTime fromTime, DateTime toTime)
        {
            CDRDatabaseDataManager cdrDatabaseDataManager = new CDRDatabaseDataManager();
            var databases = cdrDatabaseDataManager.GetReadyDatabases(fromTime, toTime);
            if (databases != null)
            {
                HashSet<string> prefixes = new HashSet<string>();
                foreach (var db in databases)
                {
                    if (db.Settings == null)
                        throw new NullReferenceException("db.Settings");
                    if (db.Settings == null)
                        throw new NullReferenceException("db.Settings.CDRNumberPrefixes");
                    foreach (var prefix in db.Settings.CDRNumberPrefixes)
                    {
                        prefixes.Add(prefix);
                    }
                }
                return prefixes;
            }
            else
                return null;
        }
    }
}
