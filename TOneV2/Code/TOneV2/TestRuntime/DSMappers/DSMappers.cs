using System;
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

            var cdrs = new List<dynamic>();
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
                importedData.IsEmpty = true;

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput ImportingCDR_MySQL(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            //Retail.Runtime.Mappers.RingoMapper mapper = new Retail.Runtime.Mappers.RingoMapper();
            //return DSMappers.ImportingCDR_MySQL(data);

            LogVerbose("Started");

            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("CDR");

            int maximumBatchSize = 15000;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("CDR");


            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));

            IDataReader reader = importedData.Reader;

            int rowCount = 0;
            while (reader.Read())
            {
                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                cdr.SwitchId = 83;
                cdr.IDonSwitch = Utils.GetReaderValue<long>(reader, "CDRId");
                //cdr.Tag = reader["Tag"] as string;
                cdr.AttemptDateTime = (DateTime)reader["AttemptDateTime"];
                //cdr.AlertDateTime = Utils.GetReaderValue<DateTime>(reader, "AlertDateTime");
                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectDateTime");
                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime>(reader, "DisconnectDateTime");
                //cdr.DurationInSeconds = Utils.GetReaderValue<Decimal>(reader, "DurationInSeconds");
                cdr.InTrunk = reader["INTRUNK"] as string;
                // cdr.InCircuit = reader["IN_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["IN_CIRCUIT"]) : default(Int64);
                //cdr.InCarrier = reader["IN_CARRIER"] as string;
                cdr.InIP = reader["INIP"] as string;
                cdr.OutTrunk = reader["OUTTRUNK"] as string;
                //cdr.OutCircuit = reader["OUT_CIRCUIT"] != DBNull.Value ? Convert.ToInt64(reader["OUT_CIRCUIT"]) : default(Int64);
                //cdr.OutCarrier = reader["OUT_CARRIER"] as string;
                cdr.OutIP = reader["OUTIP"] as string;

                cdr.CGPN = reader["CGPN"] as string;
                cdr.CDPN = reader["CDPN"] as string;
                // cdr.CauseFromReleaseCode = reader["CAUSE_FROM_RELEASE_CODE"] as string;
                // cdr.CauseFrom = reader["CAUSE_FROM"] as string;
                // cdr.CauseToReleaseCode = reader["CAUSE_TO_RELEASE_CODE"] as string;
                //cdr.CauseTo = reader["CAUSE_TO"] as string;
                // cdr.IsRerouted = reader["IsRerouted"] != DBNull.Value ? ((reader["IsRerouted"] as string) == "Y") : false;
                // cdr.CDPNOut = reader["CDPNOut"] as string;
                // cdr.CDPNIn = reader["CDPNIn"] as string;
                // cdr.SIP = reader["SIP"] as string;

                cdrs.Add(cdr);
                importedData.LastImportedId = reader["CDRID"];

                rowCount++;
                if (rowCount == maximumBatchSize)
                    break;
            }

            long startingId;
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, rowCount, out startingId);
            long currentCDRId = startingId;

            foreach (var cdr in cdrs)
            {
                cdr.Id = currentCDRId;
                currentCDRId++;
            }

            if (cdrs.Count > 0)
            {
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
            }
            else
                importedData.IsEmpty = true;

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
                            if (!string.IsNullOrEmpty(trdCode))
                                ddr.TRDCode = trdCode.Trim();

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

        private static void LogVerbose(string Message)
        {
            Console.WriteLine(Message);
        }

        public class mappedBatches
        {
            public static void Add(string activatorName, object batch)
            {
            }
        }

        public class Utils
        {
            public static T GetReaderValue<T>(IDataReader reader, string fieldName)
            {
                return reader[fieldName] != DBNull.Value ? (T)reader[fieldName] : default(T);
            }
        }
    }
}