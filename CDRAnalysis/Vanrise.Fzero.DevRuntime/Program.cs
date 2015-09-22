using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.Runtime;

namespace Vanrise.Fzero.DevRuntime
{
    class Program
    {
        static void Main(string[] args)
        {
            if(ConfigurationManager.AppSettings["IsRuntimeService"] == "true")
            {
                BusinessProcessService bpService = new BusinessProcessService() { Interval = new TimeSpan(0, 0, 2) };
                QueueActivationService queueActivationService = new QueueActivationService() { Interval = new TimeSpan(0, 0, 2) };
                SchedulerService schedulerService = new SchedulerService() { Interval = new TimeSpan(0, 0, 5) };

                var runtimeServices = new List<RuntimeService>();

                runtimeServices.Add(queueActivationService);

                runtimeServices.Add(bpService);

                runtimeServices.Add(schedulerService);

                RuntimeHost host = new RuntimeHost(runtimeServices);
                host.Start();
                Console.ReadKey();






                //LogVerbose("Started");
                //Vanrise.Fzero.CDRImport.Entities.StagingImportedCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.StagingImportedCDRBatch();
                //batch.StagingCDRs = new List<Vanrise.Fzero.CDRImport.Entities.StagingCDR>();
                //Vanrise.Integration.Entities.DBReaderImportedData ImportedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));

                //IDataReader reader = ImportedData.Reader;
                //string index = ImportedData.LastImportedId;

                //while (reader.Read())
                //{

                //    Vanrise.Fzero.CDRImport.Entities.StagingCDR stagingCDR = new Vanrise.Fzero.CDRImport.Entities.StagingCDR();

                //    stagingCDR.CDPN = reader["CDPN"] as string;
                //    stagingCDR.CGPN = reader["CGPN"] as string;
                //    stagingCDR.ConnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "ConnectDateTime");
                //    stagingCDR.DurationInSeconds = Utils.GetReaderValue<Decimal?>(reader, "DurationInSeconds");
                //    stagingCDR.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
                //    stagingCDR.InTrunkSymbol = reader["InTrunkSymbol"] as string;
                //    stagingCDR.OutTrunkSymbol = reader["OutTrunkSymbol"] as string;
                //    stagingCDR.SwitchID = 1;
                //    index = Utils.GetReaderValue<DateTime?>(reader, "ID").ToString();

                //    batch.StagingCDRs.Add(stagingCDR);
                //}

                //ImportedData.LastImportedId = index;

                //mappedBatches.Add("Staging CDR Import", batch);


                //Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
                //result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
                //LogVerbose("Finished");
                //return result;












            }
            else
            {
                MainForm f = new MainForm();
                f.ShowDialog();
                Console.ReadKey();
                return;
            }
          
        }

    }
}
