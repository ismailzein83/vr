using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Mediation.Generic.Business
{
    public class FileCustomParser
    {
        public char Spliter { get; set; }
        public Vanrise.Integration.Entities.MappingOutput GetResult(object data, Vanrise.Integration.Entities.MappedBatchItemsToEnqueue mappedBatches)
        {


            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("ParsedCDR");

            System.IO.StreamReader sr = ImportedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                if (string.IsNullOrEmpty(currentLine))
                    continue;
                string[] rowData = currentLine.Split(Spliter);
                dynamic cdr = Activator.CreateInstance(mediationCDRRuntimeType) as dynamic;
                cdr.IdOnSwitch = long.Parse(rowData[2].Trim('"'));
                cdr.SessionId = rowData[13].Split('@')[0].Trim('"');
                cdr.Identifier = rowData[1].Trim('"');
                //cdr.AttemptDateTime = DateTime.ParseExact(rowData[3], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                cdr.ConnectDateTime = DateTime.ParseExact(rowData[36].Trim('"'), "yyyyMMddHHmmss:fff", System.Globalization.CultureInfo.InvariantCulture);
                cdr.DisconnectDateTime = DateTime.ParseExact(rowData[3].Trim('"'), "yyyyMMddHHmmss:fff", System.Globalization.CultureInfo.InvariantCulture);

                cdr.CGPN = rowData[6].Trim('"');
                cdr.CDPN = rowData[8].Trim('"');

                cdrs.Add(cdr);

            }
            MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs");
            mappedBatches.Add("Mediation Store Batch", batch);
            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;
        }
    }
}
