using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Integration.Entities;

namespace Mediation.Runtime
{
    public static class DSMappers
    {
        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Ericsson_Txt(IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
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

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_Ericsson_Binary(IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = true };
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, new Guid("6f27b54c-90f3-4332-8437-1adffdb8ed2d"), options, (parsedBatch) =>
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

        public static Vanrise.Integration.Entities.MappingOutput MapCDR_File_NokiaSiemens_Binary(IImportedData data, MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.DataParser.Business.ExecuteParserOptions options = new Vanrise.DataParser.Business.ExecuteParserOptions { GenerateIds = true };
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));

            Vanrise.DataParser.Business.ParserHelper.ExecuteParser(ImportedData.Stream, ImportedData.Name, new Guid("202c8508-a24c-4664-b769-be71c86fcd75"), options, (parsedBatch) =>
            {
                Vanrise.Integration.Entities.MappedBatchItem batch = 
                    Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(parsedBatch.Records, "#RECORDSCOUNT# of Nokia Siemens Parsed CDRs", parsedBatch.RecordType);

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

        private static void LogVerbose(string Message)
        {
            Console.WriteLine(Message);
        }
    }
}