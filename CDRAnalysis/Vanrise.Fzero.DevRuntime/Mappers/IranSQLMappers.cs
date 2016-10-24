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
    public class IranSQLMappers
    {

        static Guid dataSourceId = new Guid("BE08FA12-7978-4774-B0DF-4CC543596A71");
        static DBReaderImportedData data = new DBReaderImportedData();

        public static void FillData()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = "Data Source=192.168.110.185;Initial Catalog=CDRAnalysisMobile_WF;User ID=development;Password=dev!123;";
            SqlCommand command = new SqlCommand(
              "SELECT top 1000 [ID]  ,[CallRecordType]   ,[SubscriberMSISDN]   ,[CallPartner]  ,[CallDate]  ,[CallTime]  ,[CallDuration]  ,[MSLocation]   ,[IMEI]  FROM [dbo].[IranCDR]", connection);
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
        public static Vanrise.Integration.Entities.MappingOutput ImportingCDR_SQL()
        {
            LogVerbose("Started");
            Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch batch = new Vanrise.Fzero.CDRImport.Entities.ImportedCDRBatch();
            batch.CDRs = new List<Vanrise.Fzero.CDRImport.Entities.CDR>();
            Vanrise.Integration.Entities.DBReaderImportedData ImportedData = ((Vanrise.Integration.Entities.DBReaderImportedData)(data));
            IDataReader reader = ImportedData.Reader;
            long lastImportedId = 0;
            if (ImportedData.LastImportedId != null)
                lastImportedId = Convert.ToInt64(ImportedData.LastImportedId);

            while (reader.Read())
            {
                Vanrise.Fzero.CDRImport.Entities.CDR cdr = new Vanrise.Fzero.CDRImport.Entities.CDR();

                lastImportedId = (long)reader["ID"];
                cdr.MSISDN = reader["SubscriberMSISDN"] as string;
                cdr.Destination = reader["CallPartner"] as string;


                DateTime CallDate = Utils.GetReaderValue<DateTime>(reader, "CallDate");
                TimeSpan CallTime = Utils.GetReaderValue<TimeSpan>(reader, "CallTime");
                cdr.ConnectDateTime = CallDate.Add(CallTime);


                cdr.DurationInSeconds = Utils.GetReaderValue<decimal>(reader, "CallDuration");
                cdr.IMEI = reader["IMEI"] as string;

                // Assumed this is Cell ...
                cdr.Cell = reader["MSLocation"] as string;
                ////////////////////////////////////////////////

                //In this piece of code we assume that first 5 charachters are BTS 
                if (!string.IsNullOrEmpty(cdr.Cell) && cdr.Cell.Length>5)
                    cdr.BTS = cdr.Cell.Substring(0, 5);
                ////////////////////////////////////////////////


                switch (Utils.GetReaderValue<int>(reader, "CallRecordType"))
                {
                    case 1:
                        cdr.CallType = Vanrise.Fzero.CDRImport.Entities.CallType.OutgoingVoiceCall;
                        break;

                    case 2:
                        cdr.CallType = Vanrise.Fzero.CDRImport.Entities.CallType.IncomingVoiceCall;
                        break;

                    case 3:case 5:
                        cdr.CallType = Vanrise.Fzero.CDRImport.Entities.CallType.OutgoingSms;
                        break;

                    case 4:case 6:
                        cdr.CallType = Vanrise.Fzero.CDRImport.Entities.CallType.IncomingSms;
                        break;

                    case 9:// In CDR Analysis no Call type Data ... so I changed it to not defined
                        cdr.CallType = Vanrise.Fzero.CDRImport.Entities.CallType.NotDefined;
                        break;
                }


                batch.CDRs.Add(cdr);
            }

            ImportedData.LastImportedId = lastImportedId.ToString();
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
