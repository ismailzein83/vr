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
using Vanrise.Fzero.CDRImport.Business.ExecutionFlows;

namespace Vanrise.Fzero.DevRuntime
{
    class Program
    {
        static void Main(string[] args)
        {

            string GetNormalCDRFlow = AllFlows.GetNormalCDRFlow();
            string GetNormalCDRNormalizationFlow = AllFlows.GetNormalCDRNormalizationFlow();
            string GetStagingCDRFlow = AllFlows.GetStagingCDRFlow();
            string GetStagingCDRNormalizationFlow = AllFlows.GetStagingCDRNormalizationFlow();






            //PSTN.BusinessEntity.Business.SwitchManager switchManager = new PSTN.BusinessEntity.Business.SwitchManager();
            //LogVerbose("1");

            //PSTN.BusinessEntity.Entities.Switch currentSwitch;
            //LogVerbose("2");
            //currentSwitch = switchManager.GetSwitchByDataSourceID(dataSourceId);
            //LogVerbose("3");
            //int? SwitchId = null;
            //TimeSpan TimeOffset = new TimeSpan();
            //LogVerbose("4");

            //if (currentSwitch != null)
            //{
            //    SwitchId = currentSwitch.ID;
            //    TimeOffset = currentSwitch.TimeOffset;
            //}

            //else
            //    LogVerbose("No Switch associated to this Datasource");

            //LogVerbose("5");

            //LogVerbose("Started");
            //Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch();
            //LogVerbose("6");
            //batch.StagingCDRs = new List<Vanrise.Fzero.CDRImport.Entities.StagingCDR>();
            //LogVerbose("7");
            //Vanrise.Integration.Entities.DBReaderImportedData ImportedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));
            //LogVerbose("8");

            //IDataReader reader = ImportedData.Reader;
            //LogVerbose("9");
            //string index = ImportedData.LastImportedId;
            //LogVerbose("10");

            //while (reader.Read())
            //{
            //    LogVerbose("11");
            //    Vanrise.Fzero.CDRImport.Entities.StagingCDR stagingCDR = new Vanrise.Fzero.CDRImport.Entities.StagingCDR();
            //    LogVerbose("12");
            //    stagingCDR.CDPN = reader["CDPN"] as string;
            //    LogVerbose("13");
            //    stagingCDR.CGPN = reader["CGPN"] as string;
            //    LogVerbose("14");
            //    stagingCDR.ConnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "ConnectDateTime");
            //    LogVerbose("15");
            //    if (stagingCDR.ConnectDateTime != null)
            //        stagingCDR.ConnectDateTime = stagingCDR.ConnectDateTime.Value.Add(TimeOffset);
            //    LogVerbose("16");
            //    stagingCDR.DurationInSeconds = Utils.GetReaderValue<Decimal?>(reader, "DurationInSeconds");
            //    LogVerbose("17");
            //    stagingCDR.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");

            //    if (stagingCDR.DisconnectDateTime != null)
            //        stagingCDR.DisconnectDateTime = stagingCDR.DisconnectDateTime.Value.Add(TimeOffset);
            //    LogVerbose("18");

            //    stagingCDR.InTrunkSymbol = reader["InTrunkSymbol"] as string;
            //    stagingCDR.OutTrunkSymbol = reader["OutTrunkSymbol"] as string;
            //    stagingCDR.SwitchID = SwitchId;
            //    index = Utils.GetReaderValue<int?>(reader, "ID").ToString();
            //    batch.StagingCDRs.Add(stagingCDR);
            //    LogVerbose("19");
            //}

            //ImportedData.LastImportedId = index;
            //LogVerbose("20");
            //mappedBatches.Add("Normalize CDRs", batch);
            //LogVerbose("21");
            //Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            //LogVerbose("22");
            //result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            //LogVerbose("23");
            //LogVerbose("Finished");
            //return result;



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
