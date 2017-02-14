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


        public Vanrise.Integration.Entities.MappingOutput ImportingVoiceEDR_File(object localData)
        {
            //Retail.Runtime.Mappers.RingoMapper mapper = new Retail.Runtime.Mappers.RingoMapper();
            //return mapper.ImportingVoiceEDR_File(data);

            LogVerbose("Started");
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(localData));
            var voiceEDRs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();

            Type voiceEDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("VoiceEDR");

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
                    creationDate = DateTime.ParseExact(rowData[1].Trim(new char[] { ',' }), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                    continue;
                }

                else if (rowData.Length != currentItemCount)
                    continue;

                dynamic edr = Activator.CreateInstance(voiceEDRRuntimeType) as dynamic;
                edr.IdCDR = long.Parse(rowData[0]);
                edr.StartDate = DateTime.ParseExact(rowData[2], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                string parentIdCDR = rowData[23];
                edr.ParentIdCDR = !string.IsNullOrEmpty(parentIdCDR) ? long.Parse(parentIdCDR) : default(long?);

                edr.TrafficType = rowData[3];
                edr.TypeCalled = rowData[4];
                edr.DirectionTraffic = rowData[5];
                edr.Calling = rowData[6];
                edr.Called = rowData[7];
                edr.RedirectingNumber = rowData[8];
                edr.TypeNet = rowData[9];
                edr.SourceOperator = rowData[10];
                edr.DestinationOperator = rowData[11];
                edr.SourceArea = rowData[12];
                edr.DestinationArea = rowData[13];

                string duration = rowData[14];
                edr.Duration = !string.IsNullOrEmpty(duration) ? decimal.Parse(duration) : default(decimal?);

                edr.DurationUnit = rowData[15];

                string balance = rowData[17];
                edr.Balance = !string.IsNullOrEmpty(balance) ? decimal.Parse(balance) : default(decimal?);

                string amount = rowData[19];
                edr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal?);

                edr.TypeConsumed = rowData[20];
                edr.Bag = rowData[18];
                edr.PricePlan = rowData[21];
                edr.Promotion = rowData[22];
                edr.FileName = rowData[25];

                edr.FileDate = DateTime.ParseExact(rowData[26], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                edr.CreationDate = creationDate;

                voiceEDRs.Add(edr);
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(voiceEDRs, "#RECORDSCOUNT# of Raw EDRs");
            mappedBatches.Add("Voice Transformation", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public Vanrise.Integration.Entities.MappingOutput ImportingSmsEDR_File(object localData)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(localData));
            var ringoSmsEDRs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type ringoMessageEDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("RingoMessageEDR");


            var currentItemCount = 15;
            System.IO.StreamReader sr = ImportedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                if (string.IsNullOrEmpty(currentLine))
                    continue;

                string[] rowData = currentLine.Split('|');


                if (rowData.Length != currentItemCount)
                    continue;

                dynamic edr = Activator.CreateInstance(ringoMessageEDRRuntimeType) as dynamic;
                edr.Sender = rowData[0];
                edr.Recipient = rowData[1];
                edr.SenderNetwork = rowData[2];
                edr.RecipientNetwork = rowData[3];
                edr.MSISDN = rowData[4];
                edr.RecipientRequestCode = rowData[5];
                edr.MessageType = int.Parse(rowData[6]);
                edr.NovercaFileName = rowData[7];
                edr.MessageDate = DateTime.ParseExact(rowData[8], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                edr.ACKMessageFileName = rowData[9];
                edr.ACKMessageDate = DateTime.ParseExact(rowData[10], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                edr.StateRequest = int.Parse(rowData[11]);
                edr.FlagCredit = string.IsNullOrEmpty(rowData[12]) ? 0 : int.Parse(rowData[12]);
                edr.TransferredCredit = string.IsNullOrEmpty(rowData[13]) ? 0 : int.Parse(rowData[13]);
                edr.FlagRequestCreditTransfer = string.IsNullOrEmpty(rowData[14]) ? 0 : int.Parse(rowData[14]);
                edr.FileName = ImportedData.Name;
                ringoSmsEDRs.Add(edr);
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(ringoSmsEDRs, "#RECORDSCOUNT# of Raw EDRs");
            mappedBatches.Add("Ringo SMS EDR Transformation", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;
        }

        public Vanrise.Integration.Entities.MappingOutput ImportingGprsEDR_File()
        {
            LogVerbose("Started");
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));

            var dataEDRs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type gprsEDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("GprsEDR");

            var currentItemCount = 21;
            var headerText = "H";

            DateTime creationDate = default(DateTime);
            System.IO.StreamReader sr = ImportedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                if (string.IsNullOrEmpty(currentLine))
                    continue;

                string[] rowData = currentLine.Replace("\"", string.Empty).Split(';');

                if (rowData.Length == 2 && rowData[0] == headerText)
                {
                    creationDate = DateTime.ParseExact(rowData[1], "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                    continue;
                }

                else if (rowData.Length != currentItemCount)
                    continue;

                dynamic edr = Activator.CreateInstance(gprsEDRRuntimeType) as dynamic;
                edr.StartDate = DateTime.ParseExact(rowData[2], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                edr.TypeGprs = rowData[4];
                edr.Calling = rowData[5];
                edr.Zone = rowData[6];

                string bill = rowData[7];
                edr.Bill = !string.IsNullOrEmpty(bill) ? int.Parse(bill) : default(int?);

                string credit = rowData[9];
                edr.Credit = !string.IsNullOrEmpty(credit) ? decimal.Parse(credit) : default(decimal?);

                edr.TrafficType = rowData[3];
                edr.Unit = rowData[8];

                string balance = rowData[10];
                edr.Balance = !string.IsNullOrEmpty(balance) ? decimal.Parse(balance) : default(decimal?);

                edr.Bag = rowData[11];

                string amount = rowData[12];
                edr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal?);

                edr.TypeConsumed = rowData[13];
                edr.PricePlan = rowData[14];
                edr.Promotion = rowData[15];
                edr.AccessPointName = rowData[16];
                //cdr.ParentIdCDR
                edr.IdCDR = long.Parse(rowData[0]);

                string idCdrGprs = rowData[17];
                edr.IdCdrGprs = !string.IsNullOrEmpty(idCdrGprs) ? long.Parse(idCdrGprs) : default(long?);


                edr.FileName = rowData[19];
                edr.FileDate = DateTime.ParseExact(rowData[20], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                edr.CreationDate = creationDate;


                dataEDRs.Add(edr);
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(dataEDRs, "#RECORDSCOUNT# of Raw EDRs");
            mappedBatches.Add("Gprs EDR Storage Stage", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public Vanrise.Integration.Entities.MappingOutput ImportingMmsEDR_File()
        {
            LogVerbose("Started");
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var mmsEDRs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type messageEDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("MessageEDR");


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

                dynamic edr = Activator.CreateInstance(messageEDRRuntimeType) as dynamic;
                edr.IdCDR = long.Parse(rowData[0]);
                edr.StartDate = DateTime.ParseExact(rowData[2], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);



                edr.TrafficType = rowData[3];
                edr.TypeMessage = rowData[4];
                edr.DirectionTraffic = rowData[5];
                edr.Calling = rowData[6];
                edr.Called = rowData[7];
                edr.TypeNet = rowData[8];
                edr.SourceOperator = rowData[9];
                edr.DestinationOperator = rowData[10];
                edr.SourceArea = rowData[11];
                edr.DestinationArea = rowData[12];

                string bill = rowData[13];
                edr.Bill = !string.IsNullOrEmpty(bill) ? int.Parse(bill) : default(int?);

                edr.Unit = rowData[14];

                string credit = rowData[15];
                edr.Credit = !string.IsNullOrEmpty(credit) ? decimal.Parse(credit) : default(decimal?);

                string balance = rowData[16];
                edr.Balance = !string.IsNullOrEmpty(balance) ? decimal.Parse(balance) : default(decimal?);

                edr.Bag = rowData[17];

                string amount = rowData[18];
                edr.Amount = !string.IsNullOrEmpty(amount) ? decimal.Parse(amount) : default(decimal?);

                edr.TypeConsumed = rowData[19];
                edr.PricePlan = rowData[20];
                edr.Promotion = rowData[21];

                string parentIdCDR = rowData[24];
                edr.ParentIdCDR = !string.IsNullOrEmpty(parentIdCDR) ? long.Parse(parentIdCDR) : default(long?);

                edr.FileName = rowData[25];
                edr.FileDate = DateTime.ParseExact(rowData[26], "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                edr.CreationDate = creationDate;

                mmsEDRs.Add(edr);
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(mmsEDRs, "#RECORDSCOUNT# of Raw EDRs");
            mappedBatches.Add("Message EDR Storage Stage", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }
        public Vanrise.Integration.Entities.MappingOutput ImportingZajilCDRs(object importedData)
        {
            //Retail.Runtime.Mappers.RingoMapper mapper = new Retail.Runtime.Mappers.RingoMapper();
            //return mapper.ImportingZajilCDRs(data);

            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();

            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("CDR");
            int rowCount = 0;
            var currentItemCount = 53;
            System.IO.StreamReader sr = ImportedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                if (string.IsNullOrEmpty(currentLine))
                    continue;

                string[] rowData = currentLine.Split(',');


                if (rowData.Length != currentItemCount)
                    continue;

                string recordType = rowData[1];
                if (recordType == "STOP_D" || recordType == "STOP_N")
                {
                    dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                    cdr.Call_Id = rowData[13];
                    string connectTime = rowData[38];
                    string disconnectTime = rowData[3];
                    string attemptTime = rowData[36];
                    if (!string.IsNullOrEmpty(connectTime))
                        cdr.ConnectDateTime = DateTime.ParseExact(connectTime, "yyyyMMddHHmmss:fff", System.Globalization.CultureInfo.InvariantCulture);
                    cdr.DisconnectDateTime = DateTime.ParseExact(disconnectTime, "yyyyMMddHHmmss:fff", System.Globalization.CultureInfo.InvariantCulture);
                    cdr.AttemptDateTime = DateTime.ParseExact(connectTime, "yyyyMMddHHmmss:fff", System.Globalization.CultureInfo.InvariantCulture);

                    cdr.DisconnectReason = rowData[4];
                    cdr.CallProgressState = rowData[5];
                    cdr.Account = rowData[6];
                    cdr.OriginatorId = rowData[7];
                    cdr.OriginatorNumber = rowData[8];
                    cdr.TerminatorId = rowData[11];
                    cdr.TerminatorNumber = rowData[12];
                    cdr.TransferredCall_Id = rowData[20];
                    cdr.DurationInSeconds = (decimal)(cdr.ConnectDateTime != null ? (cdr.DisconnectDateTime - cdr.ConnectDateTime).TotalSeconds : 0);
                    cdrs.Add(cdr);
                    rowCount++;
                }
            }

            long startingId;
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(cdrRuntimeType, rowCount, out startingId);
            long currentCDRId = startingId;

            foreach (var cdr in cdrs)
            {
                cdr.ID = currentCDRId;
                currentCDRId++;
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs");
            mappedBatches.Add("CDR Storage Stage", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;
        }

    }
}
