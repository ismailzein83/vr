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
            MappedBatchItem batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "ParsedCDR");
            mappedBatches.Add("Mediation Store Batch", batch);
            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;
        }

        public Vanrise.Integration.Entities.MappingOutput GetResult_Version31(object data, Vanrise.Integration.Entities.MappedBatchItemsToEnqueue mappedBatches)
        {
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("ParsedCDR");
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
                    cdr.TC_ORIGINATORNUMBER = rowData[8].Trim('"');
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
                    cdr.TC_SESSIONINITIATIONTIME = DateTime.ParseExact(rowData[36].Trim('"'), "yyyyMMddHHmmss:fff", System.Globalization.CultureInfo.InvariantCulture);
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

    }
}
