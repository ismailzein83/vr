﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;



namespace Vanrise.Fzero.DevRuntime.Tasks.Mappers
{
    public class FTP_File_Mappers
    {

        static int dataSourceId = 23;

        static StreamReaderImportedData data = new StreamReaderImportedData();
        
        public static void FillData()
        {
            data.StreamReader = new StreamReader("E:\\CDR\\1.DAT");
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


        # region FTP or File Mappers

        public static Vanrise.Integration.Entities.MappingOutput ImportingCDR_FileorFTP()
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
            Vanrise.Integration.Entities.StreamReaderImportedData ImportedData = ((Vanrise.Integration.Entities.StreamReaderImportedData)(data));

            System.IO.StreamReader sr = ImportedData.StreamReader;


            while (!sr.EndOfStream)
            {
                var i = sr.ReadLine();

                Vanrise.Fzero.CDRImport.Entities.CDR cdr = new Vanrise.Fzero.CDRImport.Entities.CDR();
                cdr.MSISDN = i.Substring(145, 20).Trim();
                cdr.IMSI = i.Substring(125, 20).Trim();
                cdr.Destination = i.Substring(198, 20).Trim();
                cdr.CallClass = i.Substring(434, 10).Trim();
                cdr.SubType = i.Substring(165, 10).Trim();
                cdr.IMEI = i.Substring(105, 20).Trim();
                cdr.CellId = i.Substring(252, 22).Trim();
                cdr.InTrunkSymbol = i.Substring(414, 20).Trim();
                cdr.OutTrunkSymbol = i.Substring(394, 20).Trim();


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



                DateTime ConnectDateTime;
                if (DateTime.TryParseExact(i.Substring(221, 14).Trim(), "yyyyMddHHmmss", System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.None, out ConnectDateTime))
                    cdr.ConnectDateTime = ConnectDateTime.Add(TimeOffset); 



                int callType = 0;
                if (int.TryParse(i.Substring(102, 3).Trim(), out callType))
                    cdr.CallType = (Vanrise.Fzero.CDRImport.Entities.CallType)callType;

                decimal cellLatitude;
                if (decimal.TryParse(i.Substring(609, 9).Trim(), out cellLatitude))
                    cdr.CellLatitude = cellLatitude;


                decimal durationInSeconds;
                if (decimal.TryParse(i.Substring(235, 5).Trim(), out durationInSeconds))
                    cdr.DurationInSeconds = durationInSeconds;


                decimal upVolume;
                if (decimal.TryParse(i.Substring(588, 10).Trim(), out upVolume))
                    cdr.UpVolume = upVolume;


                decimal cellLongitude;
                if (decimal.TryParse(i.Substring(618, 9).Trim(), out cellLongitude))
                    cdr.CellLongitude = cellLongitude;


                decimal downVolume;
                if (decimal.TryParse(i.Substring(598, 10).Trim(), out downVolume))
                    cdr.DownVolume = downVolume;

                cdr.SwitchID = SwitchId;

                batch.CDRs.Add(cdr);
            }
            //mappedBatches.Add("CDR Import", batch);// Save Without Normalization

            mappedBatches.Add("Normalize CDRs", batch); // Normalize then Save

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;

            return result;
        }


        # endregion



    }
}
