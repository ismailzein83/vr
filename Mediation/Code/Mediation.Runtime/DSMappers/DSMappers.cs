﻿using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Integration.Entities;

namespace Mediation.Runtime
{
    public static class DSMappers
    {
        #region Alcatel

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Alcatel_ICX(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = true };
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, dataSourceId, new Guid("30553F01-CE03-4D29-9BF5-80D0D06DFA34"), options, (parsedBatch) =>
            {
                Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Alcatel Parsed CDRs", parsedBatch.RecordType);
                switch (parsedBatch.RecordType)
                {
                    case "ICX_Alcatel_CDR":
                        mappedBatches.Add("CDRTransformationStage", batch);
                        break;
                    default: break;
                }
            });

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        #endregion

        #region Ericsson

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Ericsson_GPRS(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, dataSourceId, new Guid("B9648105-8914-4C70-8550-F63D946F5B0C"), (parsedBatch) =>
            {
                Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                mappedBatches.Add("GPRSStoreStage", batch);
            });

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Ericsson_Iraq(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, dataSourceId, new Guid("BA810002-0B4D-4563-9A0D-EE228D69A1A6"), (parsedBatch) =>
            {
                switch (parsedBatch.RecordType)
                {
                    case "MobileCDR":
                        List<dynamic> multiLegRecords = new List<dynamic>();
                        List<dynamic> normalRecords = new List<dynamic>();

                        foreach (dynamic record in parsedBatch.Records)
                        {
                            if (record.IntermediateRecordNumber != null)
                            {
                                record.SessionId = record.GlobalCallReference + "_" + record.RecordType;
                                multiLegRecords.Add(record);
                            }
                            else
                                normalRecords.Add(record);
                        }

                        Vanrise.Integration.Entities.MappedBatchItem normalBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(normalRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("MobileTransformationStage", normalBatch);

                        if (multiLegRecords.Count > 0)
                        {
                            Vanrise.Integration.Entities.MappedBatchItem multiLegBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(multiLegRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                            mappedBatches.Add("EricssonMediationStage", multiLegBatch);
                        }
                        break;

                    case "SMS":
                        Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("SMSTransformationStage", batch);
                        break;
                }
            });

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Ericsson_WHS_Binary(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = true };
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, dataSourceId, new Guid("6f27b54c-90f3-4332-8437-1adffdb8ed2d"), options, (parsedBatch) =>
            {
                Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                switch (parsedBatch.RecordType)
                {
                    case "WHS_Ericsson_CDR":
                        mappedBatches.Add("CDRTransformationStage", batch);
                        break;
                    default: break;
                }

            });
            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Ericsson_WHS_Txt(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData importedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("WHS_Ericsson_CDR");

            System.IO.StreamReader sr = importedData.StreamReader;
            string currentLine = sr.ReadLine();
            if (!string.IsNullOrEmpty(currentLine))
            {
                currentLine = currentLine.Replace("\0", "");

                while (true)
                {
                    if (currentLine.Length < 2)
                        break;

                    int lengthToRead;
                    string recordType = currentLine.Substring(0, 2);
                    switch (recordType)
                    {
                        case "07": lengthToRead = 89; break;
                        default: lengthToRead = 115; break;
                    }

                    if (currentLine.Length < lengthToRead)
                        break;

                    if (recordType != "07")
                    {
                        string cdrAsString = currentLine.Substring(0, lengthToRead);

                        dynamic cdr = Activator.CreateInstance(mediationCDRRuntimeType) as dynamic;

                        cdr.DataSourceId = dataSourceId;
                        cdr.FileName = importedData.Name;
                        cdr.RecordType = cdrAsString.Substring(0, 2);
                        cdr.CallStatus = cdrAsString.Substring(2, 1);
                        cdr.CauseForOutput = cdrAsString.Substring(3, 1);

                        string aNumber = cdrAsString.Substring(4, 20);
                        if (!string.IsNullOrEmpty(aNumber))
                            cdr.ANumber = Utilities.ReplaceString(aNumber.TrimStart(), "F", "", StringComparison.OrdinalIgnoreCase);

                        string bNumber = cdrAsString.Substring(24, 20);
                        if (!string.IsNullOrEmpty(bNumber))
                            cdr.BNumber = Utilities.ReplaceString(bNumber.TrimStart(), "F", "", StringComparison.OrdinalIgnoreCase);

                        cdr.ACategory = cdrAsString.Substring(44, 2);
                        cdr.BCategory = cdrAsString.Substring(46, 2);
                        cdr.ChargedParty = cdrAsString.Substring(48, 1);

                        string dateForStartCharging = cdrAsString.Substring(49, 6);
                        if (!string.IsNullOrEmpty(dateForStartCharging))
                        {
                            int year = Convert.ToInt32(dateForStartCharging.Substring(0, 2)) + 2000;
                            int month = Convert.ToInt32(dateForStartCharging.Substring(2, 2));
                            int day = Convert.ToInt32(dateForStartCharging.Substring(4, 2));
                            cdr.DateForStartCharging = new DateTime(year, month, day);
                        }

                        string timeForStartCharging = cdrAsString.Substring(55, 6);
                        if (!string.IsNullOrEmpty(timeForStartCharging))
                        {
                            int hour = Convert.ToInt32(timeForStartCharging.Substring(0, 2));
                            int minute = Convert.ToInt32(timeForStartCharging.Substring(2, 2));
                            int second = Convert.ToInt32(timeForStartCharging.Substring(4, 2));
                            cdr.TimeForStartCharging = new Time(hour, minute, second, 0);
                        }

                        string chargeableDuration = cdrAsString.Substring(61, 6);
                        if (!string.IsNullOrEmpty(chargeableDuration))
                            cdr.ChargeableDuration = Convert.ToInt32(chargeableDuration);

                        string faultCode = cdrAsString.Substring(67, 5);
                        if (!string.IsNullOrEmpty(faultCode))
                            cdr.FaultCode = faultCode.TrimStart();

                        string exchangeIdentity = cdrAsString.Substring(72, 15);
                        if (!string.IsNullOrEmpty(exchangeIdentity))
                            cdr.ExchangeIdentity = exchangeIdentity.TrimStart();

                        string recordNumber = cdrAsString.Substring(87, 2);
                        if (!string.IsNullOrEmpty(recordNumber))
                            cdr.RecordNumber = Convert.ToInt32(recordNumber);

                        cdr.TariffClass = cdrAsString.Substring(89, 3);
                        cdr.TariffSwitchingIndicator = cdrAsString.Substring(92, 1);
                        cdr.OriginForCharging = cdrAsString.Substring(93, 4);

                        string outgoingRoute = cdrAsString.Substring(97, 7);
                        if (!string.IsNullOrEmpty(outgoingRoute))
                            cdr.OutgoingRoute = outgoingRoute.TrimStart();

                        string incomingRoute = cdrAsString.Substring(104, 7);
                        if (!string.IsNullOrEmpty(incomingRoute))
                            cdr.IncomingRoute = incomingRoute.TrimStart();

                        cdrs.Add(cdr);
                    }
                    currentLine = currentLine.Remove(0, lengthToRead);
                }
            }

            if (cdrs.Count > 0)
            {
                long startingId;
                var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("WHS_Ericsson_CDR");
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, cdrs.Count, out startingId);

                foreach (var item in cdrs)
                {
                    item.Id = startingId++;
                }

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "WHS_Ericsson_CDR");
                mappedBatches.Add("CDRTransformationStage", batch);
            }
            else
                importedData.IsEmpty = true;

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;
        }

        #endregion

        #region Teles

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Teles(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("ParsedCDR");

            var cdrs = new List<dynamic>();

            try
            {
                System.IO.StreamReader sr = ImportedData.StreamReader;
                while (!sr.EndOfStream)
                {
                    string currentLine = sr.ReadLine();
                    if (string.IsNullOrEmpty(currentLine))
                        continue;
                    string[] rowData = currentLine.Split(',');
                    dynamic cdr = Activator.CreateInstance(mediationCDRRuntimeType) as dynamic;
                    cdr.TC_VERSIONID = rowData[0].Trim('"');
                    cdr.TC_CALLID = rowData[13].Trim('"');
                    cdr.TC_LOGTYPE = rowData[1].Trim('"');
                    cdr.TC_TIMESTAMP = DateTime.ParseExact(rowData[3].Trim('"'), "yyyyMMddHHmmss:fff", System.Globalization.CultureInfo.InvariantCulture);

                    cdr.TC_DISCONNECTREASON = rowData[4].Trim('"');
                    cdr.TC_CALLPROGRESSSTATE = rowData[5].Trim('"');
                    cdr.TC_ACCOUNT = rowData[6].Trim('"');
                    cdr.TC_ORIGINATORID = rowData[7].Trim('"');
                    cdr.TC_ORIGINATORNUMBER = rowData[7].Trim('"');
                    cdr.TC_ORIGINALFROMNUMBER = rowData[9].Trim('"');
                    cdr.TC_ORIGINALDIALEDNUMBER = rowData[10].Trim('"');
                    cdr.TC_TERMINATORID = rowData[11].Trim('"');
                    cdr.TC_TERMINATORNUMBER = rowData[12].Trim('"');
                    cdr.TC_INCOMINGGWID = rowData[15].Trim('"');
                    cdr.TC_OUTGOINGGWID = rowData[16].Trim('"');
                    cdr.TC_TRANSFERREDCALLID = rowData[20].Trim('"');
                    cdr.TC_TERMINATORIP = rowData[33].Trim('"');
                    cdr.TC_ORIGINATORIP = rowData[32].Trim('"');
                    cdr.TC_REPLACECALLID = rowData[18].Trim('"');
                    cdr.TC_CALLINDICATOR = rowData[14].Trim('"');
                    cdr.FileName = ImportedData.Name;
                    DateTime? attemptDateTime = default(DateTime?);
                    if (!string.IsNullOrEmpty(rowData[36].Trim('"')))
                        attemptDateTime = (DateTime?)(DateTime.ParseExact(rowData[36].Trim('"'), "yyyyMMddHHmmss:fff", System.Globalization.CultureInfo.InvariantCulture));
                    cdr.TC_SESSIONINITIATIONTIME = attemptDateTime;
                    cdr.TC_SEQUENCENUMBER = rowData[2].Trim('"');
                    cdrs.Add(cdr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "ParsedCDR");
            mappedBatches.Add("Teles Mediation Stage", batch);
            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;
        }

        #endregion

        #region Huawei

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Huawi_Iraq(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, dataSourceId, new Guid("3B0C2ED7-CC17-46C0-8F96-697BD185B273"), (parsedBatch) =>
            {
                switch (parsedBatch.RecordType)
                {
                    case "MobileCDR":
                        List<dynamic> multiLegRecords = new List<dynamic>();
                        List<dynamic> normalRecords = new List<dynamic>();
                        foreach (dynamic record in parsedBatch.Records)
                        {
                            if (record.SequenceNumber == null || record.SequenceNumber > 0)
                            {
                                record.SessionId = record.GlobalCallReference + "_" + record.RecordType + "_" + record.ConnectDateTime.ToString();
                                multiLegRecords.Add(record);
                            }
                            else
                                normalRecords.Add(record);
                        }
                        Vanrise.Integration.Entities.MappedBatchItem normalBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(normalRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("MobileTransformationStage", normalBatch);

                        if (multiLegRecords.Count > 0)
                        {
                            Vanrise.Integration.Entities.MappedBatchItem multiLegBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(multiLegRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                            mappedBatches.Add("HuaweiMediationStage", multiLegBatch);
                        }
                        break;

                    case "SMS":
                        Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("SMSTransformationStage", batch);
                        break;
                }
            });

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Huawi_Iraq_GPRS(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, dataSourceId, new Guid("16b6af8d-6a15-46a1-9c19-ccfac1ebbdde"), (parsedBatch) =>
            {
                Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                mappedBatches.Add("GPRSStoreStage", batch);
            });

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Huawi_Namibia(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, dataSourceId, new Guid("e2a77834-86da-42ba-9501-c3eb81f5f60b"), (parsedBatch) =>
            {
                Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);

                switch (parsedBatch.RecordType)
                {
                    case "WHS_CDR":
                        mappedBatches.Add("CDR_TransformationStep", batch);
                        break;

                    case "SMS":
                        mappedBatches.Add("SMS_TransformationStep", batch);
                        break;
                }
            });

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            result.Message = string.Format("Finished importing File {0}", ImportedData.Name);
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Huawi_WHS(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, dataSourceId, new Guid("504a12e9-61d2-4e31-b193-1d43749dc055"), (parsedBatch) =>
            {

            });

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        #endregion

        #region Nokia

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Nokia_EDR(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, dataSourceId, new Guid("230bedb5-a3ee-4cbe-802c-dfdaa2a2d438"), (parsedBatch) =>
            {
                switch (parsedBatch.RecordType)
                {
                    case "MobileCDR":
                        List<dynamic> multiLegRecords = new List<dynamic>();
                        List<dynamic> normalRecords = new List<dynamic>();
                        foreach (dynamic record in parsedBatch.Records)
                        {
                            if (record.IntermediateChargingIndicator != 0)
                            {
                                record.SessionId = record.GlobalCallReference + "_" + record.RecordType;
                                multiLegRecords.Add(record);
                            }
                            else
                                normalRecords.Add(record);
                        }
                        Vanrise.Integration.Entities.MappedBatchItem normalBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(normalRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("MobileTransformationStage", normalBatch);

                        if (multiLegRecords.Count > 0)
                        {
                            Vanrise.Integration.Entities.MappedBatchItem multiLegBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(multiLegRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                            mappedBatches.Add("NokiaMediationStage", multiLegBatch);
                        }
                        break;

                    case "SMS":
                        Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("SMSTransformationStage", batch);
                        break;
                }
            });

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_NokiaSiemens_ICX(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = true };
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, dataSourceId, new Guid("202c8508-a24c-4664-b769-be71c86fcd75"), options, (parsedBatch) =>
            {
                Vanrise.Integration.Entities.MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Nokia Siemens Parsed CDRs", parsedBatch.RecordType);
                switch (parsedBatch.RecordType)
                {
                    case "ICX_NokiaSiemens_CDR":
                        mappedBatches.Add("CDRTransformationStage", batch);
                        break;
                    default: break;
                }
            });

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        #endregion

        private static void LogVerbose(string Message)
        {
            Console.WriteLine(Message);
        }
    }
}