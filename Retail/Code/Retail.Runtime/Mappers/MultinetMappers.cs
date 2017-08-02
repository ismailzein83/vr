using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.Runtime.Mappers
{
    public class MultinetMappers
    {
        StreamReaderImportedData data;
        Guid dataSourceId;

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


        public Vanrise.Integration.Entities.MappingOutput MapCDR_File_TelesV2()
        {
            Vanrise.Integration.Entities.StreamReaderImportedData importedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));
            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type mediationCDRRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("ParsedCDR");

            System.IO.StreamReader sr = importedData.StreamReader;
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
                cdr.TC_CALLINDICATOR = rowData[14].Trim('"');
                cdr.FileName = importedData.Name;
                DateTime? attemptDateTime = default(DateTime?);
                if (!string.IsNullOrEmpty(rowData[36].Trim('"')))
                    attemptDateTime = (DateTime?)(DateTime.ParseExact(rowData[36].Trim('"'), "yyyyMMddHHmmss:fff", System.Globalization.CultureInfo.InvariantCulture));
                cdr.TC_SESSIONINITIATIONTIME = attemptDateTime;
                cdr.TC_SEQUENCENUMBER = rowData[2].Trim('"');
                cdrs.Add(cdr);
            }

            if (cdrs.Count > 0)
            {
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "ParsedCDR");
                mappedBatches.Add("Teles Mediation Stage", batch);
            }
            else
                importedData.IsEmpty = true;

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;
        }
    }
}