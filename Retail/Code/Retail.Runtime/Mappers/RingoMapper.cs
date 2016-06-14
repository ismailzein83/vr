using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Vanrise.Integration.Entities;
using Vanrise.Integration.Mappers;

namespace Retail.Runtime.Mappers
{
    public class RingoMapper
    {
        StreamReaderImportedData data;

        private void LogVerbose(string Message)
        {
            Console.WriteLine(Message);
        }

        private class mappedBatches
        {
            public static void Add(string activatorName, object batch)
            {
            }
        }


        public Vanrise.Integration.Entities.MappingOutput ImportingVoiceCDR_File()
        {
            LogVerbose("Started");
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var voiceCDRs = new List<dynamic>();
            //var smsCDRs = new List<dynamic>();
            //var dataCDRs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();

            Type voiceCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("VoiceCDR");
            //Type smsCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("SmsCDR");
            //Type dataCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("DataCDR");

            var currentItemCount = 27;
            var headerText = "H";

            DateTime creationDate = default(DateTime);
            System.IO.StreamReader sr = ImportedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                if (string.IsNullOrEmpty(currentLine))
                    continue;

                string[] rowData = currentLine.Split(';');

                if (rowData.Length == 2 && rowData[0] == headerText)
                {
                    creationDate = DateTime.ParseExact(rowData[1], "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                    continue;
                }

                else if (rowData.Length != currentItemCount)
                    continue;

                dynamic cdr = Activator.CreateInstance(voiceCDRRuntimeType) as dynamic;
                cdr.IdCDR = long.Parse(rowData[0]);
                cdr.StartDate = DateTime.ParseExact(rowData[2], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                string parentIdCDR = rowData[23];
                cdr.ParentIdCDR = !string.IsNullOrEmpty(parentIdCDR) ? long.Parse(parentIdCDR) : default(long);

                cdr.TrafficType = rowData[3];
                cdr.DirectionTraffic = rowData[5];
                cdr.Calling = rowData[6];
                cdr.Called = rowData[7];
                cdr.RedirectingNumber = rowData[8];
                cdr.TypeNet = rowData[9];
                cdr.SourceOperator = rowData[10];
                cdr.DestinationOperator = rowData[11];
                cdr.SourceArea = rowData[12];
                cdr.DestinationArea = rowData[13];

                string duration = rowData[14];
                cdr.Duration = !string.IsNullOrEmpty(duration) ? decimal.Parse(duration) : default(decimal);

                cdr.DurationUnit = rowData[15];

                string amount = rowData[19];
                cdr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal);
                cdr.TypeConsumed = rowData[20];
                cdr.Bag = rowData[18];
                cdr.PricePlan = rowData[21];
                cdr.Promotion = rowData[22];
                cdr.FileName = rowData[25];

                cdr.FileDate = DateTime.ParseExact(rowData[26], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                cdr.CreationDate = creationDate;

                voiceCDRs.Add(cdr);
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(voiceCDRs, "#RECORDSCOUNT# of Raw CDRs");
            mappedBatches.Add("Voice CDR Storage Stage", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }
    }
}
