using System;
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
                        case "01": lengthToRead = 354; break;
                        default: lengthToRead = 115; break;
                    }

                    if (currentLine.Length < lengthToRead)
                        break;

                    switch (recordType)
                    {
                        case "07": break;

                        case "01":
                            string cdrAsString_01 = currentLine.Substring(0, lengthToRead);

                            dynamic cdr_01 = Activator.CreateInstance(mediationCDRRuntimeType) as dynamic;
                            cdr_01.DataSourceId = dataSourceId;
                            cdr_01.FileName = importedData.Name;
                            cdr_01.RecordType = cdrAsString_01.Substring(0, 2);
                            cdr_01.CallStatus = cdrAsString_01.Substring(2, 1);
                            cdr_01.CauseForOutput = cdrAsString_01.Substring(3, 1);

                            string aNumber_01 = cdrAsString_01.Substring(4, 18);
                            if (!string.IsNullOrEmpty(aNumber_01))
                                cdr_01.ANumber = Utilities.ReplaceString(aNumber_01.Trim(), "F", "", StringComparison.OrdinalIgnoreCase);

                            string bNumber_01 = cdrAsString_01.Substring(22, 18);
                            if (!string.IsNullOrEmpty(bNumber_01))
                                cdr_01.BNumber = Utilities.ReplaceString(bNumber_01.Trim(), "F", "", StringComparison.OrdinalIgnoreCase);

                            cdr_01.ACategory = cdrAsString_01.Substring(40, 2);
                            cdr_01.BCategory = cdrAsString_01.Substring(42, 2);
                            cdr_01.ChargedParty = cdrAsString_01.Substring(44, 2);

                            string dateForStartCharging_01 = cdrAsString_01.Substring(46, 6);
                            if (!string.IsNullOrEmpty(dateForStartCharging_01))
                            {
                                int year = Convert.ToInt32(dateForStartCharging_01.Substring(0, 2)) + 2000;
                                int month = Convert.ToInt32(dateForStartCharging_01.Substring(2, 2));
                                int day = Convert.ToInt32(dateForStartCharging_01.Substring(4, 2));
                                cdr_01.DateForStartCharging = new DateTime(year, month, day);
                            }

                            string timeForStartCharging_01 = cdrAsString_01.Substring(52, 6);
                            if (!string.IsNullOrEmpty(timeForStartCharging_01))
                            {
                                int hour = Convert.ToInt32(timeForStartCharging_01.Substring(0, 2));
                                int minute = Convert.ToInt32(timeForStartCharging_01.Substring(2, 2));
                                int second = Convert.ToInt32(timeForStartCharging_01.Substring(4, 2));
                                cdr_01.TimeForStartCharging = new Time(hour, minute, second, 0);
                            }

                            string chargeableDuration_01 = cdrAsString_01.Substring(58, 6);
                            if (!string.IsNullOrEmpty(chargeableDuration_01))
                            {
                                int hour = Convert.ToInt32(chargeableDuration_01.Substring(0, 2));
                                int minute = Convert.ToInt32(chargeableDuration_01.Substring(2, 2));
                                int second = Convert.ToInt32(chargeableDuration_01.Substring(4, 2));
                                cdr_01.ChargeableDuration = (int)new TimeSpan(hour, minute, second).TotalSeconds;
                            }

                            string faultCode_01 = cdrAsString_01.Substring(64, 5);
                            if (!string.IsNullOrEmpty(faultCode_01))
                                cdr_01.FaultCode = faultCode_01.Trim();

                            string staffUser_01 = cdrAsString_01.Substring(102, 5);
                            if (!string.IsNullOrEmpty(staffUser_01))
                                cdr_01.StaffUser = staffUser_01.Trim();

                            string staffPass_01 = cdrAsString_01.Substring(107, 5);
                            if (!string.IsNullOrEmpty(staffPass_01))
                                cdr_01.StaffPass = staffPass_01.Trim();

                            string call_01 = cdrAsString_01.Substring(112, 5);
                            if (!string.IsNullOrEmpty(call_01))
                                cdr_01.Call = call_01.Trim();

                            string exchange_01 = cdrAsString_01.Substring(117, 5);
                            if (!string.IsNullOrEmpty(exchange_01))
                                cdr_01.Exchange = exchange_01.Trim();

                            string code_01 = cdrAsString_01.Substring(122, 5);
                            if (!string.IsNullOrEmpty(code_01))
                                cdr_01.Code = code_01.Trim();

                            string outgoingRoute_01 = cdrAsString_01.Substring(129, 7);
                            if (!string.IsNullOrEmpty(outgoingRoute_01))
                                cdr_01.OutgoingRoute = outgoingRoute_01.Trim();

                            string incomingRoute_01 = cdrAsString_01.Substring(136, 7);
                            if (!string.IsNullOrEmpty(incomingRoute_01))
                                cdr_01.IncomingRoute = incomingRoute_01.Trim();

                            string callingName_01 = cdrAsString_01.Substring(204, 2);
                            if (!string.IsNullOrEmpty(callingName_01))
                                cdr_01.CallingName = Convert.ToInt32(callingName_01);

                            string calledName_01 = cdrAsString_01.Substring(206, 2);
                            if (!string.IsNullOrEmpty(calledName_01))
                                cdr_01.CalledName = Convert.ToInt32(calledName_01);

                            cdrs.Add(cdr_01);
                            break;

                        default:
                            string cdrAsString = currentLine.Substring(0, lengthToRead);

                            dynamic cdr = Activator.CreateInstance(mediationCDRRuntimeType) as dynamic;
                            cdr.DataSourceId = dataSourceId;
                            cdr.FileName = importedData.Name;
                            cdr.RecordType = cdrAsString.Substring(0, 2);
                            cdr.CallStatus = cdrAsString.Substring(2, 1);
                            cdr.CauseForOutput = cdrAsString.Substring(3, 1);

                            string aNumber = cdrAsString.Substring(4, 20);
                            if (!string.IsNullOrEmpty(aNumber))
                                cdr.ANumber = Utilities.ReplaceString(aNumber.Trim(), "F", "", StringComparison.OrdinalIgnoreCase);

                            string bNumber = cdrAsString.Substring(24, 20);
                            if (!string.IsNullOrEmpty(bNumber))
                                cdr.BNumber = Utilities.ReplaceString(bNumber.Trim(), "F", "", StringComparison.OrdinalIgnoreCase);

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
                            {
                                int hour = Convert.ToInt32(chargeableDuration.Substring(0, 2));
                                int minute = Convert.ToInt32(chargeableDuration.Substring(2, 2));
                                int second = Convert.ToInt32(chargeableDuration.Substring(4, 2));
                                cdr.ChargeableDuration = (int)new TimeSpan(hour, minute, second).TotalSeconds;
                            }

                            string faultCode = cdrAsString.Substring(67, 5);
                            if (!string.IsNullOrEmpty(faultCode))
                                cdr.FaultCode = faultCode.Trim();


                            string call = cdrAsString.Substring(72, 5);
                            if (!string.IsNullOrEmpty(call))
                                cdr.Call = call.Trim();

                            string exchange = cdrAsString.Substring(77, 5);
                            if (!string.IsNullOrEmpty(exchange))
                                cdr.Exchange = exchange.Trim();

                            string code = cdrAsString.Substring(82, 5);
                            if (!string.IsNullOrEmpty(code))
                                cdr.Code = code.Trim();

                            string recordNumber = cdrAsString.Substring(87, 2);
                            if (!string.IsNullOrEmpty(recordNumber))
                                cdr.RecordNumber = Convert.ToInt32(recordNumber);

                            cdr.TariffClass = cdrAsString.Substring(89, 3);
                            cdr.TariffSwitchingIndicator = cdrAsString.Substring(92, 1);
                            cdr.OriginForCharging = cdrAsString.Substring(93, 4);

                            string outgoingRoute = cdrAsString.Substring(97, 7);
                            if (!string.IsNullOrEmpty(outgoingRoute))
                                cdr.OutgoingRoute = outgoingRoute.Trim();

                            string incomingRoute = cdrAsString.Substring(104, 7);
                            if (!string.IsNullOrEmpty(incomingRoute))
                                cdr.IncomingRoute = incomingRoute.Trim();

                            cdrs.Add(cdr);
                            break;
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

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Huawei_Jazz(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            var cdrs = new List<dynamic>();
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Jazz_Huawei_CDR");

            int batchSize = 0;

            string fileName = ImportedData.Name;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("Jazz_Huawei_CDR");

            System.IO.StreamReader sr = ImportedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string cdrLine = sr.ReadLine();
                if (string.IsNullOrEmpty(cdrLine))
                    continue;

                cdrLine = cdrLine.Replace("\"", "");
                batchSize++;
                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;

                string[] fields = cdrLine.Split(',');
                cdr.EventDirection = fields[0];
                cdr.IncomingSwitch = fields[1];
                cdr.OutgoingSwitch = fields[2];
                cdr.IncTrunk = fields[3];
                cdr.OutTrunk = fields[4];
                cdr.IncProduct = fields[5];
                cdr.OutProduct = fields[6];
                cdr.OrigANumber = fields[7];
                cdr.OrigBNumber = fields[8];

                string startDateAsString = fields[9];
                if (!string.IsNullOrEmpty(startDateAsString))
                {
                    int year = Convert.ToInt32(startDateAsString.Substring(0, 4));
                    int month = Convert.ToInt32(startDateAsString.Substring(4, 2));
                    int day = Convert.ToInt32(startDateAsString.Substring(6, 2));
                    cdr.StartDate = new DateTime(year, month, day);
                }

                string startTimeAsString= fields[10];
                if (!string.IsNullOrEmpty(startTimeAsString))
                {
                    int hour = Convert.ToInt32(startTimeAsString.Substring(0, 2));
                    int minute = Convert.ToInt32(startTimeAsString.Substring(2, 2));
                    int second = Convert.ToInt32(startTimeAsString.Substring(4, 2));
                    int millisecond = Convert.ToInt32(startTimeAsString.Substring(6, 2));
                    cdr.StartTime = new Time(hour, minute, second, millisecond);
                }

                string durationAsString = fields[11];
                if (!string.IsNullOrEmpty(durationAsString))
                {
                    decimal duration = decimal.Parse(durationAsString);
                    cdr.DurationInSeconds = duration / 1000;
                }

                string netStartDateAsString = fields[12];
                if (!string.IsNullOrEmpty(netStartDateAsString))
                {
                    int year = Convert.ToInt32(netStartDateAsString.Substring(0, 4));
                    int month = Convert.ToInt32(netStartDateAsString.Substring(4, 2));
                    int day = Convert.ToInt32(netStartDateAsString.Substring(6, 2));
                    cdr.NetStartDate = new DateTime(year, month, day);
                }

                string netStartTimeAsString = fields[13];
                if (!string.IsNullOrEmpty(netStartTimeAsString))
                {
                    int hour = Convert.ToInt32(netStartTimeAsString.Substring(0, 2));
                    int minute = Convert.ToInt32(netStartTimeAsString.Substring(2, 2));
                    int second = Convert.ToInt32(netStartTimeAsString.Substring(4, 2));
                    int millisecond = Convert.ToInt32(netStartTimeAsString.Substring(6, 2));
                    cdr.NetStartTime = new Time(hour, minute, second, millisecond);
                }

                string netDurationAsString = fields[14];
                if (!string.IsNullOrEmpty(netDurationAsString))
                {
                    decimal netDuration = decimal.Parse(netDurationAsString);
                    cdr.NetDurationInSeconds = netDuration / 1000;
                }

                cdr.DataVolume = fields[15];
                cdr.DataUnit = fields[16];
                cdr.UserType = fields[18];
                cdr.IMSINumber = fields[19];
                cdr.ServiceClass = fields[20];

                string teleServNumberAsString = fields[21];
                if (!string.IsNullOrEmpty(teleServNumberAsString))
                {
                    cdr.TELEServNumber = int.Parse(teleServNumberAsString);
                }
                
                cdr.Cell_Id = fields[22];
                cdr.RecordType = fields[23];
                cdr.FileName = fileName;

                cdrs.Add(cdr);
            }
            long startingId;
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, batchSize, out startingId);

            foreach (var item in cdrs)
                item.Id = startingId++;

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of CDRs", "Jazz_Huawei_CDR");
            mappedBatches.Add("CDRTransformationStage", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
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