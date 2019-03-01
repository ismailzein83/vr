﻿using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Integration.Entities;

namespace TestRuntime
{
    public static class DSMappers
    {
        public static Vanrise.Integration.Entities.MappingOutput MapCDR_SQL(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            LogVerbose("Started");

            List<dynamic> cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("CDR");

            int maximumBatchSize = 50000;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("CDR");

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));

            IDataReader reader = importedData.Reader;

            int rowCount = 0;
            while (reader.Read())
            {
                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                cdr.SwitchId = 5;
                cdr.IDonSwitch = Utils.GetReaderValue<long>(reader, "IDonSwitch");
                cdr.Tag = reader["Tag"] as string;
                cdr.AttemptDateTime = (DateTime)reader["AttemptDateTime"];
                cdr.AlertDateTime = Utils.GetReaderValue<DateTime>(reader, "AlertDateTime");
                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectDateTime");
                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime>(reader, "DisconnectDateTime");
                cdr.DurationInSeconds = Utils.GetReaderValue<Decimal>(reader, "DurationInSeconds");
                cdr.InTrunk = reader["IN_TRUNK"] as string;
                cdr.InCircuit = reader["IN_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["IN_CIRCUIT"]) : default(Int64);
                cdr.InCarrier = reader["IN_CARRIER"] as string;
                cdr.InIP = reader["IN_IP"] as string;
                cdr.OutTrunk = reader["OUT_TRUNK"] as string;
                cdr.OutCircuit = reader["OUT_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["OUT_CIRCUIT"]) : default(Int64);
                cdr.OutCarrier = reader["OUT_CARRIER"] as string;
                cdr.OutIP = reader["OUT_IP"] as string;

                cdr.CGPN = reader["CGPN"] as string;
                cdr.CDPN = reader["CDPN"] as string;
                cdr.CauseFromReleaseCode = reader["CAUSE_FROM_RELEASE_CODE"] as string;
                cdr.CauseFrom = reader["CAUSE_FROM"] as string;
                cdr.CauseToReleaseCode = reader["CAUSE_TO_RELEASE_CODE"] as string;
                cdr.CauseTo = reader["CAUSE_TO"] as string;
                cdr.IsRerouted = reader["IsRerouted"] != DBNull.Value ? ((reader["IsRerouted"] as string) == "Y") : false;
                cdr.CDPNOut = reader["CDPNOut"] as string;
                cdr.CDPNIn = reader["CDPNIn"] as string;
                cdr.SIP = reader["SIP"] as string;

                cdrs.Add(cdr);
                importedData.LastImportedId = reader["CDRID"];

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

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
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

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_MySQL(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            LogVerbose("Started");

            List<dynamic> cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("CDR");

            int maximumBatchSize = 50000;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("CDR");

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));

            IDataReader reader = importedData.Reader;

            int rowCount = 0;
            while (reader.Read())
            {
                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                cdr.SwitchId = 1;
                cdr.IDonSwitch = Utils.GetReaderValue<long>(reader, "ID");
                //cdr.Tag = reader["Tag"] as string;
                cdr.AttemptDateTime = (DateTime)reader["AttemptTime"];
                //cdr.AlertDateTime = Utils.GetReaderValue<DateTime>(reader, "AlertDateTime");
                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectTime");
                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime>(reader, "DisconnectTime");
                cdr.DurationInSeconds = Utils.GetReaderValue<Decimal>(reader, "DurationInSeconds");
                cdr.InTrunk = reader["TrunkIN"] as string;
                //cdr.InCircuit = reader["IN_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["IN_CIRCUIT"]) : default(Int64);
                cdr.InCarrier = reader["CarrierIN"] as string;
                cdr.InIP = reader["IPIN"] as string;
                cdr.OutTrunk = reader["TrunkOUT"] as string;
                //cdr.OutCircuit = reader["OUT_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["OUT_CIRCUIT"]) : default(Int64);
                cdr.OutCarrier = reader["CarrierOUT"] as string;
                cdr.OutIP = reader["IPOUT"] as string;

                cdr.CGPN = reader["CGPN"] as string;
                cdr.CDPN = reader["CDPN"] as string;
                //cdr.CauseFromReleaseCode = reader["CAUSE_FROM_RELEASE_CODE"] as string;
                //cdr.CauseFrom = reader["CAUSE_FROM"] as string;
                //cdr.CauseToReleaseCode = reader["CAUSE_TO_RELEASE_CODE"] as string;
                //cdr.CauseTo = reader["CAUSE_TO"] as string;
                // cdr.IsRerouted = reader["IsRerouted"] != DBNull.Value ? ((reader["IsRerouted"] as string) == "Y") : false;
                //cdr.CDPNOut = reader["CDPNOut"] as string;
                //cdr.CDPNIn = reader["CDPNIn"] as string;
                //cdr.SIP = reader["SIP"] as string;

                cdrs.Add(cdr);
                importedData.LastImportedId = reader["ID"];

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

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
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

        public static Vanrise.Integration.Entities.MappingOutput MapDDR_File_Ericsson_Ogero_Txt(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData importedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var ddrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type ddrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Destination Data Record");

            System.IO.StreamReader sr = importedData.StreamReader;
            string currentLine = sr.ReadLine();
            currentLine = currentLine.Replace("\0", "");

            List<int> validRecordTypes = new List<int>() { 1 };

            int blockSize = 86 + 86 * 15;//86 header size, 86 record size, 15 number of records per block

            if (!string.IsNullOrEmpty(currentLine))
            {
                while (true)
                {
                    if (currentLine.Length < blockSize)
                        break;

                    string header = currentLine.Substring(0, 86);

                    int? recordType = null;
                    string recordTypeAsString = header.Substring(0, 1);
                    if (!string.IsNullOrEmpty(recordTypeAsString))
                        recordType = Convert.ToInt32(recordTypeAsString);

                    if (recordType.HasValue && validRecordTypes.Contains(recordType.Value))
                    {
                        Dictionary<string, string> extraFields = new Dictionary<string, string>();

                        string exchangeIdentity = header.Substring(1, 12);
                        if (!string.IsNullOrEmpty(exchangeIdentity))
                            exchangeIdentity = exchangeIdentity.Trim();

                        string skip_13_30 = header.Substring(13, 18);
                        if (!string.IsNullOrEmpty(skip_13_30))
                            extraFields.Add("Skip_H_13_30", skip_13_30.Trim());

                        int year = 2000 + Convert.ToInt32(header.Substring(31, 2));
                        int month = Convert.ToInt32(header.Substring(33, 2));
                        int day = Convert.ToInt32(header.Substring(35, 2));
                        int hour = Convert.ToInt32(header.Substring(37, 2));
                        int minute = Convert.ToInt32(header.Substring(39, 2));
                        DateTime recordingDate = new DateTime(year, month, day, hour, minute, 0);

                        string skip_41_42 = header.Substring(41, 2);
                        if (!string.IsNullOrEmpty(skip_41_42))
                            extraFields.Add("Skip_H_41_42", skip_41_42.Trim());

                        int a1 = Convert.ToInt32(header.Substring(43, 2));
                        int a2 = Convert.ToInt32(header.Substring(45, 4));

                        string skip_49_51 = header.Substring(49, 3);
                        if (!string.IsNullOrEmpty(skip_49_51))
                            extraFields.Add("Skip_H_49_51", skip_49_51.Trim());

                        int a3 = Convert.ToInt32(header.Substring(52, 1));
                        bool a4 = Convert.ToBoolean(Convert.ToInt32(header.Substring(53, 1)));

                        string skip_55_85 = header.Substring(55, 31);
                        if (!string.IsNullOrEmpty(skip_55_85))
                            extraFields.Add("Skip_H_55_85", skip_55_85.Trim());

                        for (int i = 1; i < a1; i++)
                        {
                            dynamic ddr = Activator.CreateInstance(ddrRuntimeType) as dynamic;
                            ddr.RecordType = recordType.Value;
                            ddr.ExchangeIdentity = exchangeIdentity;
                            ddr.RecordingDate = recordingDate;
                            ddr.FileName = importedData.Name;
                            ddr.A1 = a1;
                            ddr.A2 = a2;
                            ddr.A3 = a3;
                            ddr.A4 = a4;

                            string currentRecord = currentLine.Substring(86 * i, 86);
                            Dictionary<string, string> ddrExtraFields = new Dictionary<string, string>(extraFields);

                            string skip_0_6 = currentRecord.Substring(0, 7);
                            if (!string.IsNullOrEmpty(skip_0_6))
                                ddrExtraFields.Add("Skip_R_0_6", skip_0_6.Trim());

                            string trdCode = currentRecord.Substring(7, 7);
                            int trdInt;
                            if (!string.IsNullOrEmpty(trdCode) && int.TryParse(trdCode.Trim(), out trdInt) && trdInt > 0)
                                ddr.TRDCode = trdInt;

                            ddr.N1 = Convert.ToInt64(currentRecord.Substring(14, 10));
                            ddr.N2 = Convert.ToInt64(currentRecord.Substring(24, 10));
                            ddr.N3 = Convert.ToInt64(currentRecord.Substring(34, 10));

                            string skip_44_53 = currentRecord.Substring(44, 10);
                            if (!string.IsNullOrEmpty(skip_44_53))
                                ddrExtraFields.Add("Skip_R_44_53", skip_44_53.Trim());

                            ddr.B1 = Convert.ToInt64(currentRecord.Substring(54, 10));
                            ddr.T3 = Convert.ToInt32(currentRecord.Substring(64, 6));
                            ddr.T1 = Convert.ToInt32(currentRecord.Substring(70, 6));
                            ddr.T2 = Convert.ToInt32(currentRecord.Substring(76, 6));

                            string skip_82_85 = currentRecord.Substring(82, 4);
                            if (!string.IsNullOrEmpty(skip_82_85))
                                ddrExtraFields.Add("Skip_R_82_85", skip_82_85.Trim());

                            if (ddrExtraFields.Count > 0)
                                ddr.ExtraFields = ddrExtraFields;

                            ddrs.Add(ddr);
                        }
                    }
                    currentLine = currentLine.Remove(0, blockSize);

                    if (currentLine.Length > 0)
                        currentLine = currentLine.TrimStart();
                }
            }

            if (ddrs.Count > 0)
            {
                long startingId;
                var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("Destination Data Record");
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, ddrs.Count, out startingId);

                foreach (var item in ddrs)
                {
                    item.Id = startingId++;
                }

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(ddrs, "#RECORDSCOUNT# of DDRs", "Destination Data Record");
                mappedBatches.Add("Distribute DDRs", batch);
            }

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapRDR_File_Ericsson_Ogero_Txt(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData importedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var rdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type rdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Route Data Record");

            List<int> validRecordTypes = new List<int>() { 1 };
            int blockSize = 71 + 80 * 16; //71 header size, 80 record size, 16 number of records per block

            System.IO.StreamReader sr = importedData.StreamReader;
            string currentLine = sr.ReadLine();
            currentLine = currentLine.Replace("\0", "");

            if (!string.IsNullOrEmpty(currentLine))
            {
                while (true)
                {
                    if (currentLine.Length < blockSize)
                        break;

                    string header = currentLine.Substring(0, 71);

                    int? recordType = null;
                    string recordTypeAsString = header.Substring(0, 1);
                    if (!string.IsNullOrEmpty(recordTypeAsString))
                        recordType = Convert.ToInt32(recordTypeAsString);

                    if (recordType.HasValue && validRecordTypes.Contains(recordType.Value))
                    {
                        Dictionary<string, string> extraFields = new Dictionary<string, string>();

                        string exchangeIdentity = header.Substring(1, 12);
                        if (!string.IsNullOrEmpty(exchangeIdentity))
                            exchangeIdentity = exchangeIdentity.Trim();

                        string skip_13_29 = header.Substring(13, 17);
                        if (!string.IsNullOrEmpty(skip_13_29))
                            extraFields.Add("Skip_H_13_29", skip_13_29.Trim());

                        int year = 2000 + Convert.ToInt32(header.Substring(30, 2));
                        int month = Convert.ToInt32(header.Substring(32, 2));
                        int day = Convert.ToInt32(header.Substring(34, 2));
                        int hour = Convert.ToInt32(header.Substring(36, 2));
                        int minute = Convert.ToInt32(header.Substring(38, 2));
                        DateTime recordingDate = new DateTime(year, month, day, hour, minute, 0);

                        string skip_40_51 = header.Substring(40, 12);
                        if (!string.IsNullOrEmpty(skip_40_51))
                            extraFields.Add("Skip_H_40_51", skip_40_51.Trim());

                        int a1 = Convert.ToInt32(header.Substring(52, 2));
                        int a2 = Convert.ToInt32(header.Substring(54, 5));
                        int a3 = Convert.ToInt32(header.Substring(59, 3));

                        string skip_62_70 = header.Substring(62, 9);
                        if (!string.IsNullOrEmpty(skip_62_70))
                            extraFields.Add("Skip_H_62_70", skip_62_70.Trim());

                        for (int i = 1; i < a1; i++)
                        {
                            dynamic rdr = Activator.CreateInstance(rdrRuntimeType) as dynamic;
                            rdr.RecordType = recordType.Value;
                            rdr.ExchangeIdentity = exchangeIdentity;
                            rdr.RecordingDate = recordingDate;
                            rdr.FileName = importedData.Name;
                            rdr.A1 = a1;
                            rdr.A2 = a2;
                            rdr.A3 = a3;

                            string currentRecord = currentLine.Substring(71 + (80 * (i - 1)), 80);
                            Dictionary<string, string> rdrExtraFields = new Dictionary<string, string>(extraFields);

                            string routeName = currentRecord.Substring(0, 7);
                            if (!string.IsNullOrEmpty(routeName))
                                rdr.RouteName = routeName.Trim();

                            string numberOfDevicesAsString = currentRecord.Substring(7, 10);
                            if (!string.IsNullOrEmpty(numberOfDevicesAsString))
                            {
                                long numberOfDevicesAsNumber;
                                if (long.TryParse(numberOfDevicesAsString.Trim(), out numberOfDevicesAsNumber))
                                    rdr.NumberOfDevices = numberOfDevicesAsNumber;
                            }

                            string numberOfBidsAsString = currentRecord.Substring(17, 10);
                            if (!string.IsNullOrEmpty(numberOfBidsAsString))
                            {
                                long numberOfBidsAsNumber;
                                if (long.TryParse(numberOfBidsAsString.Trim(), out numberOfBidsAsNumber))
                                    rdr.NumberOfBids = numberOfBidsAsNumber;
                            }

                            string numberOfRejectionsAsString = currentRecord.Substring(27, 10);
                            if (!string.IsNullOrEmpty(numberOfRejectionsAsString))
                            {
                                long numberOfRejectionsAsNumber;
                                if (long.TryParse(numberOfRejectionsAsString.Trim(), out numberOfRejectionsAsNumber))
                                    rdr.NumberOfRejections = numberOfRejectionsAsNumber;
                            }

                            string accNbOfBlockedDevicesAsString = currentRecord.Substring(37, 10);
                            if (!string.IsNullOrEmpty(accNbOfBlockedDevicesAsString))
                            {
                                long accNbOfBlockedDevicesAsNumber;
                                if (long.TryParse(accNbOfBlockedDevicesAsString.Trim(), out accNbOfBlockedDevicesAsNumber))
                                    rdr.AccNbOfBlockedDevices = accNbOfBlockedDevicesAsNumber;
                            }

                            string accTrafficLevelAsString = currentRecord.Substring(47, 10);
                            if (!string.IsNullOrEmpty(accTrafficLevelAsString))
                            {
                                long accTrafficLevelAsNumber;
                                if (long.TryParse(accTrafficLevelAsString.Trim(), out accTrafficLevelAsNumber))
                                    rdr.AccTrafficLevel = accTrafficLevelAsNumber;
                            }

                            string numberOfBAnswersAsString = currentRecord.Substring(57, 10);
                            if (!string.IsNullOrEmpty(numberOfBAnswersAsString))
                            {
                                long numberOfBAnswersAsNumber;
                                if (long.TryParse(numberOfBAnswersAsString.Trim(), out numberOfBAnswersAsNumber))
                                    rdr.NumberOfBAnswers = numberOfBAnswersAsNumber;
                            }

                            string skip_67_79 = currentRecord.Substring(67, 13);
                            if (!string.IsNullOrEmpty(skip_67_79))
                                rdrExtraFields.Add("skip_R_67_79", skip_67_79.Trim());

                            if (rdrExtraFields.Count > 0)
                                rdr.ExtraFields = rdrExtraFields;

                            rdrs.Add(rdr);
                        }
                    }

                    currentLine = currentLine.Remove(0, blockSize);

                    if (currentLine.Length > 0)
                        currentLine = currentLine.TrimStart();
                }
            }

            if (rdrs.Count > 0)
            {
                long startingId;
                var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("Route Data Record");
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, rdrs.Count, out startingId);

                foreach (var item in rdrs)
                    item.Id = startingId++;

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(rdrs, "#RECORDSCOUNT# of DDRs", "Route Data Record");
                mappedBatches.Add("Distribute RDRs", batch);
            }

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapSMS_SQL(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            LogVerbose("Started");

            var smsList = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type smsRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("SMS");

            int maximumBatchSize = 50000;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("SMS");

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));

            IDataReader reader = importedData.Reader;

            int rowCount = 0;
            while (reader.Read())
            {
                dynamic sms = Activator.CreateInstance(smsRuntimeType) as dynamic;
                sms.SwitchId = 1;
                sms.IDonSwitch = Utils.GetReaderValue<long>(reader, "ID");
                sms.Tag = reader["Tag"] as string;
                sms.SentDateTime = (DateTime)reader["ClientRequestDate"];
                sms.DeliveredDateTime = Utils.GetReaderValue<DateTime?>(reader, "DeliveryDate");

                sms.InTrunk = reader["IN_TRUNK"] as string;
                sms.InCircuit = reader["IN_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["IN_CIRCUIT"]) : default(Int64);
                sms.InCarrier = reader["Customer"] as string;
                sms.InIP = reader["CustomerConnHost"] as string;

                sms.OutTrunk = reader["OUT_TRUNK"] as string;
                sms.OutCircuit = reader["OUT_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["OUT_CIRCUIT"]) : default(Int64);
                sms.OutCarrier = reader["Vendor"] as string;
                sms.OutIP = reader["VendorConnHost"] as string;

                sms.Sender = reader["SRCAddressIN"] as string;
                sms.Receiver = reader["DSTAddressIN"] as string;
                sms.ReceiverIn = reader["DSTAddressIN"] as string;
                sms.ReceiverOut = reader["DSTAddressOUT"] as string;

                sms.OriginationMCC = reader["OriginationMCC"] as string;
                sms.OriginationMNC = reader["OriginationMNC"] as string;
                sms.DestinationMCC = reader["DestinationMCC"] as string;
                sms.DestinationMNC = reader["DestinationMNC"] as string;

                sms.CustomerDeliveryStatus = Utils.GetReaderValue<int>(reader, "Delivered");
                sms.SupplierDeliveryStatus = Utils.GetReaderValue<int>(reader, "SupplierDeliveryStatus");

                smsList.Add(sms);
                importedData.LastImportedId = reader["ID"];

                rowCount++;
                if (rowCount == maximumBatchSize)
                    break;
            }

            if (smsList.Count > 0)
            {
                long startingId;
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, rowCount, out startingId);
                long currentSMSId = startingId;

                foreach (var sms in smsList)
                {
                    sms.Id = currentSMSId;
                    currentSMSId++;
                }
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(smsList, "#RECORDSCOUNT# of CDRs", "SMS");
                mappedBatches.Add("Distribute Raw SMSList Stage", batch);
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