using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.Runtime
{
    public class CustomParser
    {
        public char Spliter { get; set; }
        public Vanrise.Integration.Entities.MappingOutput GetResult(object data, Vanrise.Integration.Entities.MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
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
                edr.SenderNetwork = edr.Recipient = rowData[2];
                edr.RecipientNetwork = rowData[3];
                edr.MSISDN = rowData[4];
                edr.RecipientRequestCode = rowData[5];
                edr.MessageType = int.Parse(rowData[6]);
                edr.FileName = rowData[7];
                edr.MessageDate = DateTime.ParseExact(rowData[8], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                edr.ACKMessageFileName = rowData[9];
                edr.ACKMessageDate = DateTime.ParseExact(rowData[10], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                edr.StateRequest = int.Parse(rowData[11]);
                edr.FlagCredit = string.IsNullOrEmpty(rowData[12]) ? 0 : int.Parse(rowData[12]);
                edr.TransferredCredit = string.IsNullOrEmpty(rowData[13]) ? 0 : int.Parse(rowData[13]);
                edr.FlagRequestCreditTransfer = string.IsNullOrEmpty(rowData[14]) ? 0 : int.Parse(rowData[14]);

                ringoSmsEDRs.Add(edr);
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(ringoSmsEDRs, "#RECORDSCOUNT# of Raw EDRs");
            mappedBatches.Add("Ringo Message EDR Storage Stage", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;
        }

    }
}
