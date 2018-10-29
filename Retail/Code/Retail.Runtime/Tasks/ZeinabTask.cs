using System;
using System.Data;
using Vanrise.Runtime;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.Caching.Runtime;
using Vanrise.Runtime.Entities;
using System.Collections.Generic;
using Vanrise.Integration.Mappers;
using Vanrise.Integration.Entities;

namespace Retail.Runtime.Tasks
{
    public class ZeinabTask : ITask
    {
        public static Vanrise.Integration.Entities.MappingOutput GetCDR(Guid dataSourceId, IImportedData data, Vanrise.Integration.Entities.MappedBatchItemsToEnqueue mappedBatches)
        {

            var cdrs = new List<dynamic>();
            var dataRecordTypeManager = new Vanrise.GenericData.Business.DataRecordTypeManager();
            Type cdrRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType("RA_INTL_CDR");

            int batchSize = 50000;
            long startingId;
            var dataRecordVanriseType = new Vanrise.GenericData.Entities.DataRecordVanriseType("RA_INTL_CDR");

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));

            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(dataRecordVanriseType, batchSize, out startingId);
            IDataReader reader = importedData.Reader;

            int rowCount = 0;
            long currentCDRId = startingId;
            while (reader.Read())
            {
                dynamic cdr = Activator.CreateInstance(cdrRuntimeType) as dynamic;
                cdr.IDOnSwitch = reader["IDonSwitch"] as string;
                cdr.ID = currentCDRId;
                cdr.DataSource = dataSourceId;
                cdr.OperatorID = 432189;
                cdr.AttemptDateTime = Utils.GetReaderValue<DateTime>(reader, "AttemptDateTime");
                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectDateTime");
                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
                cdr.DurationInSeconds = Utils.GetReaderValue<Decimal>(reader, "DurationInSeconds");
                cdr.CDPN = reader["CDPN"] as string;
                cdr.CGPN = reader["CGPN"] as string;
                cdr.Trunk = reader["IN_TRUNK"] as string;
                cdr.CauseFromReleaseCode = reader["CAUSE_FROM_RELEASE_CODE"] as string;
                cdr.CauseToReleaseCode = reader["CAUSE_TO_RELEASE_CODE"] as string;
                cdr.IP = reader["IN_IP"] as string;
                cdr.CallType = 1;

                cdrs.Add(cdr);
                importedData.LastImportedId = reader["ID"];
                currentCDRId++;
                rowCount++;
                if (rowCount == batchSize)
                    break;
            }
            if (cdrs.Count > 0)
            {
                var batch = Vanrise.GenericData.QueueActivators.DataRecordBatch.CreateBatchFromRecords(cdrs, "#RECORDSCOUNT# of Raw CDRs", "RA_INTL_CDR");
                mappedBatches.Add("Distribute Raw CDRs Stage", batch);
            }
            else
                importedData.IsEmpty = true;

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput
            {
                Result = Vanrise.Integration.Entities.MappingResult.Valid
            };
            // LogVerbose("Finished");

            return result;
        }
        public void Execute()
        {
            var runtimeServices = new List<RuntimeService>();

            BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpService);

            BPRegulatorRuntimeService bpRegulatorRuntimeService = new BPRegulatorRuntimeService { Interval = new TimeSpan(0, 0, 2) };
            runtimeServices.Add(bpRegulatorRuntimeService);

            RuntimeHost host = new RuntimeHost(runtimeServices);
            host.Start();
            Console.ReadKey();

        }
    }
}
