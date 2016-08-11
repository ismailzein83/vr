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
            Type ringoMessageEDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("RingoEventEDR");

            DateTime creationDate = default(DateTime);
            var currentItemCount = 5;
            System.IO.StreamReader sr = ImportedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                if (string.IsNullOrEmpty(currentLine))
                    continue;

                string[] rowData = currentLine.Split(';');


                if (rowData.Length != currentItemCount)
                    continue;

                dynamic edr = Activator.CreateInstance(ringoMessageEDRRuntimeType) as dynamic;
                edr.MSISDN = rowData[0];
                edr.EventIdMvno = int.Parse(rowData[1]);
                edr.EventId = int.Parse(rowData[2]);
                edr.CreatedDate = creationDate;
                edr.Event = rowData[3];
                edr.Token = rowData[4];

                ringoSmsEDRs.Add(edr);
            }

            var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(ringoSmsEDRs, "#RECORDSCOUNT# of Raw EDRs");
            mappedBatches.Add("Ringo Event EDR Storage Stage", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;
        }

    }
}
