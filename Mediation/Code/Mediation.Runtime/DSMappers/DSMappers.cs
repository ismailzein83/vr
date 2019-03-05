using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Integration.Entities;

namespace Mediation.Runtime
{
    public static class DSMappers
    {
        #region Ogero

        public static MappingOutput MapCDR_File_Ogero_Alcatel_ICX(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = false };
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("30553F01-CE03-4D29-9BF5-80D0D06DFA34"), options, (parsedBatch) =>
            {
                MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Alcatel Parsed CDRs", parsedBatch.RecordType);
                switch (parsedBatch.RecordType)
                {
                    case "Ogero_ICX_Alcatel_CDR":
                        mappedBatches.Add("CDRTransformationStage", batch);
                        break;
                    default: break;
                }
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_Ogero_Ericsson_WHS_Binary(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = true };
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("6f27b54c-90f3-4332-8437-1adffdb8ed2d"), options, (parsedBatch) =>
            {
                MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                switch (parsedBatch.RecordType)
                {
                    case "Ogero_WHS_Ericsson_CDR":
                        mappedBatches.Add("CDRTransformationStage", batch);
                        break;
                    default: break;
                }

            });
            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_Ogero_EricssonLocal_WHS_Txt(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Ogero_WHS_Ericsson_CDR");

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
                        case "08": lengthToRead = 41; break;
                        case "01": lengthToRead = 354; break;
                        default: lengthToRead = 115; break;
                    }

                    if (currentLine.Length < lengthToRead)
                        break;

                    switch (recordType)
                    {
                        case "07": break;
                        case "08": break;

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
                                int year;
                                if (!int.TryParse(dateForStartCharging_01.Substring(0, 2), out year))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Year value", dateForStartCharging_01));

                                year += 2000;

                                int month;
                                if (!int.TryParse(dateForStartCharging_01.Substring(2, 2), out month))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Month value", dateForStartCharging_01));

                                int day;
                                if (!int.TryParse(dateForStartCharging_01.Substring(4, 2), out day))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Day value", dateForStartCharging_01));

                                cdr_01.DateForStartCharging = new DateTime(year, month, day);
                            }

                            string timeForStartCharging_01 = cdrAsString_01.Substring(52, 6);
                            if (!string.IsNullOrWhiteSpace(timeForStartCharging_01))
                            {
                                int hour;
                                if (!int.TryParse(timeForStartCharging_01.Substring(0, 2), out hour))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Hour value", timeForStartCharging_01));

                                int minute;
                                if (!int.TryParse(timeForStartCharging_01.Substring(2, 2), out minute))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Minute value", timeForStartCharging_01));

                                int second;
                                if (!int.TryParse(timeForStartCharging_01.Substring(4, 2), out second))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Second value", timeForStartCharging_01));

                                cdr_01.TimeForStartCharging = new Time(hour, minute, second, 0);
                            }

                            string chargeableDuration_01 = cdrAsString_01.Substring(58, 6);
                            if (!string.IsNullOrWhiteSpace(chargeableDuration_01))
                            {
                                int hour;
                                if (!int.TryParse(chargeableDuration_01.Substring(0, 2), out hour))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Hour value", chargeableDuration_01));

                                int minute;
                                if (!int.TryParse(chargeableDuration_01.Substring(2, 2), out minute))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Minute value", chargeableDuration_01));

                                int second;
                                if (!int.TryParse(chargeableDuration_01.Substring(4, 2), out second))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Second value", chargeableDuration_01));

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
                                int year;
                                if (!int.TryParse(dateForStartCharging.Substring(0, 2), out year))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Year value", dateForStartCharging));

                                year += 2000;

                                int month;
                                if (!int.TryParse(dateForStartCharging.Substring(2, 2), out month))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Month value", dateForStartCharging));

                                int day;
                                if (!int.TryParse(dateForStartCharging.Substring(4, 2), out day))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Day value", dateForStartCharging));

                                cdr.DateForStartCharging = new DateTime(year, month, day);
                            }

                            string timeForStartCharging = cdrAsString.Substring(55, 6);
                            if (!string.IsNullOrWhiteSpace(timeForStartCharging))
                            {
                                int hour;
                                if (!int.TryParse(timeForStartCharging.Substring(0, 2), out hour))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Hour value", timeForStartCharging));

                                int minute;
                                if (!int.TryParse(timeForStartCharging.Substring(2, 2), out minute))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Minute value", timeForStartCharging));

                                int second;
                                if (!int.TryParse(timeForStartCharging.Substring(4, 2), out second))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Second value", timeForStartCharging));

                                cdr.TimeForStartCharging = new Time(hour, minute, second, 0);
                            }

