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

        #region Inspkt

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

        public static Vanrise.Integration.Entities.MappingOutput MapC4CDRs_CSV(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Record Analysis C4 CDR");

            int maximumBatchSize = 50000;

            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("Record Analysis C4 CDR");

            int rowCount = 0;

            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            System.IO.StreamReader sr = importedData.StreamReader;

            //Reading Headers Line
            if (!sr.EndOfStream)
                sr.ReadLine();

            while (!sr.EndOfStream)
            {
                string cdrLine = sr.ReadLine();
                if (string.IsNullOrEmpty(cdrLine))
                    continue;

                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;

                try
                {
                    string[] fields = cdrLine.Split(',');

                    string attemptDateTimeAsString = fields[1];
                    if (!string.IsNullOrEmpty(attemptDateTimeAsString))
                    {
                        DateTime attemptTime;
                        if (attemptDateTimeAsString.Length < 23 || !DateTime.TryParseExact(attemptDateTimeAsString.Substring(0, 23), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out attemptTime))
                            throw new Exception($"Invalid date format for AttemptTime '{attemptDateTimeAsString}'");
                        cdr.AttemptDateTime = attemptTime;
                    }

                    cdr.InTrunk = fields[2];
                    cdr.OutTrunk = fields[3];
                    cdr.CGPN = fields[7];
                    cdr.CDPN = fields[8];

                    cdrs.Add(cdr);

                    rowCount++;
                    if (rowCount == maximumBatchSize)
                        break;
                }
                catch (Exception ex)
                {
                    failedRecordIdentifiers.Add(rowCount);
                }
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

        #endregion

        public static Vanrise.Integration.Entities.MappingOutput ImportCDRs_SFTP(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            LogVerbose("Started");
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();
            var dataRecords = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("RetailBilling_CDR");
            Type dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("RetailBilling_Data");

            System.IO.StreamReader sr = ImportedData.StreamReader;

            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                if (string.IsNullOrEmpty(currentLine))
                    continue;

                string modifiedCurrentLine = currentLine.ToUpper();

                if (!modifiedCurrentLine.Contains("SESSION_TERMINATE"))
                    continue;

                bool shouldTakeCDR = false;
                bool isVoice = false;

                if (modifiedCurrentLine.Contains("SECONDS"))
                {
                    shouldTakeCDR = true;
                    isVoice = true;
                }

                if (!shouldTakeCDR && modifiedCurrentLine.Contains("TOTALOCTETS"))
                {
                    shouldTakeCDR = true;
                }

                if (!shouldTakeCDR)
                    continue;


                string[] receivedData = currentLine.Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries);
                string[] rowData = receivedData[1].Split(';');

                if (isVoice)
                {
                    dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;

                    cdr.ResourceName = rowData[9];
                    cdr.OtherPartyNumber = rowData[10];//txt
                    cdr.AttemptDateTime = DateTime.ParseExact(rowData[1], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    if (!string.IsNullOrEmpty(rowData[14]))
                        cdr.Duration = decimal.Parse(rowData[14]);

                    if (!string.IsNullOrEmpty(rowData[15]))
                        cdr.Amount = decimal.Parse(rowData[15]);

                    cdrs.Add(cdr);
                }
                else
                {
                    dynamic dataRecord = Activator.CreateInstance(dataRecordRuntimeType) as dynamic;

                    dataRecord.ResourceName = rowData[9];
                    dataRecord.AttemptDateTime = DateTime.ParseExact(rowData[1], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                    if (!string.IsNullOrEmpty(rowData[14]))
                        dataRecord.Volume = decimal.Parse(rowData[14]);

                    if (!string.IsNullOrEmpty(rowData[15]))
                        dataRecord.Amount = decimal.Parse(rowData[15]);

                    dataRecords.Add(dataRecord);
                }
            }


            if (cdrs.Count > 0)
            {
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of CDRs", "RetailBilling_CDR");
                mappedBatches.Add("CDR Transformation Stage", batch);
            }

            if (dataRecords.Count > 0)
            {
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(dataRecords, "#RECORDSCOUNT# of Data", "RetailBilling_Data");
                mappedBatches.Add("Data Transformation Stage", batch);
            }

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
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
