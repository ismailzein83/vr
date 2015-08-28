using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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


            //Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch();
            //batch.CDRs = new List<Vanrise.Fzero.CDRImport.Entities.CDR>();

            //Vanrise.Integration.Entities.DBReaderImportedData ImportedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));
            //IDataReader reader = ImportedData.Reader;
            //string index = ImportedData.LastImportedId;

            //while (reader.Read())
            //{
            //    Vanrise.Fzero.CDRImport.Entities.CDR normalCDR = new Vanrise.Fzero.CDRImport.Entities.CDR();

            //    normalCDR.CallType = Utils.GetReaderValue<int>(reader, "Call_Type");
            //    normalCDR.BTSId = Utils.GetReaderValue<int?>(reader, "BTS_Id");
            //    normalCDR.ConnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "ConnectDateTime");
            //    normalCDR.Id = (int)reader["Id"];
            //    normalCDR.IMSI = reader["IMSI"] as string;
            //    normalCDR.DurationInSeconds = Utils.GetReaderValue<Decimal?>(reader, "DurationInSeconds");
            //    normalCDR.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
            //    normalCDR.CallClass = reader["Call_Class"] as string;
            //    normalCDR.IsOnNet = Utils.GetReaderValue<Byte?>(reader, "IsOnNet");
            //    normalCDR.SubType = reader["Sub_Type"] as string;
            //    normalCDR.IMEI = reader["IMEI"] as string;
            //    normalCDR.CellId = reader["Cell_Id"] as string;
            //    normalCDR.SwitchRecordId = Utils.GetReaderValue<int?>(reader, "SwitchRecordId");
            //    normalCDR.UpVolume = Utils.GetReaderValue<Decimal?>(reader, "Up_Volume");
            //    normalCDR.DownVolume = Utils.GetReaderValue<Decimal?>(reader, "Down_Volume");
            //    normalCDR.CellLatitude = Utils.GetReaderValue<Decimal?>(reader, "Cell_Latitude");
            //    normalCDR.CellLongitude = Utils.GetReaderValue<Decimal?>(reader, "Cell_Longitude");
            //    normalCDR.InTrunk = reader["In_Trunk"] as string;
            //    normalCDR.OutTrunk = reader["Out_Trunk"] as string;
            //    normalCDR.ServiceType = Utils.GetReaderValue<int>(reader, "Service_Type");
            //    normalCDR.ServiceVASName = reader["Service_VAS_Name"] as string;
            //    normalCDR.Destination = reader["Destination"] as string;
            //    normalCDR.MSISDN = reader["MSISDN"] as string;

            //    index = normalCDR.Id.ToString();

            //    batch.CDRs.Add(normalCDR);
            //}

            //ImportedData.LastImportedId = index;

            //mappedBatches.Add("CDR Import", batch);

            //Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            //result.Result = Vanrise.Integration.Entities.MappingResult.Valid;

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
            

            //WF f = new WF();
            //f.ShowDialog();
            //Console.ReadKey();
            //return;
        }
    }
}
