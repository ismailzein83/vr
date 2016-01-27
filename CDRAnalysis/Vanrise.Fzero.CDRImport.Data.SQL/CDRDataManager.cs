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
            return new NormalCDRDBApplyStream { PartitionedStreamsByDBFromTime = new ConcurrentDictionary<DateTime, PartitionedNormalCDRStream>() };
        }

        public void WriteRecordToStream(CDR record, object dbApplyStream)
        {
            NormalCDRDBApplyStream normalCDRDBApplyStream = dbApplyStream as NormalCDRDBApplyStream;
            var dbFromTime = PartitionedCDRDataManager.GetDBFromTime(record.ConnectDateTime);
            PartitionedNormalCDRStream matchStream;
            if(!normalCDRDBApplyStream.PartitionedStreamsByDBFromTime.TryGetValue(dbFromTime, out matchStream))
            {
                PartitionedNormalCDRStream newStream = new PartitionedNormalCDRStream { DataManager = PartitionedCDRDataManagerFactory.GetCDRDataManager<PartitionedNormalCDRDataManager>(dbFromTime, false) };
                newStream.DBApplyStream = newStream.DataManager.InitialiazeStreamForDBApply();
                if (normalCDRDBApplyStream.PartitionedStreamsByDBFromTime.TryAdd(dbFromTime, newStream))
                    matchStream = newStream;
                else
                {
                    newStream.DataManager.FinishDBApplyStream(newStream.DBApplyStream);
                    matchStream = normalCDRDBApplyStream.PartitionedStreamsByDBFromTime[dbFromTime];
                }
            }
            matchStream.DataManager.WriteRecordToStream(record, matchStream.DBApplyStream);

        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            NormalCDRDBApplyStream normalCDRDBApplyStream = dbApplyStream as NormalCDRDBApplyStream;
            foreach (var entry in normalCDRDBApplyStream.PartitionedStreamsByDBFromTime.Values)
            {
                entry.DataManager.FinishDBApplyStream(entry.DBApplyStream);
            }
            return normalCDRDBApplyStream;
        }

        public void ApplyCDRsToDB(object preparedCDRs)
        {
            NormalCDRDBApplyStream normalCDRDBApplyStream = preparedCDRs as NormalCDRDBApplyStream;
            //foreach (var entry in normalCDRDBApplyStream.PartitionedStreamsByDBFromTime.Values)
            Parallel.ForEach(normalCDRDBApplyStream.PartitionedStreamsByDBFromTime.Values, (entry) =>
            {
                entry.DataManager.ApplyCDRsToDB(entry.DBApplyStream);
            });
        }

        public void LoadCDR(List<string> numberPrefix, DateTime from, DateTime to, int? batchSize, Action<CDR> onCDRReady)
        {
            var dbTimeRanges = PartitionedCDRDataManager.GetDBTimeRanges(from, to);
            foreach (var dbTimeRange in dbTimeRanges)
            {
                var dataManager = PartitionedCDRDataManagerFactory.GetCDRDataManager<PartitionedNormalCDRDataManager>(dbTimeRange.FromTime, true);
                if (dataManager != null)
                {
                    dataManager.LoadCDR(dbTimeRange.FromTime, numberPrefix, onCDRReady);
                }
            }
        }


        public BigResult<CDR> GetNormalCDRs(Vanrise.Entities.DataRetrievalInput<NormalCDRQuery> input)
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
                        ExecuteNonQueryText(String.Format(PartitionedCDRDataManager.NORMALCDR_CREATETABLE_QUERYTEMPLATE, tempTableName), null);
                        //foreach(var dbTimeRange in dbTimeRanges)
                        Parallel.ForEach(dbTimeRanges, (dbTimeRange) =>
                        {
                            var dataManager = PartitionedCDRDataManagerFactory.GetCDRDataManager<PartitionedNormalCDRDataManager>(dbTimeRange.FromTime, true);
                            if (dataManager != null)
                            {
                                dataManager.InsertCDRsByMSISDNToTempTable(tempTableName, input.Query.MSISDN, dbTimeRange.FromTime, dbTimeRange.ToTime);
                            }
                        });
                    }
                }
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, (new PartitionedNormalCDRDataManager()).NormalCDRMapper, _columnMapper);
        }

        #region Private Classes

        private class PartitionedNormalCDRStream
        {
            public PartitionedNormalCDRDataManager DataManager { get; set; }

            public Object DBApplyStream { get; set; }
        }

        private class NormalCDRDBApplyStream
        {
            public ConcurrentDictionary<DateTime, PartitionedNormalCDRStream> PartitionedStreamsByDBFromTime { get; set; }
        }

        #endregion
    }
}
