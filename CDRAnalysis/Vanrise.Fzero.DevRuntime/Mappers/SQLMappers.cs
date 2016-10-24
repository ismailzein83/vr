using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;



namespace Vanrise.Fzero.DevRuntime.Tasks.Mappers
{
    public class SQLMappers
    {

        static Guid dataSourceId = new Guid("BE08FA12-7978-4774-B0DF-4CC543596A71");
        static DBReaderImportedData data = new DBReaderImportedData();

        public static void FillData()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "Data Source=192.168.110.185;Initial Catalog=CDRAnalysisMobile_WF;User ID=development;Password=dev!123;";
            SqlCommand command = new SqlCommand(
              "SELECT top 100000  [ID] ,[CGPN] ,[CDPN] ,[ConnectDateTime],[DurationInSeconds] ,[DisconnectDateTime] ,[InTrunkSymbol] ,[OutTrunkSymbol] FROM [dbo].[PSTNSampleData]", connection);
            command.Connection = connection;
            connection.Open();
            data.Reader = command.ExecuteReader();
        }

        private static void LogVerbose(string Message)
        {
            Console.WriteLine(Message);
        }


        public class mappedBatches
        {
            public static void Add(string activatorName, object batch)
            {
            }
        }


        public class Utils
        {
            public static T GetReaderValue<T>(IDataReader reader, string fieldName)
            {
                return reader[fieldName] != DBNull.Value ? (T)reader[fieldName] : default(T);
            }
        }





        # region SQL Mappers

