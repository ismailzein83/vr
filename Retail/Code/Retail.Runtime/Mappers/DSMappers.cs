using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.Runtime
{
    public static class DSMappers
    {
        public static Vanrise.Integration.Entities.MappingOutput MapDataImportICX_SQL(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            LogVerbose("Started");

            var dataList = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type dataRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("ICX_RawData");

            int batchSize = 50000;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("ICX_RawData");

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));

            Guid batchIdentifier = Guid.NewGuid();

            IDataReader reader = importedData.Reader;

            int rowCount = 0;
            while (reader.Read())
            {
                dynamic dataRecord = Activator.CreateInstance(dataRuntimeType) as dynamic;

                dataRecord.RecordDateTime = Utils.GetReaderValue<DateTime>(reader, "RecordDateTime");

                string userName = reader["UserName"] as string;
                dataRecord.ISPName = userName.Substring(userName.IndexOf("@") + 1);
                dataRecord.UserName = userName;
                dataRecord.SessionId = reader["AcctSessionId"] as string;
                dataRecord.InputOctets = (decimal?)Utils.GetReaderValue<long?>(reader, "AcctInputOctets");
                dataRecord.OutputOctets = (decimal?)Utils.GetReaderValue<long?>(reader, "AcctOutputOctets");
                dataRecord.DataSourceId = dataSourceId;
                dataRecord.BatchIdentifier = batchIdentifier;
                dataList.Add(dataRecord);

                importedData.LastImportedId = reader["Id"];

                rowCount++;
                if (rowCount == batchSize)
                    break;
            }

            if (dataList.Count > 0)
            {
                long startingId;
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, rowCount, out startingId);
                long currentDataId = startingId;

                foreach (var dataRecord in dataList)
                {
                    dataRecord.Id = currentDataId;
                    currentDataId++;
                }
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(dataList, "#RECORDSCOUNT# of Raw dataList", "ICX_RawData");
                mappedBatches.Add("Prepare Data Stage", batch);
            }
            else
            {
                importedData.IsEmpty = true;
            }

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");

            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapC4CDRs_SQL(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            LogVerbose("Started");

            List<dynamic> cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Record Analysis C4 CDR");

            int maximumBatchSize = 50000;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("Record Analysis C4 CDR");

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));

            IDataReader reader = importedData.Reader;

            int rowCount = 0;
            while (reader.Read())
            {
                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                cdr.SwitchId = 8;
                cdr.AttemptDateTime = (DateTime)reader["AttemptDateTime"];
                cdr.AlertDateTime = Utils.GetReaderValue<DateTime?>(reader, "AlertDateTime");
                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "ConnectDateTime");
                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
                cdr.DurationInSeconds = Utils.GetReaderValue<Decimal>(reader, "DurationInSeconds");
                cdr.InTrunk = reader["IN_TRUNK"] as string;
                cdr.OutTrunk = reader["OUT_TRUNK"] as string;
                cdr.CGPN = reader["CGPN"] as string;
                cdr.CDPN = reader["CDPN"] as string;
                cdr.IsRerouted = reader["IsRerouted"] != DBNull.Value ? ((reader["IsRerouted"] as string) == "Y") : false;
                cdr.CauseFromReleaseCode = reader["CAUSE_FROM_RELEASE_CODE"] as string;
                cdr.CauseToReleaseCode = reader["CAUSE_TO_RELEASE_CODE"] as string;

                cdrs.Add(cdr);
                importedData.LastImportedId = reader["Id"];

                rowCount++;
                if (rowCount == maximumBatchSize)
                    break;

            }

            if (cdrs.Count > 0)
            {
                long startingId;
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, rowCount, out startingId);
                long currentCDRId = startingId;

                foreach (var cdr in cdrs)
                {
                    cdr.Id = currentCDRId;
                    currentCDRId++;
                }

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of C4 CDRs", "Record Analysis C4 CDR");
                mappedBatches.Add("Distribute C4 CDRs Stage", batch);
            }
            else
            {
                importedData.IsEmpty = true;
            }

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }


        private static void LogVerbose(string Message)
        {
            Console.WriteLine(Message);
        }

        private class Utils
        {
            public static T GetReaderValue<T>(IDataReader reader, string fieldName)
            {
                return reader[fieldName] != DBNull.Value ? (T)reader[fieldName] : default(T);
            }
        }
    }
}
