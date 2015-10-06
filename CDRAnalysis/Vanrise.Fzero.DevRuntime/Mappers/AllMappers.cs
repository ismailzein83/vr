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
    public class AllMappers
    {

        static int dataSourceId = 23;
        static DBReaderImportedData data = new DBReaderImportedData();

        public static void FillData()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "Data Source=192.168.110.185;Initial Catalog=CDRAnalysisMobile_Research;User ID=development;Password=dev!123;";
            SqlCommand command = new SqlCommand(
              "SELECT [Id] ,[MSISDN]  ,[IMSI]  ,[ConnectDateTime]  ,[Destination]  ,[DurationInSeconds]  ,[DisconnectDateTime]   ,[Call_Class]   ,[IsOnNet]  ,[Call_Type]   ,[Sub_Type]   ,[IMEI]   ,[BTS_Id]   ,[Cell_Id]   ,[SwitchRecordId]  ,[Up_Volume]   ,[Down_Volume]   ,[Cell_Latitude]   ,[Cell_Longitude]      ,[In_Trunk]  ,[Out_Trunk]    ,[Service_Type]   ,[Service_VAS_Name]  FROM [dbo].[NormalCDRZein];",
              connection);
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
        public static Vanrise.Integration.Entities.MappingOutput StaggingCDRs_SQL()
        {
            PSTN.BusinessEntity.Business.SwitchTrunkManager switchTrunkManager = new PSTN.BusinessEntity.Business.SwitchTrunkManager();
            PSTN.BusinessEntity.Entities.SwitchTrunkInfo currentTrunk = new PSTN.BusinessEntity.Entities.SwitchTrunkInfo();

            PSTN.BusinessEntity.Business.SwitchManager switchManager = new PSTN.BusinessEntity.Business.SwitchManager();
            PSTN.BusinessEntity.Entities.Switch currentSwitch;
            currentSwitch = switchManager.GetSwitchByDataSourceID(dataSourceId);
            int? SwitchId = null;
            TimeSpan TimeOffset = new TimeSpan();
            if (currentSwitch != null)
            {
                SwitchId = currentSwitch.ID;
                TimeOffset = currentSwitch.TimeOffset;
            }

            else
                LogVerbose("No Switch associated to this Datasource");




            LogVerbose("Started");
            Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch();
            batch.StagingCDRs = new List<Vanrise.Fzero.CDRImport.Entities.StagingCDR>();
            Vanrise.Integration.Entities.DBReaderImportedData ImportedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));
            IDataReader reader = ImportedData.Reader;
            string index = ImportedData.LastImportedId;
            while (reader.Read())
            {
                Vanrise.Fzero.CDRImport.Entities.StagingCDR cdr = new Vanrise.Fzero.CDRImport.Entities.StagingCDR();
                cdr.CDPN = reader["CDPN"] as string;
                cdr.CGPN = reader["CGPN"] as string;
                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "ConnectDateTime");
                if (cdr.ConnectDateTime != null)
                    cdr.ConnectDateTime = cdr.ConnectDateTime.Value.Add(TimeOffset);
                cdr.DurationInSeconds = Utils.GetReaderValue<Decimal?>(reader, "DurationInSeconds");
                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");

                if (cdr.DisconnectDateTime != null)
                    cdr.DisconnectDateTime = cdr.DisconnectDateTime.Value.Add(TimeOffset);

                cdr.InTrunkSymbol = reader["InTrunkSymbol"] as string;
                cdr.OutTrunkSymbol = reader["OutTrunkSymbol"] as string;


                if (cdr.InTrunkSymbol != null && cdr.InTrunkSymbol != string.Empty)
                {
                    currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(cdr.InTrunkSymbol);
                    if (currentTrunk != null)
                        cdr.InTrunkId = currentTrunk.ID;
                }

                if (cdr.OutTrunkSymbol != null && cdr.OutTrunkSymbol != string.Empty)
                {
                    currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(cdr.OutTrunkSymbol);
                    if (currentTrunk != null)
                        cdr.OutTrunkId = currentTrunk.ID;
                }

                cdr.SwitchID = SwitchId;

                index = Utils.GetReaderValue<int?>(reader, "ID").ToString();

                batch.StagingCDRs.Add(cdr);

            }

            ImportedData.LastImportedId = index;
            mappedBatches.Add("Save CDRs", batch); // Save Without Normalization

            mappedBatches.Add("Normalize CDRs", batch); // Normalize then Save


            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }


        public static Vanrise.Integration.Entities.MappingOutput ImportingCDR_SQL()
        {
            PSTN.BusinessEntity.Business.SwitchTrunkManager switchTrunkManager = new PSTN.BusinessEntity.Business.SwitchTrunkManager();
            PSTN.BusinessEntity.Entities.SwitchTrunkInfo currentTrunk = new PSTN.BusinessEntity.Entities.SwitchTrunkInfo();

            PSTN.BusinessEntity.Business.SwitchManager switchManager = new PSTN.BusinessEntity.Business.SwitchManager();
            PSTN.BusinessEntity.Entities.Switch currentSwitch;
            currentSwitch = switchManager.GetSwitchByDataSourceID(dataSourceId);
            int? SwitchId = null;
            TimeSpan TimeOffset = new TimeSpan();
            if (currentSwitch != null)
            {
                SwitchId = currentSwitch.ID;
                TimeOffset = currentSwitch.TimeOffset;
            }

            else
                LogVerbose("No Switch associated to this Datasource");




            LogVerbose("Started");
            Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch();
            batch.CDRs = new List<Vanrise.Fzero.CDRImport.Entities.CDR>();
            Vanrise.Integration.Entities.DBReaderImportedData ImportedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));

            IDataReader reader = ImportedData.Reader;
            string index = ImportedData.LastImportedId;

            while (reader.Read())
            {
                Vanrise.Fzero.CDRImport.Entities.CDR cdr = new Vanrise.Fzero.CDRImport.Entities.CDR();
                cdr.CallType = Utils.GetReaderValue<Vanrise.Fzero.CDRImport.Entities.CallType>(reader, "Call_Type");
                cdr.BTSId = Utils.GetReaderValue<int?>(reader, "BTS_Id");

                cdr.ConnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "ConnectDateTime");
                if (cdr.ConnectDateTime != null)
                    cdr.ConnectDateTime = cdr.ConnectDateTime.Value.Add(TimeOffset);

                cdr.DurationInSeconds = Utils.GetReaderValue<Decimal?>(reader, "DurationInSeconds");

                cdr.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");
                if (cdr.DisconnectDateTime != null)
                    cdr.DisconnectDateTime = cdr.DisconnectDateTime.Value.Add(TimeOffset);

                cdr.Id = (int)reader["Id"];
                cdr.IMSI = reader["IMSI"] as string;
                cdr.CallClass = reader["Call_Class"] as string;
                cdr.IsOnNet = Utils.GetReaderValue<Byte?>(reader, "IsOnNet");
                cdr.SubType = reader["Sub_Type"] as string;
                cdr.IMEI = reader["IMEI"] as string;
                cdr.CellId = reader["Cell_Id"] as string;
                cdr.UpVolume = Utils.GetReaderValue<Decimal?>(reader, "Up_Volume");
                cdr.DownVolume = Utils.GetReaderValue<Decimal?>(reader, "Down_Volume");
                cdr.CellLatitude = Utils.GetReaderValue<Decimal?>(reader, "Cell_Latitude");
                cdr.CellLongitude = Utils.GetReaderValue<Decimal?>(reader, "Cell_Longitude");

                cdr.InTrunkSymbol = reader["In_Trunk"] as string;
                cdr.OutTrunkSymbol = reader["Out_Trunk"] as string;


                if (cdr.InTrunkSymbol != null && cdr.InTrunkSymbol != string.Empty)
                {
                    currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(cdr.InTrunkSymbol);
                    if (currentTrunk != null)
                        cdr.InTrunkId = currentTrunk.ID;
                }

                if (cdr.OutTrunkSymbol != null && cdr.OutTrunkSymbol != string.Empty)
                {
                    currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(cdr.OutTrunkSymbol);
                    if (currentTrunk != null)
                        cdr.OutTrunkId = currentTrunk.ID;
                }

                cdr.SwitchID = SwitchId;


                cdr.ServiceType = Utils.GetReaderValue<int?>(reader, "Service_Type");
                cdr.ServiceVASName = reader["Service_VAS_Name"] as string;
                cdr.Destination = reader["Destination"] as string;
                cdr.MSISDN = reader["MSISDN"] as string;

                index = cdr.Id.ToString();

                batch.CDRs.Add(cdr);
            }

            ImportedData.LastImportedId = index;

            mappedBatches.Add("CDR Import", batch);// Save Without Normalization

            mappedBatches.Add("Normalize CDRs", batch); // Normalize then Save


            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            LogVerbose("Finished");
            return result;
        }
        # endregion

        # region FTP or File Mappers

        public static void MovingtoStaggingCDRsDS_FileorFTP()
        {
            //PSTN.BusinessEntity.Business.SwitchTrunkManager switchTrunkManager = new PSTN.BusinessEntity.Business.SwitchTrunkManager();
            //PSTN.BusinessEntity.Entities.SwitchTrunkInfo currentTrunk = new PSTN.BusinessEntity.Entities.SwitchTrunkInfo();

            //PSTN.BusinessEntity.Business.SwitchManager switchManager = new PSTN.BusinessEntity.Business.SwitchManager();
            //PSTN.BusinessEntity.Entities.Switch currentSwitch;
            //currentSwitch = switchManager.GetSwitchByDataSourceID(dataSourceId);
            //int? SwitchId = null;

            //if (currentSwitch != null)
            //    SwitchId = currentSwitch.ID;
            //else
            //    LogVerbose("No Switch associated to this Datasource");


            //LogVerbose("Started");
            //Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch();
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
            //    if (stagingCDR.ConnectDateTime != null)
            //        stagingCDR.ConnectDateTime = stagingCDR.ConnectDateTime.Value.Add(currentSwitch.TimeOffset);
            //    stagingCDR.DurationInSeconds = Utils.GetReaderValue<Decimal?>(reader, "DurationInSeconds");
            //    stagingCDR.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");

            //    if (stagingCDR.DisconnectDateTime != null)
            //        stagingCDR.DisconnectDateTime = stagingCDR.DisconnectDateTime.Value.Add(currentSwitch.TimeOffset);

            //    stagingCDR.InTrunkSymbol = reader["InTrunkSymbol"] as string;
            //    stagingCDR.OutTrunkSymbol = reader["OutTrunkSymbol"] as string;

            //    LogVerbose("stagingCDR.OutTrunkSymbol: " + stagingCDR.OutTrunkSymbol);

            //    LogVerbose("stagingCDR.InTrunkSymbol: " + stagingCDR.InTrunkSymbol);


            //    if (stagingCDR.InTrunkSymbol != string.Empty)
            //    {
            //        LogVerbose("before check: 1 ");
            //        currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(stagingCDR.InTrunkSymbol);

            //        LogVerbose("currentTrunk.ID 1: " + currentTrunk.ID);
            //        stagingCDR.InTrunkId = currentTrunk.ID;
            //    }

            //    if (stagingCDR.OutTrunkSymbol != string.Empty)
            //    {
            //        LogVerbose("before check: 2 ");
            //        currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(stagingCDR.OutTrunkSymbol);

            //        LogVerbose("currentTrunk.ID 2: " + currentTrunk.ID);
            //        stagingCDR.OutTrunkId = currentTrunk.ID;
            //    }



            //    stagingCDR.SwitchID = SwitchId.Value;
            //    index = Utils.GetReaderValue<int?>(reader, "ID").ToString();
            //    batch.StagingCDRs.Add(stagingCDR);
            //}

            //ImportedData.LastImportedId = index;
            //mappedBatches.Add("Save CDRs", batch);
            //Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            //result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            //LogVerbose("Finished");
            //return result;
        }

        public static void NormlaizingandMovingtoStaggingCDRsDS_FileorFTP()
        {
            //PSTN.BusinessEntity.Business.SwitchTrunkManager switchTrunkManager = new PSTN.BusinessEntity.Business.SwitchTrunkManager();
            //PSTN.BusinessEntity.Entities.SwitchTrunkInfo currentTrunk=null;


            //PSTN.BusinessEntity.Business.SwitchManager switchManager = new PSTN.BusinessEntity.Business.SwitchManager();

            //PSTN.BusinessEntity.Entities.Switch currentSwitch;
            //currentSwitch = switchManager.GetSwitchByDataSourceID(dataSourceId);
            //LogVerbose("3");
            //int? SwitchId = null;
            //TimeSpan TimeOffset = new TimeSpan();

            //if (currentSwitch != null)
            //{
            //    SwitchId = currentSwitch.ID;
            //    TimeOffset = currentSwitch.TimeOffset;
            //}

            //else
            //    LogVerbose("No Switch associated to this Datasource");


            //LogVerbose("Started");
            //Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedStagingCDRBatch();
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
            //    if (stagingCDR.ConnectDateTime != null)
            //        stagingCDR.ConnectDateTime = stagingCDR.ConnectDateTime.Value.Add(TimeOffset);
            //    stagingCDR.DurationInSeconds = Utils.GetReaderValue<Decimal?>(reader, "DurationInSeconds");
            //    stagingCDR.DisconnectDateTime = Utils.GetReaderValue<DateTime?>(reader, "DisconnectDateTime");

            //    if (stagingCDR.DisconnectDateTime != null)
            //        stagingCDR.DisconnectDateTime = stagingCDR.DisconnectDateTime.Value.Add(TimeOffset);

            //    stagingCDR.InTrunkSymbol = reader["InTrunkSymbol"] as string;
            //    stagingCDR.OutTrunkSymbol = reader["OutTrunkSymbol"] as string;

            //    if (stagingCDR.InTrunkSymbol != string.Empty)
            //        currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(stagingCDR.InTrunkSymbol);

            //    if (currentTrunk != null)
            //    {
            //        stagingCDR.InTrunkId = currentTrunk.ID;
            //        currentTrunk = null;
            //    }



            //    if (stagingCDR.OutTrunkSymbol != string.Empty)
            //        currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(stagingCDR.OutTrunkSymbol);

            //    if (currentTrunk != null)
            //    {
            //        stagingCDR.OutTrunkId = currentTrunk.ID;
            //        currentTrunk = null;
            //    }

            //    stagingCDR.SwitchID = SwitchId;
            //    index = Utils.GetReaderValue<int?>(reader, "ID").ToString();
            //    batch.StagingCDRs.Add(stagingCDR);
            //}

            //ImportedData.LastImportedId = index;
            //mappedBatches.Add("Normalize CDRs", batch);
            //Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            //result.Result = Vanrise.Integration.Entities.MappingResult.Valid;
            //LogVerbose("Finished");
            //return result;
        }

        public static void ImportingCDR_FileorFTP()
        {
            //PSTN.BusinessEntity.Business.SwitchTrunkManager switchTrunkManager = new PSTN.BusinessEntity.Business.SwitchTrunkManager();
            //PSTN.BusinessEntity.Entities.SwitchTrunkInfo currentTrunk=null;

            //Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch();
            //batch.CDRs = new List<Vanrise.Fzero.CDRImport.Entities.CDR>();
            //System.IO.StreamReader sr = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data)).StreamReader;
            //while (!sr.EndOfStream)
            //{
            //    var i = sr.ReadLine();

            //    Vanrise.Fzero.CDRImport.Entities.CDR cdr = new Vanrise.Fzero.CDRImport.Entities.CDR();
            //    cdr.MSISDN = i.Substring(145, 20).Trim();
            //    cdr.IMSI = i.Substring(125, 20).Trim();
            //    cdr.Destination = i.Substring(198, 20).Trim();
            //    cdr.CallClass = i.Substring(434, 10).Trim();
            //    cdr.SubType = i.Substring(165, 10).Trim();
            //    cdr.IMEI = i.Substring(105, 20).Trim();
            //    cdr.CellId = i.Substring(252, 22).Trim();
            //    cdr.InTrunkSymbol = i.Substring(414, 20).Trim();
            //    cdr.OutTrunkSymbol = i.Substring(394, 20).Trim();

            //    if (cdr.InTrunkSymbol != string.Empty)
            //        currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(cdr.InTrunkSymbol);

            //    if (currentTrunk != null)
            //    {
            //        cdr.InTrunkId = currentTrunk.ID;
            //        currentTrunk = null;
            //    }



            //    if (cdr.OutTrunkSymbol != string.Empty)
            //        currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(cdr.OutTrunkSymbol);

            //    if (currentTrunk != null)
            //    {
            //        cdr.OutTrunkId = currentTrunk.ID;
            //        currentTrunk = null;
            //    }



            //    DateTime ConnectDateTime;
            //    if (DateTime.TryParseExact(i.Substring(221, 14).Trim(), "yyyyMddHHmmss", System.Globalization.CultureInfo.InvariantCulture,
            //                               System.Globalization.DateTimeStyles.None, out ConnectDateTime))
            //        cdr.ConnectDateTime = ConnectDateTime;



            //    int callType = 0;
            //    if (int.TryParse(i.Substring(102, 3).Trim(), out callType))
            //        cdr.CallType = (Vanrise.Fzero.CDRImport.Entities.CallType)callType;

            //    decimal cellLatitude;
            //    if (decimal.TryParse(i.Substring(609, 9).Trim(), out cellLatitude))
            //        cdr.CellLatitude = cellLatitude;


            //    decimal durationInSeconds;
            //    if (decimal.TryParse(i.Substring(235, 5).Trim(), out durationInSeconds))
            //        cdr.DurationInSeconds = durationInSeconds;


            //    decimal upVolume;
            //    if (decimal.TryParse(i.Substring(588, 10).Trim(), out upVolume))
            //        cdr.UpVolume = upVolume;


            //    decimal cellLongitude;
            //    if (decimal.TryParse(i.Substring(618, 9).Trim(), out cellLongitude))
            //        cdr.CellLongitude = cellLongitude;


            //    decimal downVolume;
            //    if (decimal.TryParse(i.Substring(598, 10).Trim(), out downVolume))
            //        cdr.DownVolume = downVolume;


            //    batch.CDRs.Add(cdr);
            //}
            //mappedBatches.Add("CDR Import", batch);

            //Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            //result.Result = Vanrise.Integration.Entities.MappingResult.Valid;

            //return result;
        }

        public static void NormlaizingandImportingCDR_FileorFTP()
        {
            //PSTN.BusinessEntity.Business.SwitchTrunkManager switchTrunkManager = new PSTN.BusinessEntity.Business.SwitchTrunkManager();
            //PSTN.BusinessEntity.Entities.SwitchTrunkInfo currentTrunk=null;

            //Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch();
            //batch.CDRs = new List<Vanrise.Fzero.CDRImport.Entities.CDR>();
            //System.IO.StreamReader sr = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data)).StreamReader;
            //while (!sr.EndOfStream)
            //{
            //    var i = sr.ReadLine();

            //    Vanrise.Fzero.CDRImport.Entities.CDR cdr = new Vanrise.Fzero.CDRImport.Entities.CDR();
            //    cdr.MSISDN = i.Substring(145, 20).Trim();
            //    cdr.IMSI = i.Substring(125, 20).Trim();
            //    cdr.Destination = i.Substring(198, 20).Trim();
            //    cdr.CallClass = i.Substring(434, 10).Trim();
            //    cdr.SubType = i.Substring(165, 10).Trim();
            //    cdr.IMEI = i.Substring(105, 20).Trim();
            //    cdr.CellId = i.Substring(252, 22).Trim();
            //    cdr.InTrunkSymbol = i.Substring(414, 20).Trim();
            //    cdr.OutTrunkSymbol = i.Substring(394, 20).Trim();

            //    if (cdr.InTrunkSymbol != string.Empty)
            //        currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(cdr.InTrunkSymbol);

            //    if (currentTrunk != null)
            //    {
            //        cdr.InTrunkId = currentTrunk.ID;
            //        currentTrunk = null;
            //    }



            //    if (cdr.OutTrunkSymbol != string.Empty)
            //        currentTrunk = switchTrunkManager.GetSwitchTrunkBySymbol(cdr.OutTrunkSymbol);

            //    if (currentTrunk != null)
            //    {
            //        cdr.OutTrunkId = currentTrunk.ID;
            //        currentTrunk = null;
            //    }


            //    DateTime ConnectDateTime;
            //    if (DateTime.TryParseExact(i.Substring(221, 14).Trim(), "yyyyMddHHmmss", System.Globalization.CultureInfo.InvariantCulture,
            //                               System.Globalization.DateTimeStyles.None, out ConnectDateTime))
            //        cdr.ConnectDateTime = ConnectDateTime;



            //    int callType = 0;
            //    if (int.TryParse(i.Substring(102, 3).Trim(), out callType))
            //        cdr.CallType = (Vanrise.Fzero.CDRImport.Entities.CallType)callType;

            //    decimal cellLatitude;
            //    if (decimal.TryParse(i.Substring(609, 9).Trim(), out cellLatitude))
            //        cdr.CellLatitude = cellLatitude;


            //    decimal durationInSeconds;
            //    if (decimal.TryParse(i.Substring(235, 5).Trim(), out durationInSeconds))
            //        cdr.DurationInSeconds = durationInSeconds;


            //    decimal upVolume;
            //    if (decimal.TryParse(i.Substring(588, 10).Trim(), out upVolume))
            //        cdr.UpVolume = upVolume;


            //    decimal cellLongitude;
            //    if (decimal.TryParse(i.Substring(618, 9).Trim(), out cellLongitude))
            //        cdr.CellLongitude = cellLongitude;


            //    decimal downVolume;
            //    if (decimal.TryParse(i.Substring(598, 10).Trim(), out downVolume))
            //        cdr.DownVolume = downVolume;


            //    batch.CDRs.Add(cdr);
            //}
            //mappedBatches.Add("Normalize CDRs", batch);

            //Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            //result.Result = Vanrise.Integration.Entities.MappingResult.Valid;

            //return result;
        }

        # endregion









    }
}
