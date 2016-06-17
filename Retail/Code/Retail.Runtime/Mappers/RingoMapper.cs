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

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();

            Type voiceCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("VoiceCDR");

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
                cdr.ParentIdCDR = !string.IsNullOrEmpty(parentIdCDR) ? long.Parse(parentIdCDR) : default(long?);

                cdr.TrafficType = rowData[3];
                cdr.TypeCalled = rowData[4];
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
                cdr.Duration = !string.IsNullOrEmpty(duration) ? decimal.Parse(duration) : default(decimal?);

                cdr.DurationUnit = rowData[15];

                string balance = rowData[17];
                cdr.Balance = !string.IsNullOrEmpty(balance) ? decimal.Parse(balance) : default(decimal?);

                string amount = rowData[19];
                cdr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal?);

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

        public Vanrise.Integration.Entities.MappingOutput ImportingSmsCDR_File()
        {
            LogVerbose("Started");
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var smsCDRs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type messageCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("MessageCDR");


            var currentItemCount = 26;
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

                dynamic cdr = Activator.CreateInstance(messageCDRRuntimeType) as dynamic;
                cdr.IdCDR = long.Parse(rowData[0]);
                cdr.StartDate = DateTime.ParseExact(rowData[2], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                string parentIdCDR = rowData[22];
                cdr.ParentIdCDR = !string.IsNullOrEmpty(parentIdCDR) ? long.Parse(parentIdCDR) : default(long?);

                cdr.TrafficType = rowData[3];
                cdr.TypeMessage = rowData[4];
                cdr.DirectionTraffic = rowData[5];
                cdr.Calling = rowData[6];
                cdr.Called = rowData[7];
                cdr.TypeNet = rowData[8];
                cdr.SourceOperator = rowData[9];
                cdr.DestinationOperator = rowData[10];
                cdr.SourceArea = rowData[11];
                cdr.DestinationArea = rowData[12];

                string bill = rowData[13];
                cdr.Bill = !string.IsNullOrEmpty(bill) ? int.Parse(bill) : default(int?);

                string credit = rowData[15];
                cdr.Credit = !string.IsNullOrEmpty(credit) ? decimal.Parse(credit) : default(decimal?);

                cdr.Unit = rowData[14];

                string balance = rowData[16];
                cdr.Balance = !string.IsNullOrEmpty(balance) ? decimal.Parse(balance) : default(decimal?);

                cdr.Bag = rowData[17];

                string amount = rowData[18];
                cdr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal?);

                cdr.TypeConsumed = rowData[19];
                cdr.PricePlan = rowData[20];
                cdr.Promotion = rowData[21];

                cdr.FileName = rowData[24];
                cdr.FileDate = DateTime.ParseExact(rowData[25], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                cdr.CreationDate = creationDate;

                smsCDRs.Add(cdr);
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(smsCDRs, "#RECORDSCOUNT# of Raw CDRs");
            mappedBatches.Add("Message CDR Storage Stage", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public Vanrise.Integration.Entities.MappingOutput ImportingGprsCDR_File()
        {
            LogVerbose("Started");
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));

            var dataCDRs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type gprsCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("GprsCDR");

            var currentItemCount = 21;
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

                dynamic cdr = Activator.CreateInstance(gprsCDRRuntimeType) as dynamic;
                cdr.StartDate = DateTime.ParseExact(rowData[2], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                cdr.TypeGprs = rowData[4];
                cdr.Calling = rowData[5];
                cdr.Zone = rowData[6];

                string bill = rowData[7];
                cdr.Bill = !string.IsNullOrEmpty(bill) ? int.Parse(bill) : default(int?);

                string credit = rowData[9];
                cdr.Credit = !string.IsNullOrEmpty(credit) ? decimal.Parse(credit) : default(decimal?);

                cdr.TrafficType = rowData[3];
                cdr.Unit = rowData[8];

                string balance = rowData[10];
                cdr.Balance = !string.IsNullOrEmpty(balance) ? decimal.Parse(balance) : default(decimal?);

                cdr.Bag = rowData[11];

                string amount = rowData[12];
                cdr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal?);

                cdr.TypeConsumed = rowData[13];
                cdr.PricePlan = rowData[14];
                cdr.Promotion = rowData[15];
                cdr.AccessPointName = rowData[16];
                //cdr.ParentIdCDR
                cdr.IdCDR = long.Parse(rowData[0]);

                string idCdrGprs = rowData[17];
                cdr.IdCdrGprs = !string.IsNullOrEmpty(idCdrGprs) ? long.Parse(idCdrGprs) : default(long?);


                cdr.FileName = rowData[19];
                cdr.FileDate = DateTime.ParseExact(rowData[20], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                cdr.CreationDate = creationDate;


                dataCDRs.Add(cdr);
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(dataCDRs, "#RECORDSCOUNT# of Raw CDRs");
            mappedBatches.Add("Gprs CDR Storage Stage", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public Vanrise.Integration.Entities.MappingOutput ImportingMmsCDR_File()
        {
            LogVerbose("Started");
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var mmsCDRs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type messageCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("MessageCDR");


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

                dynamic cdr = Activator.CreateInstance(messageCDRRuntimeType) as dynamic;
                cdr.IdCDR = long.Parse(rowData[0]);
                cdr.StartDate = DateTime.ParseExact(rowData[2], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                

                cdr.TrafficType = rowData[3];
                cdr.TypeMessage = rowData[4];
                cdr.DirectionTraffic = rowData[5];
                cdr.Calling = rowData[6];
                cdr.Called = rowData[7];
                cdr.TypeNet = rowData[8];
                cdr.SourceOperator = rowData[9];
                cdr.DestinationOperator = rowData[10];
                cdr.SourceArea = rowData[11];
                cdr.DestinationArea = rowData[12];

                string bill = rowData[13];
                cdr.Bill = !string.IsNullOrEmpty(bill) ? int.Parse(bill) : default(int?);

                cdr.Unit = rowData[14];

                string credit = rowData[15];
                cdr.Credit = !string.IsNullOrEmpty(credit) ? decimal.Parse(credit) : default(decimal?);

                string balance = rowData[16];
                cdr.Balance = !string.IsNullOrEmpty(balance) ? decimal.Parse(balance) : default(decimal?);

                cdr.Bag = rowData[17];

                string amount = rowData[18];
                cdr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal?);

                cdr.TypeConsumed = rowData[19];
                cdr.PricePlan = rowData[20];
                cdr.Promotion = rowData[21];

                string parentIdCDR = rowData[24];
                cdr.ParentIdCDR = !string.IsNullOrEmpty(parentIdCDR) ? long.Parse(parentIdCDR) : default(long?);

                cdr.FileName = rowData[25];
                cdr.FileDate = DateTime.ParseExact(rowData[26], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                cdr.CreationDate = creationDate;

                mmsCDRs.Add(cdr);
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(mmsCDRs, "#RECORDSCOUNT# of Raw CDRs");
            mappedBatches.Add("Message CDR Storage Stage", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }
    }
}
