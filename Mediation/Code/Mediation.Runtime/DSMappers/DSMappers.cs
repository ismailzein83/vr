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
                            Dictionary<string, string> extraFields_01 = new Dictionary<string, string>();

                            cdr_01.DataSourceId = dataSourceId;
                            cdr_01.FileName = importedData.Name;
                            cdr_01.RecordType = cdrAsString_01.Substring(0, 2);
                            cdr_01.CallStatus = cdrAsString_01.Substring(2, 1);
                            cdr_01.CauseForOutput = cdrAsString_01.Substring(3, 1);

                            string aNumber_01 = cdrAsString_01.Substring(4, 18);
                            if (!string.IsNullOrWhiteSpace(aNumber_01))
                                cdr_01.ANumber = Utilities.ReplaceString(aNumber_01.Trim(), "F", "", StringComparison.OrdinalIgnoreCase);

                            string bNumber_01 = cdrAsString_01.Substring(22, 18);
                            if (!string.IsNullOrWhiteSpace(bNumber_01))
                                cdr_01.BNumber = Utilities.ReplaceString(bNumber_01.Trim(), "F", "", StringComparison.OrdinalIgnoreCase);

                            cdr_01.ACategory = cdrAsString_01.Substring(40, 2);
                            cdr_01.BCategory = cdrAsString_01.Substring(42, 2);
                            cdr_01.ChargedParty = cdrAsString_01.Substring(44, 2);

                            string dateForStartCharging_01 = cdrAsString_01.Substring(46, 6);
                            if (!string.IsNullOrWhiteSpace(dateForStartCharging_01))
                            {
                                int year = Convert.ToInt32(dateForStartCharging_01.Substring(0, 2)) + 2000;
                                int month = Convert.ToInt32(dateForStartCharging_01.Substring(2, 2));
                                int day = Convert.ToInt32(dateForStartCharging_01.Substring(4, 2));
                                cdr_01.DateForStartCharging = new DateTime(year, month, day);
                            }

                            string timeForStartCharging_01 = cdrAsString_01.Substring(52, 6);
                            if (!string.IsNullOrWhiteSpace(timeForStartCharging_01))
                            {
                                int hour = Convert.ToInt32(timeForStartCharging_01.Substring(0, 2));
                                int minute = Convert.ToInt32(timeForStartCharging_01.Substring(2, 2));
                                int second = Convert.ToInt32(timeForStartCharging_01.Substring(4, 2));
                                cdr_01.TimeForStartCharging = new Time(hour, minute, second, 0);
                            }

                            string chargeableDuration_01 = cdrAsString_01.Substring(58, 6);
                            if (!string.IsNullOrWhiteSpace(chargeableDuration_01))
                            {
                                int hour = Convert.ToInt32(chargeableDuration_01.Substring(0, 2));
                                int minute = Convert.ToInt32(chargeableDuration_01.Substring(2, 2));
                                int second = Convert.ToInt32(chargeableDuration_01.Substring(4, 2));
                                cdr_01.ChargeableDuration = (int)new TimeSpan(hour, minute, second).TotalSeconds;
                            }

                            string faultCode_01 = cdrAsString_01.Substring(64, 5);
                            if (!string.IsNullOrWhiteSpace(faultCode_01))
                                cdr_01.FaultCode = faultCode_01.Trim();

                            string classOfCall_01 = cdrAsString_01.Substring(69, 2);
                            if (!string.IsNullOrWhiteSpace(classOfCall_01))
                                extraFields_01.Add("ClassOfCall", classOfCall_01.Trim());

                            string priorityOfCall_01 = cdrAsString_01.Substring(71, 1);
                            if (!string.IsNullOrWhiteSpace(priorityOfCall_01))
                                extraFields_01.Add("PriorityOfCall", priorityOfCall_01.Trim());

                            string normalData_01 = cdrAsString_01.Substring(72, 1);
                            if (!string.IsNullOrWhiteSpace(normalData_01))
                                extraFields_01.Add("NormalData", normalData_01.Trim());

                            string ircd_01 = cdrAsString_01.Substring(73, 1);
                            if (!string.IsNullOrWhiteSpace(ircd_01))
                                extraFields_01.Add("IRCD", ircd_01.Trim());

                            string totalRCD_01 = cdrAsString_01.Substring(74, 4);
                            if (!string.IsNullOrWhiteSpace(totalRCD_01))
                                extraFields_01.Add("TotalRCD", totalRCD_01.Trim());

                            string serviceIdentity_01 = cdrAsString_01.Substring(78, 1);
                            if (!string.IsNullOrWhiteSpace(serviceIdentity_01))
                                extraFields_01.Add("ServiceIdentity", serviceIdentity_01.Trim());

                            string personalCall_01 = cdrAsString_01.Substring(79, 1);
                            if (!string.IsNullOrWhiteSpace(personalCall_01))
                                extraFields_01.Add("PersonalCall", personalCall_01.Trim());

                            string callType_01 = cdrAsString_01.Substring(80, 2);
                            if (!string.IsNullOrWhiteSpace(callType_01))
                                extraFields_01.Add("CallType", callType_01.Trim());

                            string chargedNumber_01 = cdrAsString_01.Substring(82, 18);
                            if (!string.IsNullOrWhiteSpace(chargedNumber_01))
                                extraFields_01.Add("ChargedNumber", chargedNumber_01.Trim());

                            string reducedCharges_01 = cdrAsString_01.Substring(100, 2);
                            if (!string.IsNullOrWhiteSpace(reducedCharges_01))
                                extraFields_01.Add("ReducedCharges", reducedCharges_01.Trim());

                            string signatureOfBookingOperator_01 = cdrAsString_01.Substring(102, 5);
                            if (!string.IsNullOrWhiteSpace(signatureOfBookingOperator_01))
                                extraFields_01.Add("SignatureOfBookingOperator", signatureOfBookingOperator_01.Trim());

                            string signatureOfCallHandlingOperator_01 = cdrAsString_01.Substring(107, 5);
                            if (!string.IsNullOrWhiteSpace(signatureOfCallHandlingOperator_01))
                                extraFields_01.Add("SignatureOfCallHandlingOperator", signatureOfCallHandlingOperator_01.Trim());

                            string call_01 = cdrAsString_01.Substring(112, 5);
                            if (!string.IsNullOrWhiteSpace(call_01))
                                cdr_01.Call = call_01.Trim();

                            string exchange_01 = cdrAsString_01.Substring(117, 5);
                            if (!string.IsNullOrWhiteSpace(exchange_01))
                                cdr_01.Exchange = exchange_01.Trim();

                            string code_01 = cdrAsString_01.Substring(122, 5);
                            if (!string.IsNullOrWhiteSpace(code_01))
                                cdr_01.Code = code_01.Trim();

                            string recordNumber_01 = cdrAsString_01.Substring(127, 2);
                            if (!string.IsNullOrWhiteSpace(recordNumber_01))
                                extraFields_01.Add("RecordNumber", recordNumber_01.Trim());

                            string outgoingRoute_01 = cdrAsString_01.Substring(129, 7);
                            if (!string.IsNullOrWhiteSpace(outgoingRoute_01))
                                cdr_01.OutgoingRoute = outgoingRoute_01.Trim();

                            string incomingRoute_01 = cdrAsString_01.Substring(136, 7);
                            if (!string.IsNullOrWhiteSpace(incomingRoute_01))
                                cdr_01.IncomingRoute = incomingRoute_01.Trim();

                            string isi_01 = cdrAsString_01.Substring(143, 1);
                            if (!string.IsNullOrWhiteSpace(isi_01))
                                extraFields_01.Add("ISI", isi_01.Trim());

                            string price_01 = cdrAsString_01.Substring(144, 8);
                            if (!string.IsNullOrWhiteSpace(price_01))
                                extraFields_01.Add("Price", price_01.Trim());

                            string originOfCharging_01 = cdrAsString_01.Substring(152, 4);
                            if (!string.IsNullOrWhiteSpace(originOfCharging_01))
                                extraFields_01.Add("OriginOfCharging", originOfCharging_01.Trim());

                            string accountingTime_01 = cdrAsString_01.Substring(156, 6);
                            if (!string.IsNullOrWhiteSpace(accountingTime_01))
                            {
                                string accountingTime_hour = accountingTime_01.Substring(0, 2);
                                string accountingTime_minute = accountingTime_01.Substring(2, 2);
                                string accountingTime_second = accountingTime_01.Substring(4, 2);
                                extraFields_01.Add("AccountingTime", string.Format("{0}:{1}:{2}", accountingTime_hour, accountingTime_minute, accountingTime_second));
                            }

                            string tarrif_01 = cdrAsString_01.Substring(162, 3);
                            if (!string.IsNullOrWhiteSpace(tarrif_01))
                                extraFields_01.Add("Tarrif", tarrif_01.Trim());

                            string eOfSASide_01 = cdrAsString_01.Substring(165, 2);
                            if (!string.IsNullOrWhiteSpace(eOfSASide_01))
                                extraFields_01.Add("EOfSASide", eOfSASide_01.Trim());

                            string iat_01 = cdrAsString_01.Substring(167, 1);
                            if (!string.IsNullOrWhiteSpace(iat_01))
                                extraFields_01.Add("IAT", iat_01.Trim());

                            string automaticallyTransferredANumber_01 = cdrAsString_01.Substring(168, 10);
                            if (!string.IsNullOrWhiteSpace(automaticallyTransferredANumber_01))
                                extraFields_01.Add("AutomaticallyTransferredANumber", automaticallyTransferredANumber_01.Trim());

                            string serialCallId_01 = cdrAsString_01.Substring(178, 1);
                            if (!string.IsNullOrWhiteSpace(serialCallId_01))
                                extraFields_01.Add("SerialCallId", serialCallId_01.Trim());

                            string serialCallIdNumber_01 = cdrAsString_01.Substring(179, 3);
                            if (!string.IsNullOrWhiteSpace(serialCallIdNumber_01))
                                extraFields_01.Add("SerialCallIdNumber", serialCallIdNumber_01.Trim());

                            string serialCallSequenceNumber_01 = cdrAsString_01.Substring(182, 2);
                            if (!string.IsNullOrWhiteSpace(serialCallSequenceNumber_01))
                                extraFields_01.Add("SerialCallSequenceNumber", serialCallSequenceNumber_01.Trim());

                            string dsoe_01 = cdrAsString_01.Substring(184, 1);
                            if (!string.IsNullOrWhiteSpace(dsoe_01))
                                extraFields_01.Add("DSOE", dsoe_01.Trim());

                            string oldChargeableDuration_01 = cdrAsString_01.Substring(185, 6);
                            if (!string.IsNullOrWhiteSpace(oldChargeableDuration_01))
                            {
                                string oldChargeableDuration_hour = oldChargeableDuration_01.Substring(0, 2);
                                string oldChargeableDuration_minute = oldChargeableDuration_01.Substring(2, 2);
                                string oldChargeableDuration_second = oldChargeableDuration_01.Substring(4, 2);
                                extraFields_01.Add("OldChargeableDuration", string.Format("{0}:{1}:{2}", oldChargeableDuration_hour, oldChargeableDuration_minute, oldChargeableDuration_second));
                            }

                            string originalTimeForStartOfCharging_01 = cdrAsString_01.Substring(191, 6);
                            if (!string.IsNullOrWhiteSpace(originalTimeForStartOfCharging_01))
                            {
                                string originalTimeForStartOfCharging_hour = originalTimeForStartOfCharging_01.Substring(0, 2);
                                string originalTimeForStartOfCharging_minute = originalTimeForStartOfCharging_01.Substring(2, 2);
                                string originalTimeForStartOfCharging_second = originalTimeForStartOfCharging_01.Substring(4, 2);
                                extraFields_01.Add("OriginalTimeForStartOfCharging", string.Format("{0}:{1}:{2}", originalTimeForStartOfCharging_hour, originalTimeForStartOfCharging_minute, originalTimeForStartOfCharging_second));
                            }

                            string numberOfTimeResets_01 = cdrAsString_01.Substring(197, 3);
                            if (!string.IsNullOrWhiteSpace(numberOfTimeResets_01))
                                extraFields_01.Add("NumberOfTimeResets", numberOfTimeResets_01.Trim());

                            string timeForTheCallRequest_01 = cdrAsString_01.Substring(200, 8);
                            if (!string.IsNullOrWhiteSpace(timeForTheCallRequest_01))
                            {
                                string timeForTheCallRequest_month = timeForTheCallRequest_01.Substring(0, 2);
                                string timeForTheCallRequest_day = timeForTheCallRequest_01.Substring(2, 2);
                                string timeForTheCallRequest_hour = timeForTheCallRequest_01.Substring(4, 2);
                                string timeForTheCallRequest_minute = timeForTheCallRequest_01.Substring(6, 2);
                                extraFields_01.Add("TimeForTheCallRequest", string.Format("{0}-{1} {2}:{3}", timeForTheCallRequest_month, timeForTheCallRequest_day, timeForTheCallRequest_hour, timeForTheCallRequest_minute));
                            }

                            string timeForCancelingTheCallRequest_01 = cdrAsString_01.Substring(208, 8);
                            if (!string.IsNullOrWhiteSpace(timeForCancelingTheCallRequest_01))
                            {
                                string timeForCancelingTheCallRequest_month = timeForCancelingTheCallRequest_01.Substring(0, 2);
                                string timeForCancelingTheCallRequest_day = timeForCancelingTheCallRequest_01.Substring(2, 2);
                                string timeForCancelingTheCallRequest_hour = timeForCancelingTheCallRequest_01.Substring(4, 2);
                                string timeForCancelingTheCallRequest_minute = timeForCancelingTheCallRequest_01.Substring(6, 2);
                                extraFields_01.Add("TimeForCancelingTheCallRequest", string.Format("{0}-{1} {2}:{3}", timeForCancelingTheCallRequest_month, timeForCancelingTheCallRequest_day, timeForCancelingTheCallRequest_hour, timeForCancelingTheCallRequest_minute));
                            }

                            string cri_01 = cdrAsString_01.Substring(216, 1);
                            if (!string.IsNullOrWhiteSpace(cri_01))
                                extraFields_01.Add("CRI", cri_01.Trim());

                            string callEstablishmentNumberBSide_01 = cdrAsString_01.Substring(217, 18);
                            if (!string.IsNullOrWhiteSpace(callEstablishmentNumberBSide_01))
                                extraFields_01.Add("CallEstablishmentNumberBSide", callEstablishmentNumberBSide_01.Trim());

                            string miscellaneousBSubscriberData_01 = cdrAsString_01.Substring(235, 18);
                            if (!string.IsNullOrWhiteSpace(miscellaneousBSubscriberData_01))
                                extraFields_01.Add("MiscellaneousBSubscriberData", miscellaneousBSubscriberData_01.Trim());

                            string callEstablishmentNumberASide_01 = cdrAsString_01.Substring(253, 18);
                            if (!string.IsNullOrWhiteSpace(callEstablishmentNumberASide_01))
                                extraFields_01.Add("CallEstablishmentNumberASide", callEstablishmentNumberASide_01.Trim());

                            string miscellaneousASubscriberData_01 = cdrAsString_01.Substring(271, 18);
                            if (!string.IsNullOrWhiteSpace(miscellaneousASubscriberData_01))
                                extraFields_01.Add("MiscellaneousASubscriberData", miscellaneousASubscriberData_01.Trim());

                            string callEstablishmentNumberThreeSide_01 = cdrAsString_01.Substring(289, 18);
                            if (!string.IsNullOrWhiteSpace(callEstablishmentNumberThreeSide_01))
                                extraFields_01.Add("CallEstablishmentNumberThreeSide", callEstablishmentNumberThreeSide_01.Trim());

                            string miscellaneousThreeSubscriberData_01 = cdrAsString_01.Substring(307, 18);
                            if (!string.IsNullOrWhiteSpace(miscellaneousThreeSubscriberData_01))
                                extraFields_01.Add("MiscellaneousThreeSubscriberData", miscellaneousThreeSubscriberData_01.Trim());

                            string antm_01 = cdrAsString_01.Substring(325, 4);
                            if (!string.IsNullOrWhiteSpace(antm_01))
                                extraFields_01.Add("ANTM", antm_01.Trim());

                            string hatm_01 = cdrAsString_01.Substring(329, 4);
                            if (!string.IsNullOrWhiteSpace(hatm_01))
                                extraFields_01.Add("HATM", hatm_01.Trim());

                            string signatureOrNumberAtInquiryCall_01 = cdrAsString_01.Substring(333, 18);
                            if (!string.IsNullOrWhiteSpace(signatureOrNumberAtInquiryCall_01))
                                extraFields_01.Add("SignatureOrNumberAtInquiryCall", signatureOrNumberAtInquiryCall_01.Trim());

                            if (extraFields_01.Count > 0)
                                cdr_01.ExtraFields = extraFields_01;

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
                            if (!string.IsNullOrWhiteSpace(aNumber))
                                cdr.ANumber = Utilities.ReplaceString(aNumber.Trim(), "F", "", StringComparison.OrdinalIgnoreCase);

                            string bNumber = cdrAsString.Substring(24, 20);
                            if (!string.IsNullOrWhiteSpace(bNumber))
                                cdr.BNumber = Utilities.ReplaceString(bNumber.Trim(), "F", "", StringComparison.OrdinalIgnoreCase);

                            cdr.ACategory = cdrAsString.Substring(44, 2);
                            cdr.BCategory = cdrAsString.Substring(46, 2);
                            cdr.ChargedParty = cdrAsString.Substring(48, 1);

                            string dateForStartCharging = cdrAsString.Substring(49, 6);
                            if (!string.IsNullOrWhiteSpace(dateForStartCharging))
                            {
                                int year = Convert.ToInt32(dateForStartCharging.Substring(0, 2)) + 2000;
                                int month = Convert.ToInt32(dateForStartCharging.Substring(2, 2));
                                int day = Convert.ToInt32(dateForStartCharging.Substring(4, 2));
                                cdr.DateForStartCharging = new DateTime(year, month, day);
                            }

                            string timeForStartCharging = cdrAsString.Substring(55, 6);
                            if (!string.IsNullOrWhiteSpace(timeForStartCharging))
                            {
                                int hour = Convert.ToInt32(timeForStartCharging.Substring(0, 2));
                                int minute = Convert.ToInt32(timeForStartCharging.Substring(2, 2));
                                int second = Convert.ToInt32(timeForStartCharging.Substring(4, 2));
                                cdr.TimeForStartCharging = new Time(hour, minute, second, 0);
                            }

                            string chargeableDuration = cdrAsString.Substring(61, 6);
                            if (!string.IsNullOrWhiteSpace(chargeableDuration))
                            {
                                int hour = Convert.ToInt32(chargeableDuration.Substring(0, 2));
                                int minute = Convert.ToInt32(chargeableDuration.Substring(2, 2));
                                int second = Convert.ToInt32(chargeableDuration.Substring(4, 2));
                                cdr.ChargeableDuration = (int)new TimeSpan(hour, minute, second).TotalSeconds;
                            }

                            string faultCode = cdrAsString.Substring(67, 5);
                            if (!string.IsNullOrWhiteSpace(faultCode))
                                cdr.FaultCode = faultCode.Trim();


                            string call = cdrAsString.Substring(72, 5);
                            if (!string.IsNullOrWhiteSpace(call))
                                cdr.Call = call.Trim();

                            string exchange = cdrAsString.Substring(77, 5);
                            if (!string.IsNullOrWhiteSpace(exchange))
                                cdr.Exchange = exchange.Trim();

                            string code = cdrAsString.Substring(82, 5);
                            if (!string.IsNullOrWhiteSpace(code))
                                cdr.Code = code.Trim();

                            string recordNumber = cdrAsString.Substring(87, 2);
                            if (!string.IsNullOrWhiteSpace(recordNumber))
                                cdr.RecordNumber = Convert.ToInt32(recordNumber);

                            cdr.TariffClass = cdrAsString.Substring(89, 3);
                            cdr.TariffSwitchingIndicator = cdrAsString.Substring(92, 1);
                            cdr.OriginForCharging = cdrAsString.Substring(93, 4);

                            string outgoingRoute = cdrAsString.Substring(97, 7);
                            if (!string.IsNullOrWhiteSpace(outgoingRoute))
                                cdr.OutgoingRoute = outgoingRoute.Trim();

                            string incomingRoute = cdrAsString.Substring(104, 7);
                            if (!string.IsNullOrWhiteSpace(incomingRoute))
                                cdr.IncomingRoute = incomingRoute.Trim();

                            string routeId = cdrAsString.Substring(111, 4);
                            if (!string.IsNullOrWhiteSpace(routeId))
                                cdr.RouteId = routeId.Trim();

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

                string startTimeAsString = fields[10];
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
                    int duration_hour = Convert.ToInt32(durationAsString.Substring(2, 2));
                    int duration_minute = Convert.ToInt32(durationAsString.Substring(4, 2));
                    int duration_second = Convert.ToInt32(durationAsString.Substring(6, 2));
                    int duration_millisecond = Convert.ToInt32(durationAsString.Substring(8, 2));

                    decimal duration = (decimal)(new TimeSpan(0, duration_hour, duration_minute, duration_second, duration_millisecond).TotalSeconds);
                    cdr.DurationInSeconds = duration;
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
                    int netDuration_hour = Convert.ToInt32(netDurationAsString.Substring(2, 2));
                    int netDuration_minute = Convert.ToInt32(netDurationAsString.Substring(4, 2));
                    int netDuration_second = Convert.ToInt32(netDurationAsString.Substring(6, 2));
                    int netDuration_millisecond = Convert.ToInt32(netDurationAsString.Substring(8, 2));

                    decimal netDuration = (decimal)(new TimeSpan(0, netDuration_hour, netDuration_minute, netDuration_second, netDuration_millisecond).TotalSeconds);
                    cdr.NetDurationInSeconds = netDuration;
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

        #region Radius

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Radius(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            var cdrs = new List<dynamic>();
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Ogero_Radius_CDR");

            int batchSize = 0;

            string fileName = ImportedData.Name;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("Ogero_Radius_CDR");

            System.IO.StreamReader sr = ImportedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string cdrLine = sr.ReadLine();
                if (string.IsNullOrEmpty(cdrLine))
                    continue;

                try
                {


                    cdrLine = cdrLine.Replace("\"", "");
                    batchSize++;
                    dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;

                    string[] fields = cdrLine.Split(',');
                    cdr.ComputerName = fields[0];
                    cdr.ServiceName = fields[1];

                    int? intValue = default(int?);
                    //date
                    cdr.RecordDate = DateTime.ParseExact(fields[2], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    cdr.RecordTime = string.IsNullOrEmpty(fields[3]) ? null : new Time(fields[3]);

                    //number

                    cdr.PacketType = string.IsNullOrEmpty(fields[4]) ? intValue : int.Parse(fields[4]);

                    cdr.UserName = fields[5];
                    cdr.FullyQualifiedUserName = fields[6];
                    cdr.CalledStationID = fields[7];
                    cdr.CallingStationID = fields[8];
                    cdr.CallbackNumber = fields[9];
                    cdr.FramedIPAddress = fields[10];
                    cdr.NASIdentifier = fields[11];
                    cdr.NASIPAddress = fields[12];
                    cdr.NASPort = fields[13];
                    cdr.ClientVendor = fields[14];
                    cdr.ClientIPAddress = fields[15];
                    cdr.ClientFriendlyName = fields[16];

                    //date
                    cdr.EventTimestamp = string.IsNullOrEmpty(fields[17]) ? null : new Time(fields[17]);

                    cdr.PortLimit = fields[18];
                    //number
                    cdr.NASPortType = string.IsNullOrEmpty(fields[19]) ? intValue : int.Parse(fields[19]);

                    cdr.ConnectInfo = fields[20];

                    //number
                    cdr.FramedProtocol = string.IsNullOrEmpty(fields[21]) ? intValue : int.Parse(fields[21]);
                    cdr.ServiceType = string.IsNullOrEmpty(fields[22]) ? intValue : int.Parse(fields[22]);
                    cdr.AuthenticationType = string.IsNullOrEmpty(fields[23]) ? intValue : int.Parse(fields[23]);


                    cdr.NPPolicyName = fields[24];

                    //number
                    cdr.ReasonCode = string.IsNullOrEmpty(fields[25]) ? intValue : int.Parse(fields[25]);


                    cdr.Class = fields[26];

                    //number
                    cdr.SessionTimeout = string.IsNullOrEmpty(fields[27]) ? intValue : int.Parse(fields[27]);
                    cdr.IdleTimeout = string.IsNullOrEmpty(fields[28]) ? intValue : int.Parse(fields[28]);
                    cdr.TerminationAction = string.IsNullOrEmpty(fields[29]) ? intValue : int.Parse(fields[29]);

                    cdr.EAPFriendlyName = fields[30];

                    //number
                    cdr.AcctStatusType = string.IsNullOrEmpty(fields[31]) ? intValue : int.Parse(fields[31]);
                    cdr.AcctDelayTime = string.IsNullOrEmpty(fields[32]) ? intValue : int.Parse(fields[32]);
                    cdr.AcctInputOctets = string.IsNullOrEmpty(fields[33]) ? default(long?) : long.Parse(fields[33]);
                    cdr.AcctOutputOctets = string.IsNullOrEmpty(fields[34]) ? default(long?) : long.Parse(fields[34]);

                    cdr.AcctSessionId = fields[35];

                    //number
                    cdr.AcctAuthentic = string.IsNullOrEmpty(fields[36]) ? intValue : int.Parse(fields[36]);
                    cdr.AcctSessionTime = string.IsNullOrEmpty(fields[37]) ? intValue : int.Parse(fields[37]);
                    cdr.AcctInputPackets = string.IsNullOrEmpty(fields[38]) ? intValue : int.Parse(fields[38]);
                    cdr.AcctOutputPackets = string.IsNullOrEmpty(fields[39]) ? intValue : int.Parse(fields[39]);
                    cdr.AcctTerminateCause = string.IsNullOrEmpty(fields[40]) ? intValue : int.Parse(fields[40]);

                    cdr.AcctMultiSsnID = fields[41];

                    //number
                    cdr.AcctLinkCount = string.IsNullOrEmpty(fields[42]) ? intValue : int.Parse(fields[42]);
                    cdr.AcctInterimInterval = string.IsNullOrEmpty(fields[43]) ? intValue : int.Parse(fields[43]);
                    cdr.TunnelType = string.IsNullOrEmpty(fields[44]) ? intValue : int.Parse(fields[44]);
                    cdr.TunnelMediumType = string.IsNullOrEmpty(fields[45]) ? intValue : int.Parse(fields[45]);

                    cdr.TunnelClientEndpt = fields[46];
                    cdr.TunnelServerEndpt = fields[47];
                    cdr.AcctTunnelConnection = fields[48];
                    cdr.TunnelPvtGroupID = fields[49];
                    cdr.TunnelAssignmentID = fields[50];

                    //number
                    cdr.TunnelPreference = string.IsNullOrEmpty(fields[51]) ? intValue : int.Parse(fields[51]);
                    cdr.MSAcctAuthType = string.IsNullOrEmpty(fields[52]) ? intValue : int.Parse(fields[52]);
                    cdr.MSAcctEAPType = string.IsNullOrEmpty(fields[53]) ? intValue : int.Parse(fields[53]);

                    cdr.MSRASVersion = fields[54];

                    //mumber
                    cdr.MSRASVendor = string.IsNullOrEmpty(fields[55]) ? intValue : int.Parse(fields[55]);


                    cdr.MSCHAPError = fields[56];
                    cdr.MSCHAPDomain = fields[57];
                    //number
                    cdr.MSMPPEEncryptionTypes = string.IsNullOrEmpty(fields[58]) ? intValue : int.Parse(fields[58]);
                    cdr.MSMPPEEncryptionPolicy = string.IsNullOrEmpty(fields[59].Trim()) ? intValue : int.Parse(fields[59]);

                    cdr.FileName = fileName;

                    cdrs.Add(cdr);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }


            long startingId;
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, batchSize, out startingId);

            foreach (var item in cdrs)
                item.Id = startingId++;

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of CDRs", "Ogero_Radius_CDR");
            mappedBatches.Add("CookedCDRsStorage", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;


            //Vanrise.Integration.Entities.MappedBatchItemsToEnqueue mappedBatchesNew = new Vanrise.Integration.Entities.MappedBatchItemsToEnqueue();

            //Vanrise.Integration.Entities.MappingOutput result = Mediation.Runtime.DSMappers.MapCDR_File_Radius(new Guid("125f9d3f-52ea-431d-bf0d-74c2380aa261"), data, mappedBatchesNew);

            //LogVerbose("Finished");
            //return result;
        }

        #endregion

        private static void LogVerbose(string Message)
        {
            Console.WriteLine(Message);
        }
    }
}