                            string chargeableDuration = cdrAsString.Substring(61, 6);
                            if (!string.IsNullOrWhiteSpace(chargeableDuration))
                            {
                                int hour;
                                if (!int.TryParse(chargeableDuration.Substring(0, 2), out hour))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Hour value", chargeableDuration));

                                int minute;
                                if (!int.TryParse(chargeableDuration.Substring(2, 2), out minute))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Minute value", chargeableDuration));

                                int second;
                                if (!int.TryParse(chargeableDuration.Substring(4, 2), out second))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Second value", chargeableDuration));

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
                //long startingId;
                //var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("WHS_Ericsson_CDR");
                //Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, cdrs.Count, out startingId);

                //foreach (var item in cdrs)
                //    item.Id = startingId++;

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "Ogero_WHS_Ericsson_CDR");
                mappedBatches.Add("CDRTransformationStage", batch);
            }

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            return result;
        }
         
        public static MappingOutput MapCDR_File_Ogero_EricssonInternational_WHS_Txt(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Ogero_WHS_Ericsson_CDR");

            var duplicatedISDNs = new Dictionary<string, Tuple<Object, List<int>>>();

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
                        case "08": lengthToRead = 41; break;
                        case "01": lengthToRead = 354; break;
                        default: lengthToRead = 115; break;
                    }

                    if (currentLine.Length < lengthToRead)
                        break;

                    switch (recordType)
                    {
                        case "07": break;
                        case "08": break;

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
                                int year;
                                if (!int.TryParse(dateForStartCharging_01.Substring(0, 2), out year))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Year value", dateForStartCharging_01));

                                year += 2000;

                                int month;
                                if (!int.TryParse(dateForStartCharging_01.Substring(2, 2), out month))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Month value", dateForStartCharging_01));

                                int day;
                                if (!int.TryParse(dateForStartCharging_01.Substring(4, 2), out day))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Day value", dateForStartCharging_01));

                                cdr_01.DateForStartCharging = new DateTime(year, month, day);
                            }

                            string timeForStartCharging_01 = cdrAsString_01.Substring(52, 6);
                            if (!string.IsNullOrWhiteSpace(timeForStartCharging_01))
                            {
                                int hour;
                                if (!int.TryParse(timeForStartCharging_01.Substring(0, 2), out hour))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Hour value", timeForStartCharging_01));

                                int minute;
                                if (!int.TryParse(timeForStartCharging_01.Substring(2, 2), out minute))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Minute value", timeForStartCharging_01));

                                int second;
                                if (!int.TryParse(timeForStartCharging_01.Substring(4, 2), out second))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Second value", timeForStartCharging_01));

                                cdr_01.TimeForStartCharging = new Time(hour, minute, second, 0);
                            }

                            string chargeableDuration_01 = cdrAsString_01.Substring(58, 6);
                            if (!string.IsNullOrWhiteSpace(chargeableDuration_01))
                            {
                                int hour;
                                if (!int.TryParse(chargeableDuration_01.Substring(0, 2), out hour))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Hour value", chargeableDuration_01));

                                int minute;
                                if (!int.TryParse(chargeableDuration_01.Substring(2, 2), out minute))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Minute value", chargeableDuration_01));

                                int second;
                                if (!int.TryParse(chargeableDuration_01.Substring(4, 2), out second))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Second value", chargeableDuration_01));

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

                            //if (cdr_01.BNumber != null && cdr_01.BNumber.StartsWith("990"))
                            //{ 
                            string serializedCdr_01 = Serializer.Serialize(cdr_01);

                            Tuple<object, List<int>> duplicatedISDN_01;

                            if (!duplicatedISDNs.TryGetValue(serializedCdr_01, out duplicatedISDN_01))
                            {
                                duplicatedISDN_01 = Tuple.Create(cdr_01 as object, new List<int>() { 0 });
                                duplicatedISDNs.Add(serializedCdr_01, duplicatedISDN_01);
                            }
                            duplicatedISDN_01.Item2[0]++;

                            break;
                        //}

                        //cdrs.Add(cdr_01);
                        //break;

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
                                int year;
                                if (!int.TryParse(dateForStartCharging.Substring(0, 2), out year))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Year value", dateForStartCharging));

                                year += 2000;

                                int month;
                                if (!int.TryParse(dateForStartCharging.Substring(2, 2), out month))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Month value", dateForStartCharging));

                                int day;
                                if (!int.TryParse(dateForStartCharging.Substring(4, 2), out day))
                                    throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Day value", dateForStartCharging));

                                cdr.DateForStartCharging = new DateTime(year, month, day);
                            }

                            string timeForStartCharging = cdrAsString.Substring(55, 6);
                            if (!string.IsNullOrWhiteSpace(timeForStartCharging))
                            {
                                int hour;
                                if (!int.TryParse(timeForStartCharging.Substring(0, 2), out hour))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Hour value", timeForStartCharging));

                                int minute;
                                if (!int.TryParse(timeForStartCharging.Substring(2, 2), out minute))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Minute value", timeForStartCharging));

                                int second;
                                if (!int.TryParse(timeForStartCharging.Substring(4, 2), out second))
                                    throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Second value", timeForStartCharging));

                                cdr.TimeForStartCharging = new Time(hour, minute, second, 0);
                            }

                            string chargeableDuration = cdrAsString.Substring(61, 6);
                            if (!string.IsNullOrWhiteSpace(chargeableDuration))
                            {
                                int hour;
                                if (!int.TryParse(chargeableDuration.Substring(0, 2), out hour))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Hour value", chargeableDuration));

                                int minute;
                                if (!int.TryParse(chargeableDuration.Substring(2, 2), out minute))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Minute value", chargeableDuration));

                                int second;
                                if (!int.TryParse(chargeableDuration.Substring(4, 2), out second))
                                    throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Second value", chargeableDuration));

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

                            //if (cdr.BNumber != null && cdr.BNumber.StartsWith("990"))
                            //{ 
                            string serializedCdr = Serializer.Serialize(cdr);

                            Tuple<object, List<int>> duplicatedISDN;

                            if (!duplicatedISDNs.TryGetValue(serializedCdr, out duplicatedISDN))
                            {
                                duplicatedISDN = Tuple.Create(cdr as object, new List<int>() { 0 });
                                duplicatedISDNs.Add(serializedCdr, duplicatedISDN);
                            }
                            duplicatedISDN.Item2[0]++;

                            break;
                            //}

                            //cdrs.Add(cdr);
                            //break;
                    }

                    currentLine = currentLine.Remove(0, lengthToRead);
                }
            }

            foreach (var duplicatedISDN in duplicatedISDNs.Values)
            {
                Tuple<object, List<int>> cdr = duplicatedISDN;
                (cdr.Item1 as dynamic).ChargeableDuration *= cdr.Item2[0];
                cdrs.Add(cdr.Item1);
            }

            if (cdrs.Count > 0)
            {
                //long startingId;
                //var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("WHS_Ericsson_CDR");
                //Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, cdrs.Count, out startingId);

                //foreach (var item in cdrs)
                //    item.Id = startingId++;

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "Ogero_WHS_Ericsson_CDR");
                mappedBatches.Add("CDRTransformationStage", batch);
            }

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            return result;
        }

        public static MappingOutput MapCDR_File_Ogero_HuaweiIMS_WHS(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = false };
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("504a12e9-61d2-4e31-b193-1d43749dc055"), options, (parsedBatch) =>
            {
                MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Huawei IMS Parsed CDRs", parsedBatch.RecordType);
                switch (parsedBatch.RecordType)
                {
                    case "Ogero_HuaweiIMS_CDR":
                        mappedBatches.Add("CDRTransformationStage", batch);
                        break;
                    default: break;
                }
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_Ogero_HuaweiMGCF_WHS(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = false };
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("D6896CC8-22EF-4FAA-BEF4-4644EE5323F9"), options, (parsedBatch) =>
            {
                MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Huawei MGCF Parsed CDRs", parsedBatch.RecordType);
                switch (parsedBatch.RecordType)
                {
                    case "Ogero_HuaweiMGCF_CDR":
                        mappedBatches.Add("CDRTransformationStage", batch);
                        break;
                    default: break;
                }
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_Ogero_HuaweiEPC_WHS(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = false };
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("95037A1B-EF0C-4F2B-8B22-F51EE65ACD45"), options, (parsedBatch) =>
            {
                MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Huawei EPC Parsed CDRs", parsedBatch.RecordType);
                switch (parsedBatch.RecordType)
                {
                    case "Ogero_HuaweiEPC_CDR":
                        mappedBatches.Add("CDRTransformationStage", batch);
                        break;
                    default: break;
                }
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_Ogero_NokiaSiemens_ICX(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = false };
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("202c8508-a24c-4664-b769-be71c86fcd75"), options, (parsedBatch) =>
            {
                MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Nokia Siemens Parsed CDRs", parsedBatch.RecordType);
                switch (parsedBatch.RecordType)
                {
                    case "Ogero_ICX_NokiaSiemens_CDR":
                        mappedBatches.Add("CDRTransformationStage", batch);
                        break;
                    default: break;
                }
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_Ogero_Radius(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            var cdrs = new List<dynamic>();
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Ogero_Radius_CDR");

            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("Ogero_Radius_CDR");

            int batchSize = 0;
            int lineNumber = 0;
            string fileName = importedData.Name;

            int? intValue = default(int?);
            long? longValue = default(long?);

            System.IO.StreamReader sr = importedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string cdrLine = sr.ReadLine();
                lineNumber++;
                if (string.IsNullOrEmpty(cdrLine))
                    continue;

                //cdrLine = cdrLine.Replace("\"", "");
                batchSize++;
                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                string[] fields = System.Text.RegularExpressions.Regex.Split(cdrLine, @",(?=(?:[^""]*""[^""]*"")*[^""]*$)");
                for (int i = 0; i < fields.Length; i++)
                    fields[i] = fields[i].Replace("\"", "");

                cdr.ComputerName = fields[0];
                cdr.ServiceName = fields[1];

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
                cdr.AcctInputPackets = string.IsNullOrEmpty(fields[38]) ? longValue : long.Parse(fields[38]);
                cdr.AcctOutputPackets = string.IsNullOrEmpty(fields[39]) ? longValue : long.Parse(fields[39]);
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

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of CDRs", "Ogero_Radius_CDR");
            mappedBatches.Add("CookedCDRsStorage", batch);

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");

            return result;
        }

        #endregion

        #region MobileAnalysis 

        public static MappingOutput MapCDR_File_MobileAnalysis_Ericsson(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("BA810002-0B4D-4563-9A0D-EE228D69A1A6"), (parsedBatch) =>
            {
                switch (parsedBatch.RecordType)
                {
                    case "MobileAnalysis_CDR":
                        List<dynamic> multiLegRecords = new List<dynamic>();
                        List<dynamic> normalRecords = new List<dynamic>();

                        foreach (dynamic record in parsedBatch.Records)
                        {
                            if (record.IntermediateRecordNumber != null)
                            {
                                record.SessionId = record.RecordType + "_" + record.CallingNumber + "_" + record.CalledNumber + "_" + record.SwitchId + "_" + record.GlobalCallReference;
                                multiLegRecords.Add(record);
                            }
                            else
                                normalRecords.Add(record);
                        }

                        MappedBatchItem normalBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(normalRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("MobileTransformationStage", normalBatch);

                        if (multiLegRecords.Count > 0)
                        {
                            MappedBatchItem multiLegBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(multiLegRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                            mappedBatches.Add("EricssonMediationStage", multiLegBatch);
                        }
                        break;

                    case "MobileAnalysis_SMS":
                        MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("SMSTransformationStage", batch);
                        break;
                }
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_MobileAnalysis_Ericsson_GPRS(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("B9648105-8914-4C70-8550-F63D946F5B0C"), (parsedBatch) =>
            {
                MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                mappedBatches.Add("GPRSStoreStage", batch);
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_MobileAnalysis_Huawei(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("3B0C2ED7-CC17-46C0-8F96-697BD185B273"), (parsedBatch) =>
            {
                switch (parsedBatch.RecordType)
                {
                    case "MobileAnalysis_CDR":
                        List<dynamic> multiLegRecords = new List<dynamic>();
                        List<dynamic> normalRecords = new List<dynamic>();
                        foreach (dynamic record in parsedBatch.Records)
                        {
                            if (record.SequenceNumber == null || record.SequenceNumber > 0)
                            {
                                record.SessionId = record.RecordType + "_" + record.CallingNumber + "_" + record.CalledNumber + "_" + record.SwitchId + "_" + record.GlobalCallReference;
                                multiLegRecords.Add(record);
                            }
                            else
                                normalRecords.Add(record);
                        }
                        MappedBatchItem normalBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(normalRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("MobileTransformationStage", normalBatch);

                        if (multiLegRecords.Count > 0)
                        {
                            MappedBatchItem multiLegBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(multiLegRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                            mappedBatches.Add("HuaweiMediationStage", multiLegBatch);
                        }
                        break;

                    case "MobileAnalysis_SMS":
                        MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("SMSTransformationStage", batch);
                        break;
                }
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_MobileAnalysis_Huawei_GPRS(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("16b6af8d-6a15-46a1-9c19-ccfac1ebbdde"), (parsedBatch) =>
            {
                MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                mappedBatches.Add("GPRSStoreStage", batch);
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_MobileAnalysis_Nokia(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));

            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("230bedb5-a3ee-4cbe-802c-dfdaa2a2d438"), (parsedBatch) =>
            {
                var utilityManager = new Vanrise.Common.Business.UtilityManager();
                var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();

                var dataRecordType = dataRecordTypeManager.GetDataRecordType(parsedBatch.RecordType);
                dataRecordType.ThrowIfNull("dataRecordType");
                dataRecordType.Settings.ThrowIfNull("dataRecordType.Settings", dataRecordType.DataRecordTypeId);
                string dataRecordTypeDateTimeField = dataRecordType.Settings.DateTimeField;

                foreach (dynamic record in parsedBatch.Records)
                {
                    DateTime? recordDateTime = record.GetFieldValue(dataRecordTypeDateTimeField);
                    if (utilityManager.CheckIfDefaultOrInvalid(recordDateTime))
                        throw new DataIntegrityValidationException("Invalid dateTime value");
                }

                switch (parsedBatch.RecordType)
                {
                    case "MobileAnalysis_CDR":
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
                            {
                                normalRecords.Add(record);
                            }
                        }

                        MappedBatchItem normalBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(normalRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("MobileTransformationStage", normalBatch);

                        if (multiLegRecords.Count > 0)
                        {
                            MappedBatchItem multiLegBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(multiLegRecords, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                            mappedBatches.Add("NokiaMediationStage", multiLegBatch);
                        }

                        break;

                    case "MobileAnalysis_SMS":
                        MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("SMSTransformationStage", batch);
                        break;
                }
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");

            return result;
        }
          
        #endregion

        #region Mobilis

        public static MappingOutput MapCDR_File_Mobilis_Ericsson_111(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Mobilis_Ericsson_111_CDR");

            System.IO.StreamReader sr = importedData.StreamReader;
            string currentLine = sr.ReadLine();
            int lengthToRead = 111;
            if (!string.IsNullOrEmpty(currentLine))
            {
                currentLine = currentLine.Replace("\0", "");

                while (true)
                {
                    if (currentLine.Length < lengthToRead)
                        break;

                    string cdrAsString = currentLine.Substring(0, lengthToRead);
                    dynamic cdr = Activator.CreateInstance(mediationCDRRuntimeType) as dynamic;

                    cdr.DataSourceId = dataSourceId;
                    cdr.FileName = importedData.Name;

                    cdr.RecordType = cdrAsString.Substring(0, 2);
                    cdr.CauseForOutput = cdrAsString.Substring(2, 1);
                    cdr.RecordNumber = cdrAsString.Substring(3, 2);

                    string aNumber = cdrAsString.Substring(5, 18);
                    if (!string.IsNullOrWhiteSpace(aNumber))
                        cdr.ANumber = aNumber.Trim();

                    string bNumber = cdrAsString.Substring(23, 18);
                    if (!string.IsNullOrWhiteSpace(bNumber))
                        cdr.BNumber = bNumber.Trim();

                    string redirectingNumber = cdrAsString.Substring(41, 20);
                    if (!string.IsNullOrWhiteSpace(redirectingNumber))
                        cdr.RedirectingNumber = redirectingNumber.Trim();

                    string dateForStartCharging = cdrAsString.Substring(61, 6);
                    if (!string.IsNullOrWhiteSpace(dateForStartCharging))
                    {
                        int year;
                        if (!int.TryParse(dateForStartCharging.Substring(0, 2), out year))
                            throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Year value", dateForStartCharging));

                        year += 2000;

                        int month;
                        if (!int.TryParse(dateForStartCharging.Substring(2, 2), out month))
                            throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Month value", dateForStartCharging));

                        int day;
                        if (!int.TryParse(dateForStartCharging.Substring(4, 2), out day))
                            throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Day value", dateForStartCharging));

                        cdr.DateForStartCharging = new DateTime(year, month, day);
                    }

                    string timeForStartCharging = cdrAsString.Substring(67, 6);
                    if (!string.IsNullOrWhiteSpace(timeForStartCharging))
                    {
                        int startHour;
                        if (!int.TryParse(timeForStartCharging.Substring(0, 2), out startHour))
                            throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Hour value", timeForStartCharging));

                        int startMinute;
                        if (!int.TryParse(timeForStartCharging.Substring(2, 2), out startMinute))
                            throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Minute value", timeForStartCharging));

                        int startSecond;
                        if (!int.TryParse(timeForStartCharging.Substring(4, 2), out startSecond))
                            throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Second value", timeForStartCharging));

                        cdr.TimeForStartCharging = new Time(startHour, startMinute, startSecond, 0);
                    }

                    string timeForStopCharging = cdrAsString.Substring(73, 6);
                    if (!string.IsNullOrWhiteSpace(timeForStopCharging))
                    {
                        int stopHour;
                        if (!int.TryParse(timeForStopCharging.Substring(0, 2), out stopHour))
                            throw new Exception(String.Format("TimeForStopCharging '{0}' contains invalid Hour value", timeForStopCharging));

                        int stopMinute;
                        if (!int.TryParse(timeForStopCharging.Substring(2, 2), out stopMinute))
                            throw new Exception(String.Format("TimeForStopCharging '{0}' contains invalid Minute value", timeForStopCharging));

                        int stopSecond;
                        if (!int.TryParse(timeForStopCharging.Substring(4, 2), out stopSecond))
                            throw new Exception(String.Format("TimeForStopCharging '{0}' contains invalid Second value", timeForStopCharging));

                        cdr.TimeForStopCharging = new Time(stopHour, stopMinute, stopSecond, 0);
                    }

                    string chargeableDuration = cdrAsString.Substring(79, 6);
                    if (!string.IsNullOrWhiteSpace(chargeableDuration))
                    {
                        int hour;
                        if (!int.TryParse(chargeableDuration.Substring(0, 2), out hour))
                            throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Hour value", chargeableDuration));

                        int minute;
                        if (!int.TryParse(chargeableDuration.Substring(2, 2), out minute))
                            throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Minute value", chargeableDuration));

                        int second;
                        if (!int.TryParse(chargeableDuration.Substring(4, 2), out second))
                            throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Second value", chargeableDuration));

                        cdr.ChargeableDuration = (int)new TimeSpan(hour, minute, second).TotalSeconds;
                    }

                    cdr.NumberOfMeterPulses = cdrAsString.Substring(85, 6);

                    string outgoingRoute = cdrAsString.Substring(91, 7);
                    if (!string.IsNullOrWhiteSpace(outgoingRoute))
                        cdr.OutgoingRoute = outgoingRoute.Trim();

                    string incomingRoute = cdrAsString.Substring(98, 7);
                    if (!string.IsNullOrWhiteSpace(incomingRoute))
                        cdr.IncomingRoute = incomingRoute.Trim();

                    string aNumberLength = cdrAsString.Substring(105, 2);
                    if (!string.IsNullOrWhiteSpace(aNumberLength))
                        cdr.ANumberLength = aNumberLength.Trim();

                    string bNumberLength = cdrAsString.Substring(107, 2);
                    if (!string.IsNullOrWhiteSpace(bNumberLength))
                        cdr.BNumberLength = bNumberLength.Trim();

                    cdr.OriginatingCode = cdrAsString.Substring(109, 2);

                    cdrs.Add(cdr);
                    currentLine = currentLine.Remove(0, lengthToRead);
                }
            }

            if (cdrs.Count > 0)
            {
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "Mobilis_Ericsson_111_CDR");
                mappedBatches.Add("CDRTransformationStage", batch);
            }

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            return result;
        }

        public static MappingOutput MapCDR_File_Mobilis_Ericsson_102(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Mobilis_Ericsson_102_CDR");

            var lengthToRead = 102;

            System.IO.StreamReader sr = importedData.StreamReader;
            string currentLine = sr.ReadLine();
            if (!string.IsNullOrEmpty(currentLine))
            {
                currentLine = currentLine.Replace("\0", "");

                while (true)
                {
                    if (currentLine.Length < lengthToRead)
                        break;

                    string cdrAsString = currentLine.Substring(0, lengthToRead);

                    dynamic cdr = Activator.CreateInstance(mediationCDRRuntimeType) as dynamic;

                    cdr.DataSourceId = dataSourceId;
                    cdr.FileName = importedData.Name;
                    cdr.RecordType = cdrAsString.Substring(0, 2);
                    cdr.CauseForOutput = cdrAsString.Substring(2, 1);
                    cdr.RecordNumber = cdrAsString.Substring(3, 2);

                    string aNumber = cdrAsString.Substring(5, 15);
                    if (!string.IsNullOrWhiteSpace(aNumber))
                        cdr.ANumber = aNumber.Trim();

                    string bNumber = cdrAsString.Substring(20, 18);
                    if (!string.IsNullOrWhiteSpace(bNumber))
                        cdr.BNumber = bNumber.Trim();

                    string redirectingNumber = cdrAsString.Substring(38, 20);
                    if (!string.IsNullOrWhiteSpace(redirectingNumber))
                        cdr.RedirectingNumber = redirectingNumber.Trim();

                    string dateForStartCharging = cdrAsString.Substring(58, 6);
                    if (!string.IsNullOrWhiteSpace(dateForStartCharging))
                    {
                        int year;
                        if (!int.TryParse(dateForStartCharging.Substring(0, 2), out year))
                            throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Year value", dateForStartCharging));

                        year += 2000;

                        int month;
                        if (!int.TryParse(dateForStartCharging.Substring(2, 2), out month))
                            throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Month value", dateForStartCharging));

                        int day;
                        if (!int.TryParse(dateForStartCharging.Substring(4, 2), out day))
                            throw new Exception(String.Format("DateForStartCharging '{0}' contains invalid Day value", dateForStartCharging));

                        cdr.DateForStartCharging = new DateTime(year, month, day);
                    }

                    string timeForStartCharging = cdrAsString.Substring(64, 6);
                    if (!string.IsNullOrWhiteSpace(timeForStartCharging))
                    {
                        int startHour;
                        if (!int.TryParse(timeForStartCharging.Substring(0, 2), out startHour))
                            throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Hour value", timeForStartCharging));

                        int startMinute;
                        if (!int.TryParse(timeForStartCharging.Substring(2, 2), out startMinute))
                            throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Minute value", timeForStartCharging));

                        int startSecond;
                        if (!int.TryParse(timeForStartCharging.Substring(4, 2), out startSecond))
                            throw new Exception(String.Format("TimeForStartCharging '{0}' contains invalid Second value", timeForStartCharging));

                        cdr.TimeForStartCharging = new Time(startHour, startMinute, startSecond, 0);
                    }

                    string timeForStopCharging = cdrAsString.Substring(70, 6);
                    if (!string.IsNullOrWhiteSpace(timeForStopCharging))
                    {
                        int stopHour;
                        if (!int.TryParse(timeForStopCharging.Substring(0, 2), out stopHour))
                            throw new Exception(String.Format("TimeForStopCharging '{0}' contains invalid Hour value", timeForStopCharging));

                        int stopMinute;
                        if (!int.TryParse(timeForStopCharging.Substring(2, 2), out stopMinute))
                            throw new Exception(String.Format("TimeForStopCharging '{0}' contains invalid Minute value", timeForStopCharging));

                        int stopSecond;
                        if (!int.TryParse(timeForStopCharging.Substring(4, 2), out stopSecond))
                            throw new Exception(String.Format("TimeForStopCharging '{0}' contains invalid Second value", timeForStopCharging));

                        cdr.TimeForStopCharging = new Time(stopHour, stopMinute, stopSecond, 0);
                    }

                    string chargeableDuration = cdrAsString.Substring(76, 6);
                    if (!string.IsNullOrWhiteSpace(chargeableDuration))
                    {
                        int hour;
                        if (!int.TryParse(chargeableDuration.Substring(0, 2), out hour))
                            throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Hour value", chargeableDuration));

                        int minute;
                        if (!int.TryParse(chargeableDuration.Substring(2, 2), out minute))
                            throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Minute value", chargeableDuration));

                        int second;
                        if (!int.TryParse(chargeableDuration.Substring(4, 2), out second))
                            throw new Exception(String.Format("ChargeableDuration '{0}' contains invalid Second value", chargeableDuration));

                        cdr.ChargeableDuration = (int)new TimeSpan(hour, minute, second).TotalSeconds;
                    }

                    cdr.NumberOfMeterPulses = cdrAsString.Substring(82, 6);

                    string outgoingRoute = cdrAsString.Substring(88, 7);
                    if (!string.IsNullOrWhiteSpace(outgoingRoute))
                        cdr.OutgoingRoute = outgoingRoute.Trim();

                    string incomingRoute = cdrAsString.Substring(95, 7);
                    if (!string.IsNullOrWhiteSpace(incomingRoute))
                        cdr.IncomingRoute = incomingRoute.Trim();

                    cdrs.Add(cdr);

                    currentLine = currentLine.Remove(0, lengthToRead);
                }
            }

            if (cdrs.Count > 0)
            {
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "Mobilis_Ericsson_102_CDR");
                mappedBatches.Add("CDRTransformationStage", batch);
            }

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            return result;
        }

        public static MappingOutput MapCDR_File_Mobilis_Ericsson(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("57E3E68E-9403-440D-A67D-CC5896D6BAD5"), (parsedBatch) =>
            {
                switch (parsedBatch.RecordType)
                {
                    case "Mobilis_Ericsson_CDR":
                        MappedBatchItem cdrBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("CDRTransformationStage", cdrBatch);
                        break;

                    case "Mobilis_Ericsson_SMS":
                        MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("SMSTransformationStage", batch);
                        break;
                }
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_Mobilis_Huawei(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions();
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("9B613038-9F64-47C7-B9FA-2F41ABE85286"), options, (parsedBatch) =>
            {
                MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of CDRs", parsedBatch.RecordType);
                switch (parsedBatch.RecordType)
                {
                    case "Mobilis_Huawei_CDR":
                        mappedBatches.Add("CDRTransformationStage", batch);
                        break;
                    default: break;
                }
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }
          
        public static MappingOutput MapCDR_File_Mobilis_SMS(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            var smsList = new List<dynamic>();
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type smsRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Mobilis_SMS");

            int batchSize = 0;

            System.IO.StreamReader sr = importedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string smsLine = sr.ReadLine();
                if (string.IsNullOrEmpty(smsLine))
                    continue;

                batchSize++;
                dynamic sms = Activator.CreateInstance(smsRuntimeType) as dynamic;

                try
                {
                    string[] fields = smsLine.Split(',');

                    sms.DataSourceId = dataSourceId;
                    sms.FileName = importedData.Name;

                    sms.SMID = fields[0];
                    sms.OriginalAddress = fields[1];
                    sms.DestAddr = fields[2];
                    sms.MOMSCAddr = fields[3];
                    sms.ScAddr = fields[4];
                    sms.EsmClass = fields[5];
                    sms.PriorityLevel = fields[6];
                    sms.RD = fields[7];
                    sms.ReplyPath = fields[8];
                    sms.UDHI = fields[9];
                    sms.SRR = fields[10];
                    sms.MR = fields[11];
                    sms.PID = fields[12];
                    sms.DCS = fields[13];

                    string scheduleTimeAsString = fields[14];
                    if (!string.IsNullOrEmpty(scheduleTimeAsString))
                    {
                        DateTime scheduleTime;
                        if (scheduleTimeAsString.Length < 23 || !DateTime.TryParseExact(scheduleTimeAsString.Substring(0, 23), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out scheduleTime))
                            throw new Exception($"Invalid date format for ScheduleTime '{scheduleTimeAsString}'");
                        sms.ScheduleTime = scheduleTime;
                    }

                    string expireTimeAsString = fields[15];
                    if (!string.IsNullOrEmpty(expireTimeAsString))
                    {
                        DateTime expireTime;
                        if (expireTimeAsString.Length < 23 || !DateTime.TryParseExact(expireTimeAsString.Substring(0, 23), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out expireTime))
                            throw new Exception($"Invalid date format for ExpireTime '{expireTimeAsString}'");
                        sms.ExpireTime = expireTime;
                    }

                    sms.DefaultID = fields[16];
                    sms.UDL = fields[17];
                    sms.SMType = fields[18];
                    sms.SMSubmissionResult = fields[19];
                    sms.FCS = fields[20];

                    string writeTimeAsString = fields[21];
                    if (!string.IsNullOrEmpty(writeTimeAsString))
                    {
                        DateTime writeTime = new DateTime();
                        if (writeTimeAsString.Length < 23 || !DateTime.TryParseExact(writeTimeAsString.Substring(0, 23), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out writeTime))
                            throw new Exception($"Invalid date format for WriteTime '{writeTimeAsString}'");
                        else sms.WriteTime = writeTime;
                    }

                    sms.Message = fields[22];
                    sms.ServiceType = fields[23];
                    sms.PPSUser = fields[24];
                    sms.OrgAccount = fields[25];
                    sms.CalledServiceFlag = fields[26];
                    sms.RawOrgAddress = fields[27];
                    sms.RawDestAddress = fields[28];
                    sms.SubmitMultiID = fields[29];
                    sms.OriginalGroup = fields[30];
                    sms.OrgCommandID = fields[31];
                    sms.RawOrgTON = fields[32];
                    sms.RawOrgNPI = fields[33];
                    sms.RawDestTON = fields[34];
                    sms.RawDestNPI = fields[35];
                    sms.MOMSCAddrType = fields[36];
                    sms.MOMSCTON = fields[37];
                    sms.MOMSCNPI = fields[38];
                    sms.DestNetType = fields[39];
                    sms.DestIFType = fields[40];
                    sms.TLVsDataLen = fields[41];
                    sms.TLVsData = fields[42];
                    sms.IFForward = fields[43];
                    sms.UDEncryptType = fields[44];
                    sms.OrgOPID = fields[45];
                    sms.DestOPID = fields[46];
                    sms.QueryOrgOPIDResult = fields[47];
                    sms.QueryDestOPIDResult = fields[48];
                    sms.NetworkErrorCode = fields[49];
                    sms.OrgOCSID = fields[50];
                    sms.DestOCSID = fields[51];
                    sms.DestAccount = fields[52];
                    sms.MessageType = fields[53];
                    sms.SRICallingSCCP = fields[54];
                    sms.MTSubmitCallingSCCP = fields[55];
                    sms.OrgIMSIAddr = fields[56];
                    sms.DestIMSIAddr = fields[57];

                    string mOSubmitTimeAsString = fields[58];
                    if (!string.IsNullOrEmpty(mOSubmitTimeAsString))
                    {
                        DateTime mOSubmitTime = new DateTime();
                        if (mOSubmitTimeAsString.Length < 23 || !DateTime.TryParseExact(mOSubmitTimeAsString.Substring(0, 23), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out mOSubmitTime))
                            throw new Exception($"Invalid date format for MOSubmitTime '{mOSubmitTimeAsString}'");
                        else sms.MOSubmitTime = mOSubmitTime;
                    }

                    sms.BillingIdentification = fields[59];
                    sms.AntiSpammingCheckResult = fields[60];
                    sms.FilterdAVP = fields[61];
                    sms.IsDestNative = fields[62];
                    sms.SMContentKeyword = fields[63];
                    sms.SMMR = fields[64];
                    sms.SMRN = fields[65];
                    sms.SMMN = fields[66];
                    sms.SMSN = fields[67];
                    sms.ServiceControlResultCode = fields[68];
                    sms.RealTimeRated = fields[69];
                    sms.ServiceCtrlResult = fields[70];
                    sms.CgiAddr = fields[71];
                    sms.IMEIAddr = fields[72];
                    sms.LastResort = fields[73];
                    sms.OriginalValidityPeriod = fields[74];

                    string originalSubmissionTimeAsString = fields[75];
                    if (!string.IsNullOrEmpty(originalSubmissionTimeAsString))
                    {
                        DateTime originalSubmissionTime = new DateTime();
                        if (originalSubmissionTimeAsString.Length < 23 || !DateTime.TryParseExact(originalSubmissionTimeAsString.Substring(0, 23), "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out originalSubmissionTime))
                            throw new Exception($"Invalid date format for OriginalSubmissionTime '{mOSubmitTimeAsString}'");
                        else sms.OriginalSubmissionTime = originalSubmissionTime;
                    }

                    sms.SMBufferingForAntiSpam = fields[76];
                    sms.AntiSpoofingResult = fields[77];
                    sms.RealMSCAddr = fields[78];
                    sms.CalledCellID = fields[79];
                    sms.SpecialServiceIndicator = fields[80];

                    smsList.Add(sms);
                }
                catch (Exception ex)
                {
                    failedRecordIdentifiers.Add(batchSize);
                }
            }

            if (smsList.Count > 0)
            {
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(smsList, "#RECORDSCOUNT# of Raw SMSs", "Mobilis_SMS");
                mappedBatches.Add("CDRTransformationStage", batch);
            }

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;

            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_Mobilis_ZTE_WLL(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Mobilis_ZTE_WLL_CDR");

            var lengthToRead = 82;

            System.IO.StreamReader sr = importedData.StreamReader;
            string currentLine = sr.ReadLine();
            if (!string.IsNullOrEmpty(currentLine))
            {
                currentLine = currentLine.Replace("\0", "");

                while (true)
                {
                    if (currentLine.Length < lengthToRead)
                        break;

                    string cdrAsString = currentLine.Substring(0, lengthToRead);

                    dynamic cdr = Activator.CreateInstance(mediationCDRRuntimeType) as dynamic;

                    cdr.DataSourceId = dataSourceId;
                    cdr.FileName = importedData.Name;

                    string lACOfSubscriber = cdrAsString.Substring(0, 5);
                    if (!string.IsNullOrWhiteSpace(lACOfSubscriber))
                        cdr.LACOfSubscriber = lACOfSubscriber.Trim();

                    cdr.CIOfSubscriber = cdrAsString.Substring(5, 5);

                    cdr.TicketType = cdrAsString.Substring(10, 1);

                    string cityCodeOfCallingNumber = cdrAsString.Substring(11, 7);
                    if (!string.IsNullOrWhiteSpace(cityCodeOfCallingNumber))
                        cdr.CityCodeOfCallingNumber = cityCodeOfCallingNumber.Trim();

                    string callingNumber = cdrAsString.Substring(18, 10);
                    if (!string.IsNullOrWhiteSpace(callingNumber))
                        cdr.CallingNumber = callingNumber.Trim();

                    string calldate = cdrAsString.Substring(28, 8);
                    if (!string.IsNullOrWhiteSpace(calldate))
                    {
                        int day;
                        if (!int.TryParse(calldate.Substring(0, 2), out day))
                            throw new Exception(String.Format("CallDate '{0}' contains invalid Day value", calldate));

                        int month;
                        if (!int.TryParse(calldate.Substring(2, 2), out month))
                            throw new Exception(String.Format("CallDate '{0}' contains invalid Month value", calldate));

                        int year;
                        if (!int.TryParse(calldate.Substring(4, 4), out year))
                            throw new Exception(String.Format("CallDate '{0}' contains invalid Year value", calldate));

                        cdr.CallDate = new DateTime(year, month, day);
                    }

                    string callTime = cdrAsString.Substring(36, 6);
                    if (!string.IsNullOrWhiteSpace(callTime))
                    {
                        int callTimeHour;
                        if (!int.TryParse(callTime.Substring(0, 2), out callTimeHour))
                            throw new Exception(String.Format("CallTime '{0}' contains invalid Hour value", callTime));

                        int callTimeMinute;
                        if (!int.TryParse(callTime.Substring(2, 2), out callTimeMinute))
                            throw new Exception(String.Format("CallTime '{0}' contains invalid Minute value", callTime));

                        int callTimeSecond;
                        if (!int.TryParse(callTime.Substring(4, 2), out callTimeSecond))
                            throw new Exception(String.Format("CallTime '{0}' contains invalid Second value", callTime));

                        cdr.CallTime = new Time(callTimeHour, callTimeMinute, callTimeSecond, 0);
                    }

                    string calledNumber = cdrAsString.Substring(42, 18);
                    if (!string.IsNullOrWhiteSpace(calledNumber))
                        cdr.CalledNumber = calledNumber.Trim();

                    string durationAsString = cdrAsString.Substring(60, 6);
                    if (!string.IsNullOrWhiteSpace(durationAsString))
                    {
                        durationAsString = durationAsString.Trim();
                        int duration;
                        if (!int.TryParse(durationAsString, out duration))
                            cdr.Duration = null;
                        else cdr.Duration = duration;
                    }

                    string pulse = cdrAsString.Substring(66, 6);
                    if (!string.IsNullOrWhiteSpace(pulse))
                        cdr.Pulse = pulse.Trim();

                    cdr.CallType = cdrAsString.Substring(72, 1);
                    cdr.SPS800 = cdrAsString.Substring(73, 1);


                    string filler = cdrAsString.Substring(74, 8);
                    if (!string.IsNullOrWhiteSpace(filler))
                        cdr.Filler = filler.Trim();

                    cdrs.Add(cdr);

                    currentLine = currentLine.Remove(0, lengthToRead);
                }
            }

            if (cdrs.Count > 0)
            {
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "Mobilis_ZTE_WLL_CDR");
                mappedBatches.Add("CDRTransformationStage", batch);
            }

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;

            LogVerbose("Finished");
            return result;
        } 
        
        #endregion

        #region Multinet

        public static MappingOutput MapCDR_File_Multinet_Teles(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("ParsedCDR");

            var cdrs = new List<dynamic>();

            try
            {
                System.IO.StreamReader sr = importedData.StreamReader;
                while (!sr.EndOfStream)
                {
                    string currentLine = sr.ReadLine();
                    if (string.IsNullOrEmpty(currentLine))
                        continue;
                    var rowData = System.Text.RegularExpressions.Regex.Split(currentLine, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
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
                    cdr.FileName = importedData.Name;
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
            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            return result;
        } 

        #endregion

        #region Jazz

        public static MappingOutput MapCDR_File_Jazz_Huawei_WithTransitRule(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            var cdrs = new List<dynamic>();
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Jazz_Huawei_CDR");

            int batchSize = 0;

            string fileName = importedData.Name;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("Jazz_Huawei_CDR");

            System.IO.StreamReader sr = importedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string cdrLine = sr.ReadLine();
                if (string.IsNullOrEmpty(cdrLine))
                    continue;

                batchSize++;
                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;

                try
                {
                    string[] fields = cdrLine.Split(',');
                    cdr.DatasourceId = dataSourceId;
                    cdr.IgnoreTransitRule = false;
                    cdr.Direction = fields[0];
                    cdr.SwitchType = fields[1];
                    cdr.InTrunk = fields[2];
                    cdr.OutTrunk = fields[3];
                    cdr.ANum = fields[4];
                    cdr.BNum = fields[5];
                    cdr.ICTRecordType = fields[9];
                    cdr.CauseForTermination = fields[10];

                    string startDateAsString = fields[6];
                    if (!string.IsNullOrEmpty(startDateAsString))
                    {
                        int year = Convert.ToInt32(startDateAsString.Substring(0, 4));
                        int month = Convert.ToInt32(startDateAsString.Substring(4, 2));
                        int day = Convert.ToInt32(startDateAsString.Substring(6, 2));
                        cdr.StartDate = new DateTime(year, month, day);
                    }

                    string startTimeAsString = fields[7];
                    if (!string.IsNullOrEmpty(startTimeAsString))
                    {
                        int hour = Convert.ToInt32(startTimeAsString.Substring(0, 2));
                        int minute = Convert.ToInt32(startTimeAsString.Substring(2, 2));
                        int second = Convert.ToInt32(startTimeAsString.Substring(4, 2));
                        //int millisecond = Convert.ToInt32(startTimeAsString.Substring(6, 2));
                        cdr.StartTime = new Time(hour, minute, second, 0);
                    }

                    string durationInSecondsAsString = fields[8];
                    if (!string.IsNullOrEmpty(durationInSecondsAsString))
                        cdr.DurationInSeconds = decimal.Parse(durationInSecondsAsString);
                    cdr.ReasonForTermination = fields[11];
                    cdr.CallReferenceNumber = fields[12];
                    cdr.DCRCallId = fields[13];
                    string sequenceNumberAsString = fields[14];
                    if (!string.IsNullOrEmpty(sequenceNumberAsString))
                    {
                        cdr.SequenceNumber = long.Parse(sequenceNumberAsString);
                    }

                    cdr.CallTransactionType = fields[15];
                    cdr.ThirdNumber = fields[16];
                    cdr.Rfu1 = fields[17];
                    cdr.Rfu2 = fields[18];
                    cdr.Rfu3 = fields[19];
                    cdr.Rfu4 = fields[20];
                    cdr.Rfu5 = fields[21];

                    cdr.FileName = fileName;

                    cdrs.Add(cdr);
                }
                catch (Exception ex)
                {
                    failedRecordIdentifiers.Add(batchSize);
                }
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of CDRs", "Jazz_Huawei_CDR");
            mappedBatches.Add("CDRTransformationStage", batch);

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static MappingOutput MapCDR_File_Jazz_Huawei_WithoutTransitRule(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            var cdrs = new List<dynamic>();
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("Jazz_Huawei_CDR");

            int batchSize = 0;

            string fileName = importedData.Name;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("Jazz_Huawei_CDR");

            System.IO.StreamReader sr = importedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string cdrLine = sr.ReadLine();
                if (string.IsNullOrEmpty(cdrLine))
                    continue;

                batchSize++;
                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;

                try
                {
                    string[] fields = cdrLine.Split(',');
                    cdr.DatasourceId = dataSourceId;
                    cdr.IgnoreTransitRule = true;
                    cdr.Direction = fields[0];
                    cdr.SwitchType = fields[1];
                    cdr.InTrunk = fields[2];
                    cdr.OutTrunk = fields[3];
                    cdr.ANum = fields[4];
                    cdr.BNum = fields[5];
                    cdr.ICTRecordType = fields[9];
                    cdr.CauseForTermination = fields[10];

                    string startDateAsString = fields[6];
                    if (!string.IsNullOrEmpty(startDateAsString))
                    {
                        int year = Convert.ToInt32(startDateAsString.Substring(0, 4));
                        int month = Convert.ToInt32(startDateAsString.Substring(4, 2));
                        int day = Convert.ToInt32(startDateAsString.Substring(6, 2));
                        cdr.StartDate = new DateTime(year, month, day);
                    }

                    string startTimeAsString = fields[7];
                    if (!string.IsNullOrEmpty(startTimeAsString))
                    {
                        int hour = Convert.ToInt32(startTimeAsString.Substring(0, 2));
                        int minute = Convert.ToInt32(startTimeAsString.Substring(2, 2));
                        int second = Convert.ToInt32(startTimeAsString.Substring(4, 2));
                        //int millisecond = Convert.ToInt32(startTimeAsString.Substring(6, 2));
                        cdr.StartTime = new Time(hour, minute, second, 0);
                    }

                    string durationInSecondsAsString = fields[8];
                    if (!string.IsNullOrEmpty(durationInSecondsAsString))
                        cdr.DurationInSeconds = decimal.Parse(durationInSecondsAsString);
                    cdr.ReasonForTermination = fields[11];
                    cdr.CallReferenceNumber = fields[12];
                    cdr.DCRCallId = fields[13];
                    string sequenceNumberAsString = fields[14];
                    if (!string.IsNullOrEmpty(sequenceNumberAsString))
                    {
                        cdr.SequenceNumber = long.Parse(sequenceNumberAsString);
                    }

                    cdr.CallTransactionType = fields[15];
                    cdr.ThirdNumber = fields[16];
                    cdr.Rfu1 = fields[17];
                    cdr.Rfu2 = fields[18];
                    cdr.Rfu3 = fields[19];
                    cdr.Rfu4 = fields[20];
                    cdr.Rfu5 = fields[21];

                    cdr.FileName = fileName;

                    cdrs.Add(cdr);
                }
                catch (Exception ex)
                {
                    failedRecordIdentifiers.Add(batchSize);
                }
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of CDRs", "Jazz_Huawei_CDR");
            mappedBatches.Add("CDRTransformationStage", batch);

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        #endregion

        #region Namibia

        public static MappingOutput MapCDR_File_Namibia_Huawei(Guid dataSourceId, IImportedData data, MappedBatchItemsToEnqueue mappedBatches, List<Object> failedRecordIdentifiers)
        {
            StreamReaderImportedData importedData = ((StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(importedData.Stream, importedData.Name, dataSourceId, new Guid("e2a77834-86da-42ba-9501-c3eb81f5f60b"), (parsedBatch) =>
            {
                switch (parsedBatch.RecordType)
                {
                    case "Namibia_WHS_CDR":
                        foreach (dynamic record in parsedBatch.Records)
                        {
                            DateTime? connectDateTime = record.GetFieldValue("ConnectDateTime");
                            if (!connectDateTime.HasValue || connectDateTime.Value == DateTime.MinValue)
                            {
                                int duration = record.GetFieldValue("DurationInSeconds");
                                if (duration == 0)
                                {
                                    DateTime? disconnectDateTime = record.GetFieldValue("DisconnectDateTime");
                                    record.SetFieldValue("ConnectDateTime", disconnectDateTime);
                                }
                            }
                        }

                        MappedBatchItem cdrBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("CDR_TransformationStep", cdrBatch);
                        break;

                    case "Namibia_WHS_SMS":
                        MappedBatchItem smsBatch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Parsed CDRs", parsedBatch.RecordType);
                        mappedBatches.Add("SMS_TransformationStep", smsBatch);
                        break;
                }
            });

            MappingOutput result = new MappingOutput();
            result.Result = MappingResult.Valid;
            result.Message = string.Format("Finished importing File {0}", importedData.Name);
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