        public static Vanrise.Integration.Entities.MappingOutput NewNormalCDRMapping_SQL()
        {
            LogVerbose("Started");

            var cdrs = new List<Vanrise.Fzero.CDRImport.Entities.CDR>();

            PSTN.BusinessEntity.Business.TrunkManager trunkManager = new PSTN.BusinessEntity.Business.TrunkManager();

            var importedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data)); 
            IDataReader reader = importedData.Reader;

            int rowCount = 0;
            while (reader.Read())
            {
                var cdr = new Vanrise.Fzero.CDRImport.Entities.CDR();

                cdr.CallType = Utils.GetReaderValue<Vanrise.Fzero.CDRImport.Entities.CallType>(reader, "CallTypeID");
                cdr.BTS = reader["BTS"].ToString();
                cdr.IMSI = reader["IMSI"] as string;
                cdr.IMEI = reader["IMEI"] as string;
                cdr.Cell = reader["Cell"] as string;
                cdr.UpVolume = Utils.GetReaderValue<Decimal?>(reader, "UpVolume");
                cdr.DownVolume = Utils.GetReaderValue<Decimal?>(reader, "DownVolume");
                cdr.CellLatitude = Utils.GetReaderValue<Decimal?>(reader, "CellLatitude");
                cdr.CellLongitude = Utils.GetReaderValue<Decimal?>(reader, "CellLongitude");
                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectDateTime");
                cdr.DurationInSeconds = Utils.GetReaderValue<decimal>(reader, "DurationInSeconds");
                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
                cdr.ServiceTypeId = Utils.GetReaderValue<int?>(reader, "ServiceTypeID");
                cdr.ServiceVASName = reader["ServiceVASName"] as string;
                cdr.ReleaseCode = reader["ReleaseCode"] as string;
                cdr.Destination = reader["Destination"] as string;
                cdr.MSISDN = reader["MSISDN"] as string;

                Vanrise.Fzero.FraudAnalysis.Entities.CallClass callClass = Vanrise.Fzero.FraudAnalysis.Business.CallClassManager.GetCallClassByDesc(reader["CallClassName"] as string);
                if (callClass != null)
                    cdr.CallClassId = callClass.Id;

                if (Utils.GetReaderValue<Byte?>(reader, "IsOnNet") == 1)
                    cdr.IsOnNet = true;
                else
                    cdr.IsOnNet = false;

                switch (reader["SubscriberTypeID"] as string)
                {
                    case "INROAMER":
                        cdr.SubscriberType = Vanrise.Fzero.CDRImport.Entities.SubscriberType.INROAMER;
                        break;

                    case "OUTROAMER":
                        cdr.SubscriberType = Vanrise.Fzero.CDRImport.Entities.SubscriberType.OUTROAMER;
                        break;

                    case "POSTPAID":
                        cdr.SubscriberType = Vanrise.Fzero.CDRImport.Entities.SubscriberType.POSTPAID;
                        break;

                    case "PREPAID":
                        cdr.SubscriberType = Vanrise.Fzero.CDRImport.Entities.SubscriberType.PREPAID;
                        break;

                    case "PREROAMER":
                        cdr.SubscriberType = Vanrise.Fzero.CDRImport.Entities.SubscriberType.PREROAMER;
                        break;
                }

                cdr.InTrunkId = trunkManager.GetTrunkIdBySymbol(reader["In_Trunk"] as string);
                cdr.OutTrunkId = trunkManager.GetTrunkIdBySymbol(reader["Out_Trunk"] as string);

                importedData.LastImportedId = reader["ID"];

                cdrs.Add(cdr);
                rowCount++;
                if (rowCount == 100000)
                    break;

            }
            if (cdrs.Count > 0)
            {
                var batch = new Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch();
                batch.CDRs = cdrs;
                mappedBatches.Add("Map Normal CDRs", batch);
                //mappedBatches.Add("Save Normal CDRs", batch);     
            }
            else
                importedData.IsEmpty = true;
            var result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput StaggingCDRs_SQL()
        {
            LogVerbose("Started");
            Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch();
            batch.StagingCDRs = new List<Vanrise.Fzero.CDRImport.Entities.StagingCDR>();
            Vanrise.Integration.Entities.DBReaderImportedData ImportedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));
            PSTN.BusinessEntity.Business.TrunkManager trunkManager = new PSTN.BusinessEntity.Business.TrunkManager();
            IDataReader reader = ImportedData.Reader;
            Object index = ImportedData.LastImportedId;
            while (reader.Read())
            {
                Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr = new Vanrise.Fzero.CDRImport.Entities.StagingCDR();
                cdr.CDPN = reader["CDPN"] as string;
                cdr.CGPN = reader["CGPN"] as string;
                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectDateTime");
                cdr.DurationInSeconds = Utils.GetReaderValue<decimal>(reader, "DurationInSeconds");
                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
                cdr.InTrunkId = trunkManager.GetTrunkIdBySymbol(reader["InTrunkSymbol"] as string);
                cdr.OutTrunkId = trunkManager.GetTrunkIdBySymbol(reader["OutTrunkSymbol"] as string);
                index = reader["ID"];
                batch.StagingCDRs.Add(cdr);
            }

            ImportedData.LastImportedId = index;
            batch.Datasource = dataSourceId;
            mappedBatches.Add("Normalize CDRs", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }

        public static Vanrise.Integration.Entities.MappingOutput ImportingCDR_SQL()
        {
            LogVerbose("Started");
            Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch();
            batch.CDRs = new List<Vanrise.Fzero.CDRImport.Entities.CDR>();
            Vanrise.Integration.Entities.DBReaderImportedData ImportedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));
            PSTN.BusinessEntity.Business.TrunkManager trunkManager = new PSTN.BusinessEntity.Business.TrunkManager();
            IDataReader reader = ImportedData.Reader;
            Object index = ImportedData.LastImportedId;

            while (reader.Read())
            {
                Vanrise.Fzero.CDRImport.Entities.CDR cdr = new Vanrise.Fzero.CDRImport.Entities.CDR();
                cdr.CallType = Utils.GetReaderValue<Vanrise.Fzero.CDRImport.Entities.CallType>(reader, "CallTypeID");
                cdr.BTS = reader["BTS"].ToString();
                cdr.IMSI = reader["IMSI"] as string;
                cdr.IMEI = reader["IMEI"] as string;
                cdr.Cell = reader["Cell"] as string;
                cdr.UpVolume = Utils.GetReaderValue<Decimal?>(reader, "UpVolume");
                cdr.DownVolume = Utils.GetReaderValue<Decimal?>(reader, "DownVolume");
                cdr.CellLatitude = Utils.GetReaderValue<Decimal?>(reader, "CellLatitude");
                cdr.CellLongitude = Utils.GetReaderValue<Decimal?>(reader, "CellLongitude");
                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime>(reader, "ConnectDateTime");
                cdr.DurationInSeconds = Utils.GetReaderValue<decimal>(reader, "DurationInSeconds");
                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
                cdr.ServiceTypeId = Utils.GetReaderValue<int?>(reader, "ServiceTypeID");
                cdr.ServiceVASName = reader["ServiceVASName"] as string;
                cdr.ReleaseCode = reader["ReleaseCode"] as string;
                cdr.Destination = reader["Destination"] as string;
                cdr.MSISDN = reader["MSISDN"] as string;

                Vanrise.Fzero.FraudAnalysis.Entities.CallClass callClass = Vanrise.Fzero.FraudAnalysis.Business.CallClassManager.GetCallClassByDesc(reader["CallClassName"] as string);
                if (callClass != null)
                    cdr.CallClassId = callClass.Id;

                if (Utils.GetReaderValue<Byte?>(reader, "IsOnNet") == 1)
                    cdr.IsOnNet = true;
                else
                    cdr.IsOnNet = false;


                switch (reader["SubscriberTypeID"] as string)
                {
                    case "INROAMER":
                        cdr.SubscriberType = Vanrise.Fzero.CDRImport.Entities.SubscriberType.INROAMER;
                        break;

                    case "OUTROAMER":
                        cdr.SubscriberType = Vanrise.Fzero.CDRImport.Entities.SubscriberType.OUTROAMER;
                        break;

                    case "POSTPAID":
                        cdr.SubscriberType = Vanrise.Fzero.CDRImport.Entities.SubscriberType.POSTPAID;
                        break;

                    case "PREPAID":
                        cdr.SubscriberType = Vanrise.Fzero.CDRImport.Entities.SubscriberType.PREPAID;
                        break;

                    case "PREROAMER":
                        cdr.SubscriberType = Vanrise.Fzero.CDRImport.Entities.SubscriberType.PREROAMER;
                        break;
                }

                cdr.InTrunkId = trunkManager.GetTrunkIdBySymbol(reader["In_Trunk"] as string);
                cdr.OutTrunkId = trunkManager.GetTrunkIdBySymbol(reader["Out_Trunk"] as string);

                index = reader["ID"];

                batch.CDRs.Add(cdr);
            }

            ImportedData.LastImportedId = index;
            batch.Datasource = dataSourceId;
            mappedBatches.Add("Normalize CDRs", batch);

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }
        # endregion


    }
}
