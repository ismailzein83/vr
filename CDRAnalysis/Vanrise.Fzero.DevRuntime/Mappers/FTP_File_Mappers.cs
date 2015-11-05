using System;
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
                cdr.ReleaseCode = i.Substring(274, 50).Trim();


                DateTime ConnectDateTime;
                if (DateTime.TryParseExact(i.Substring(221, 14).Trim(), "yyyyMddHHmmss", System.Globalization.CultureInfo.InvariantCulture,
                                           System.Globalization.DateTimeStyles.None, out ConnectDateTime))
                    cdr.ConnectDateTime = ConnectDateTime; 


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

                batch.CDRs.Add(cdr);
            }

            mappedBatches.Add("Normalize CDRs", batch); // Normalize then Save

            Vanrise.Integration.Entities.MappingOutput result = new Vanrise.Integration.Entities.MappingOutput();
            result.Result = Vanrise.Integration.Entities.MappingResult.Valid;

            return result;
        }


        # endregion



    }
}
