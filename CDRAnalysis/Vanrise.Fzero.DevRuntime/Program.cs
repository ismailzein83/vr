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

            //string conStr = "Data Source=192.168.110.185;Initial Catalog=CDRAnalysisMobile_Research;User ID=development;Password=dev!123;";
            //string query = "SELECT [MSISDN],[IMSI],[ConnectDateTime],[Destination],[DurationInSeconds],[DisconnectDateTime],[Call_Class],[IsOnNet],[Call_Type],[Sub_Type],[IMEI],[BTS_Id],[Cell_Id],[SwitchRecordId],[Up_Volume],[Down_Volume],[Cell_Latitude],[Cell_Longitude],[In_Trunk],[Out_Trunk],[Service_Type],[Service_VAS_Name],[Id] FROM [CDRAnalysisMobile_Research].[dbo].[NormalCDRAf] where Id > 120585";

            //IDataReader reader = null;

            //SqlConnection conn = new SqlConnection(conStr);
            
            //conn.Open();
            //SqlCommand cmd = new SqlCommand(query, conn);
            //reader = cmd.ExecuteReader();
            
            

            //Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch();
            //batch.CDRs = new List<Vanrise.Fzero.CDRImport.Entities.CDR>();
            
            //Vanrise.Integration.Entities.DBReaderImportedData ImportedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));
            
            //IDataReader reader = ImportedData.Reader;
            //string index = ImportedData.LastImportedId;
            
            //while (reader.Read())
            //{

            //    Vanrise.Fzero.CDRImport.Entities.CDR normalCDR = new Vanrise.Fzero.CDRImport.Entities.CDR();

            //    normalCDR.CallType = GetReaderValue<Vanrise.Fzero.CDRImport.Entities.CallTypeEnum>(reader, "Call_Type");
            //    normalCDR.BTSId = GetReaderValue<int?>(reader, "BTS_Id");
            //    normalCDR.ConnectDateTime = GetReaderValue<DateTime?>(reader, "ConnectDateTime");
            //    normalCDR.Id = (int)reader["Id"];
            //    normalCDR.IMSI = reader["IMSI"] as string;
            //    normalCDR.DurationInSeconds = GetReaderValue<Decimal?>(reader, "DurationInSeconds");
            //    normalCDR.DisconnectDateTime = GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
            //    normalCDR.CallClass = reader["Call_Class"] as string;
            //    normalCDR.IsOnNet = GetReaderValue<short?>(reader, "IsOnNet");
            //    normalCDR.SubType = reader["Sub_Type"] as string;
            //    normalCDR.IMEI = reader["IMEI"] as string;
            //    normalCDR.CellId = reader["Cell_Id"] as string;
            //    normalCDR.UpVolume = GetReaderValue<Decimal?>(reader, "Up_Volume");
            //    normalCDR.DownVolume = GetReaderValue<Decimal?>(reader, "Down_Volume");
            //    normalCDR.CellLatitude = GetReaderValue<Decimal?>(reader, "Cell_Latitude");
            //    normalCDR.CellLongitude = GetReaderValue<Decimal?>(reader, "Cell_Longitude");
            //    normalCDR.InTrunk = reader["In_Trunk"] as string;
            //    normalCDR.OutTrunk = reader["Out_Trunk"] as string;
            //    normalCDR.ServiceType = GetReaderValue<int?>(reader, "Service_Type");
            //    normalCDR.ServiceVASName = reader["Service_VAS_Name"] as string;
            //    normalCDR.Destination = reader["Destination"] as string;
            //    normalCDR.MSISDN = reader["MSISDN"] as string;

            //    index = normalCDR.Id.ToString();

            //    Console.WriteLine(index);
                

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

        public static T GetReaderValue<T>(IDataReader reader, string fieldName)
        {
            return reader[fieldName] != DBNull.Value ? (T)reader[fieldName] : default(T);
        } 
    }
}
