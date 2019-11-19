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

        public static Vanrise.Integration.Entities.MappingOutput MapC5Records_CSV(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            var mtcObj = new { MSISDN = 3, IMSI = 1, ConnectDateTime = 21, Destination = 4, DurationInSeconds = 23, DisconnectDateTime = 22, CallClassId = 33, IsOnNet = 160, SubscriberTypeId = 73, IMEI = 2, BTS = 9, Cell = 10, UpVolume = -1, DownVolume = -1, CellLatitude = -1, CellLongitude = -1, ServiceTypeID = 12, ServiceVASName = 15, InTrunkId = 7, OutTrunkId = 8, ReleaseCode = 27, MSISDNAreaCode = 54, DestinationAreaCode = -1, AttemptDateTime = 84, AlertDateTime = 85 };
            var mocObj = new { MSISDN = 3, IMSI = 1, ConnectDateTime = 24, Destination = 5, DurationInSeconds = 26, DisconnectDateTime = 25, CallClassId = 38, IsOnNet = 187, SubscriberTypeId = 82, IMEI = 2, BTS = 12, Cell = 13, UpVolume = -1, DownVolume = -1, CellLatitude = -1, CellLongitude = -1, ServiceTypeID = 182, ServiceVASName = 154, InTrunkId = 10, OutTrunkId = 11, ReleaseCode = 30, MSISDNAreaCode = -1, DestinationAreaCode = -1, AttemptDateTime = 92, AlertDateTime = 93 };

            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Record Analysis C5 Record");

            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("Record Analysis C5 Record");

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
                    // Removing ; from end of line
                    cdrLine = cdrLine.Remove(cdrLine.Length - 1);
                    string[] fields = cdrLine.Split(',');

                    cdr.DataSource = dataSourceId;
                    cdr.FileName = importedData.Name;
                    int recordType = int.Parse(fields[0]);

                    dynamic temp;
                    switch (recordType)
                    {
                        case 0: temp = mocObj; cdr.CallType = 0; break;
                        case 1: temp = mtcObj; cdr.CallType = 1; break;
                        default: continue;
                    }

                    cdr.MSISDN = fields[temp.MSISDN];
                    cdr.IMSI = fields[temp.IMSI];

                    string connectDateTimeAsString = fields[temp.ConnectDateTime];
                    if (!string.IsNullOrWhiteSpace(connectDateTimeAsString))
                    {
                        int year;
                        if (!int.TryParse(connectDateTimeAsString.Substring(0, 2), out year))
                            throw new Exception(String.Format("ConnectDateTime '{0}' contains invalid Year value", connectDateTimeAsString));

                        year += 2000;

                        int month;
                        if (!int.TryParse(connectDateTimeAsString.Substring(2, 2), out month))
                            throw new Exception(String.Format("ConnectDateTime '{0}' contains invalid Month value", connectDateTimeAsString));

                        int day;
                        if (!int.TryParse(connectDateTimeAsString.Substring(4, 2), out day))
                            throw new Exception(String.Format("ConnectDateTime '{0}' contains invalid Day value", connectDateTimeAsString));

                        int hour;
                        if (!int.TryParse(connectDateTimeAsString.Substring(6, 2), out hour))
                            throw new Exception(String.Format("ConnectDateTime '{0}' contains invalid Hour value", connectDateTimeAsString));

                        int minute;
                        if (!int.TryParse(connectDateTimeAsString.Substring(8, 2), out minute))
                            throw new Exception(String.Format("ConnectDateTime '{0}' contains invalid Minute value", connectDateTimeAsString));

                        int second;
                        if (!int.TryParse(connectDateTimeAsString.Substring(10, 2), out second))
                            throw new Exception(String.Format("ConnectDateTime '{0}' contains invalid Second value", connectDateTimeAsString));

                        cdr.ConnectDateTime = new DateTime(year, month, day, hour, minute, second);
                    }

                    string alertDateTimeAsString = fields[temp.AlertDateTime];
                    if (!string.IsNullOrWhiteSpace(alertDateTimeAsString))
                    {
                        int year;
                        if (!int.TryParse(alertDateTimeAsString.Substring(0, 2), out year))
                            throw new Exception(String.Format("AlertDateTime '{0}' contains invalid Year value", alertDateTimeAsString));

                        year += 2000;

                        int month;
                        if (!int.TryParse(alertDateTimeAsString.Substring(2, 2), out month))
                            throw new Exception(String.Format("AlertDateTime '{0}' contains invalid Month value", alertDateTimeAsString));

                        int day;
                        if (!int.TryParse(alertDateTimeAsString.Substring(4, 2), out day))
                            throw new Exception(String.Format("AlertDateTime '{0}' contains invalid Day value", alertDateTimeAsString));

                        int hour;
                        if (!int.TryParse(alertDateTimeAsString.Substring(6, 2), out hour))
                            throw new Exception(String.Format("AlertDateTime '{0}' contains invalid Hour value", alertDateTimeAsString));

                        int minute;
                        if (!int.TryParse(alertDateTimeAsString.Substring(8, 2), out minute))
                            throw new Exception(String.Format("AlertDateTime '{0}' contains invalid Minute value", alertDateTimeAsString));

                        int second;
                        if (!int.TryParse(alertDateTimeAsString.Substring(10, 2), out second))
                            throw new Exception(String.Format("AlertDateTime '{0}' contains invalid Second value", alertDateTimeAsString));

                        cdr.AlertDateTime = new DateTime(year, month, day, hour, minute, second);
                    }

                    string attemptDateTimeAsString = fields[temp.AttemptDateTime];
                    if (!string.IsNullOrWhiteSpace(attemptDateTimeAsString))
                    {
                        int year;
                        if (!int.TryParse(attemptDateTimeAsString.Substring(0, 2), out year))
                            throw new Exception(String.Format("AttemptDateTime '{0}' contains invalid Year value", attemptDateTimeAsString));

                        year += 2000;

                        int month;
                        if (!int.TryParse(attemptDateTimeAsString.Substring(2, 2), out month))
                            throw new Exception(String.Format("AttemptDateTime '{0}' contains invalid Month value", attemptDateTimeAsString));

                        int day;
                        if (!int.TryParse(attemptDateTimeAsString.Substring(4, 2), out day))
                            throw new Exception(String.Format("AttemptDateTime '{0}' contains invalid Day value", attemptDateTimeAsString));

                        int hour;
                        if (!int.TryParse(attemptDateTimeAsString.Substring(6, 2), out hour))
                            throw new Exception(String.Format("AttemptDateTime '{0}' contains invalid Hour value", attemptDateTimeAsString));

                        int minute;
                        if (!int.TryParse(attemptDateTimeAsString.Substring(8, 2), out minute))
                            throw new Exception(String.Format("AttemptDateTime '{0}' contains invalid Minute value", attemptDateTimeAsString));

                        int second;
                        if (!int.TryParse(attemptDateTimeAsString.Substring(10, 2), out second))
                            throw new Exception(String.Format("AttemptDateTime '{0}' contains invalid Second value", attemptDateTimeAsString));

                        cdr.AttemptDateTime = new DateTime(year, month, day, hour, minute, second);
                    }

                    cdr.Destination = fields[temp.Destination];

                    if (decimal.TryParse(fields[temp.DurationInSeconds], out decimal durationInSeconds))
                        cdr.DurationInSeconds = durationInSeconds;

                    string disconnectDateTimeAsString = fields[temp.DisconnectDateTime];
                    if (!string.IsNullOrWhiteSpace(disconnectDateTimeAsString))
                    {
                        int year;
                        if (!int.TryParse(disconnectDateTimeAsString.Substring(0, 2), out year))
                            throw new Exception(String.Format("DisconnectDateTime '{0}' contains invalid Year value", disconnectDateTimeAsString));

                        year += 2000;

                        int month;
                        if (!int.TryParse(disconnectDateTimeAsString.Substring(2, 2), out month))
                            throw new Exception(String.Format("DisconnectDateTime '{0}' contains invalid Month value", disconnectDateTimeAsString));

                        int day;
                        if (!int.TryParse(disconnectDateTimeAsString.Substring(4, 2), out day))
                            throw new Exception(String.Format("DisconnectDateTime '{0}' contains invalid Day value", disconnectDateTimeAsString));

                        int hour;
                        if (!int.TryParse(disconnectDateTimeAsString.Substring(6, 2), out hour))
                            throw new Exception(String.Format("DisconnectDateTime '{0}' contains invalid Hour value", disconnectDateTimeAsString));

                        int minute;
                        if (!int.TryParse(disconnectDateTimeAsString.Substring(8, 2), out minute))
                            throw new Exception(String.Format("DisconnectDateTime '{0}' contains invalid Minute value", disconnectDateTimeAsString));

                        int second;
                        if (!int.TryParse(disconnectDateTimeAsString.Substring(10, 2), out second))
                            throw new Exception(String.Format("DisconnectDateTime '{0}' contains invalid Second value", disconnectDateTimeAsString));

                        cdr.DisconnectDateTime = new DateTime(year, month, day, hour, minute, second);
                    }

                    if (int.TryParse(fields[temp.CallClassId], out int callClassId))
                        cdr.CallClassId = callClassId;

                    if (bool.TryParse(fields[temp.IsOnNet], out bool isOnNet))
                        cdr.IsOnNet = isOnNet;

                    if (int.TryParse(fields[temp.SubscriberTypeId], out int subscriberTypeId))
                        cdr.SubscriberTypeID = subscriberTypeId;

                    cdr.IMEI = fields[temp.IMEI];
                    cdr.BTS = fields[temp.BTS];
                    cdr.Cell = fields[temp.Cell];


                    decimal upVolume = 0;
                    if (temp.UpVolume >= 0 && decimal.TryParse(fields[temp.UpVolume], out upVolume))
                        cdr.UpVolume = upVolume;

                    decimal downVolume = 0;
                    if (temp.DownVolume >= 0 && decimal.TryParse(fields[temp.DownVolume], out downVolume))
                        cdr.DownVolume = downVolume;

                    decimal cellLatitude = 0;
                    if (temp.CellLatitude >= 0 && decimal.TryParse(fields[temp.CellLatitude], out cellLatitude))
                        cdr.CellLatitude = cellLatitude;

                    decimal cellLongitude = 0;
                    if (temp.CellLongitude >= 0 && decimal.TryParse(fields[temp.CellLongitude], out cellLongitude))
                        cdr.CellLongitude = cellLongitude;

                    if (int.TryParse(fields[temp.ServiceTypeID], out int serviceTypeId))
                        cdr.ServiceTypeId = serviceTypeId;

                    cdr.ServiceVASName = fields[temp.ServiceVASName];

                    cdr.InTrunkId = fields[temp.InTrunkId];

                    cdr.OutTrunkId = fields[temp.OutTrunkId];

                    cdr.ReleaseCode = fields[temp.ReleaseCode];

                    if (temp.MSISDNAreaCode >= 0)
                        cdr.MSISDNAreaCode = fields[temp.MSISDNAreaCode];

                    if (temp.DestinationAreaCode >= 0)
                        cdr.DestinationAreaCode = fields[temp.DestinationAreaCode];

                    cdrs.Add(cdr);

                    rowCount++;
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

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of C5 Records", "Record Analysis C5 Record");
                mappedBatches.Add("Store C5 Records", batch);
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

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Qualitynet_Teles(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData importedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("CDR");
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("CDR");

            System.IO.StreamReader sr = importedData.StreamReader;

            string dateTimeFormat = "yyyyMMddHHmmssfff";

            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                if (string.IsNullOrEmpty(currentLine))
                    continue;

                string[] rowData = currentLine.Split(';');

                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;

                cdr.DataSource = dataSourceId;
                cdr.FileName = importedData.Name;
                cdr.Call_Id = rowData[0];

                string attemptDateAsString = rowData[10];
                if (string.IsNullOrEmpty(attemptDateAsString))
                    throw new NullReferenceException("Attempt date can't be empty");

                string attemptTimeAsString = rowData[11];
                cdr.AttemptDateTime = DateTime.ParseExact(string.Concat(attemptDateAsString, !string.IsNullOrEmpty(attemptTimeAsString) ? attemptTimeAsString : "000000000"), dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);

                string connectDateAsString = rowData[14];
                if (!string.IsNullOrEmpty(connectDateAsString))
                {
                    string connectTimeAsString = rowData[15];
                    cdr.ConnectDateTime = DateTime.ParseExact(string.Concat(connectDateAsString, !string.IsNullOrEmpty(connectTimeAsString) ? connectTimeAsString : "000000000"), dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                }

                string disconnectDateAsString = rowData[16];
                if (!string.IsNullOrEmpty(disconnectDateAsString))
                {
                    string disconnectTimeAsString = rowData[17];
                    cdr.DisconnectDateTime = DateTime.ParseExact(string.Concat(disconnectDateAsString, !string.IsNullOrEmpty(disconnectTimeAsString) ? disconnectTimeAsString : "000000000"), dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                }

                cdr.OriginatorNumber = rowData[2];
                cdr.TerminatorNumber = rowData[3];

                string durationInMillisecondsAsString = rowData[8];
                if (!string.IsNullOrEmpty(durationInMillisecondsAsString))
                {
                    if (decimal.TryParse(durationInMillisecondsAsString, out decimal durationInMilliseconds))
                        cdr.DurationInSeconds = durationInMilliseconds / 1000;
                }

                cdr.ExtraFields = new Dictionary<string, string>();

                string callStateAsString = rowData[1];
                if (!string.IsNullOrEmpty(callStateAsString) && int.TryParse(callStateAsString, out int callStateAsInt))
                    cdr.ExtraFields.Add("CallState", Convert.ToBoolean(callStateAsInt).ToString());

                string inTrunkAsString = rowData[4];
                if (!string.IsNullOrEmpty(inTrunkAsString))
                    cdr.ExtraFields.Add("InTrunk", inTrunkAsString);

                string inCircuitAsString = rowData[5];
                if (!string.IsNullOrEmpty(inCircuitAsString))
                    cdr.ExtraFields.Add("InCircuit", inCircuitAsString);

                string outTrunkAsString = rowData[6];
                if (!string.IsNullOrEmpty(outTrunkAsString))
                    cdr.ExtraFields.Add("OutTrunk", outTrunkAsString);

                string outCircuitAsString = rowData[7];
                if (!string.IsNullOrEmpty(outCircuitAsString))
                    cdr.ExtraFields.Add("OutCircuit", outCircuitAsString);

                string outDuration2AsString = rowData[9];
                if (!string.IsNullOrEmpty(outDuration2AsString))
                    cdr.ExtraFields.Add("Duration2(ms)", outDuration2AsString);

                string alertDateAsString = rowData[12];
                if (!string.IsNullOrEmpty(alertDateAsString))
                {
                    string alertTimeAsString = rowData[13];
                    cdr.ExtraFields.Add("AlertDateTime", DateTime.ParseExact(string.Concat(alertDateAsString, !string.IsNullOrEmpty(alertTimeAsString) ? alertTimeAsString : "000000000"), dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                }

                string utcOffsetAsString = rowData[18];
                if (!string.IsNullOrEmpty(utcOffsetAsString))
                    cdr.ExtraFields.Add("UTCOffset", utcOffsetAsString);

                string sipCallIdLegAAsString = rowData[19];
                if (!string.IsNullOrEmpty(sipCallIdLegAAsString))
                    cdr.ExtraFields.Add("SIPCallIdLegA", sipCallIdLegAAsString);

                string sipCallIdLegBAsString = rowData[20];
                if (!string.IsNullOrEmpty(sipCallIdLegBAsString))
                    cdr.ExtraFields.Add("SIPCallIdLegB", sipCallIdLegBAsString);

                string causeFromAsString = rowData[21];
                if (!string.IsNullOrEmpty(causeFromAsString))
                    cdr.ExtraFields.Add("CauseFrom", causeFromAsString);

                string causeFromReleaseCodeAsString = rowData[22];
                if (!string.IsNullOrEmpty(causeFromReleaseCodeAsString))
                    cdr.ExtraFields.Add("CauseFromReleaseCode", causeFromReleaseCodeAsString);

                string causeToReleaseCodeAsString = rowData[23];
                if (!string.IsNullOrEmpty(causeToReleaseCodeAsString))
                    cdr.ExtraFields.Add("CauseToReleaseCode", causeToReleaseCodeAsString);

                string mediaErrorAsString = rowData[24];
                if (!string.IsNullOrEmpty(mediaErrorAsString))
                    cdr.ExtraFields.Add("MediaError", mediaErrorAsString);

                string lastCallReroutedAsString = rowData[25];
                if (!string.IsNullOrEmpty(lastCallReroutedAsString) && int.TryParse(lastCallReroutedAsString, out int lastCallReroutedAsInt))
                    cdr.ExtraFields.Add("LastCallRerouted", Convert.ToBoolean(lastCallReroutedAsInt).ToString());

                cdrs.Add(cdr);
            }

            if (cdrs.Count > 0)
            {
                long startingId;
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, cdrs.Count, out startingId);
                long currentCDRId = startingId;

                foreach (var cdr in cdrs)
                {
                    cdr.ID = currentCDRId;
                    currentCDRId++;
                }

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
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
