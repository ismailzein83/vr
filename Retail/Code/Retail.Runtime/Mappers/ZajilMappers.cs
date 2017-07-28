using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.Runtime.Mappers
{
    public class ZajilMappers
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
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("CDR");
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("CDR");
            int rowCount = 0;
            var currentItemCount = 53;
            System.IO.StreamReader sr = importedData.StreamReader;
            while (!sr.EndOfStream)
            {
                string currentLine = sr.ReadLine();
                if (string.IsNullOrEmpty(currentLine))
                    continue;

                string[] rowData = currentLine.Replace("\"", string.Empty).Split(',');


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
                    cdr.AttemptDateTime = DateTime.ParseExact(attemptTime, "yyyyMMddHHmmss:fff", System.Globalization.CultureInfo.InvariantCulture);

                    cdr.DisconnectReason = rowData[4];
                    cdr.CallProgressState = rowData[5];
                    cdr.Account = rowData[6];
                    cdr.OriginatorId = rowData[7];
                    cdr.OriginatorNumber = rowData[8];
                    cdr.TerminatorId = rowData[11];
                    cdr.TerminatorNumber = rowData[12];
                    cdr.TransferredCall_Id = rowData[20];
                    cdr.IncomingGwId = rowData[15];
                    cdr.OutgoingGwId = rowData[16];
                    cdr.OriginalDialedNumber = rowData[10];
                    cdr.OriginatorFromNumber = rowData[9];
                    cdr.DurationInSeconds = (decimal)(cdr.ConnectDateTime != null ? (cdr.DisconnectDateTime - cdr.ConnectDateTime).TotalSeconds : 0);
                    cdr.FileName = importedData.Name;
                    cdr.DataSource = dataSourceId;
                    cdrs.Add(cdr);
                    rowCount++;
                }
            }

            if (cdrs.Count > 0)
            {
                long startingId;
                Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, rowCount, out startingId);
                long currentCDRId = startingId;

                foreach (var cdr in cdrs)
                {
                    cdr.ID = currentCDRId;
                    currentCDRId++;
                }

                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
            }
            else
                importedData.IsEmpty = true;

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            return result;

        }

    }
}