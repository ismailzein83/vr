using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Data;
using System.Data.SqlClient;
using System.IO;
using Vanrise.Data.SQL;


namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class NumberProfileDataManager : BaseSQLDataManager, INumberProfileDataManager
    {
        public NumberProfileDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<NormalCDR> onBatchReady)
        {
            string query_GetCDRRange = @"SELECT  [Id] ,[MSISDN] ,[IMSI] ,[ConnectDateTime] ,[Destination] ,[DurationInSeconds] ,[DisconnectDateTime] ,[Call_Class]  ,[IsOnNet] ,[Call_Type] ,[Sub_Type] ,[IMEI]
                                                ,[BTS_Id]  ,[Cell_Id]  ,[SwitchRecordId]  ,[Up_Volume]  ,[Down_Volume] ,[Cell_Latitude]  ,[Cell_Longitude]  ,[In_Trunk]  ,[Out_Trunk]  ,[Service_Type]  ,[Service_VAS_Name] FROM NormalCDR
                                                 with(nolock)    where connectDateTime >= @From and connectDateTime <=@To  order by MSISDN; ";



            ExecuteReaderText(query_GetCDRRange, (reader) =>
                {

                    NormalCDR normalCDR = new NormalCDR();
                    int count = 0;
                    int currentIndex = 0;


                    while (reader.Read())
                    {
                        normalCDR.callType = GetReaderValue<int>(reader, "Call_Type");
                        normalCDR.bTSId = GetReaderValue<int>(reader, "BTS_Id");
                        normalCDR.connectDateTime = GetReaderValue<DateTime>(reader, "ConnectDateTime");
                        normalCDR.id = (int)reader["Id"];
                        normalCDR.iMSI = reader["IMSI"] as string;
                        normalCDR.durationInSeconds = GetReaderValue<Decimal>(reader, "DurationInSeconds");
                        normalCDR.disconnectDateTime = GetReaderValue<DateTime>(reader, "DisconnectDateTime");
                        normalCDR.callClass = reader["Call_Class"] as string;
                        normalCDR.isOnNet = GetReaderValue<Byte>(reader, "IsOnNet");
                        normalCDR.subType = reader["Sub_Type"] as string;
                        normalCDR.iMEI = reader["IMEI"] as string;
                        normalCDR.cellId = reader["Cell_Id"] as string;
                        normalCDR.switchRecordId = GetReaderValue<int>(reader, "SwitchRecordId");
                        normalCDR.upVolume = GetReaderValue<Decimal>(reader, "Up_Volume");
                        normalCDR.downVolume = GetReaderValue<Decimal>(reader, "Down_Volume");
                        normalCDR.cellLatitude = GetReaderValue<Decimal>(reader, "Cell_Latitude");
                        normalCDR.cellLongitude = GetReaderValue<Decimal>(reader, "Cell_Longitude");
                        normalCDR.inTrunk = reader["In_Trunk"] as string;
                        normalCDR.outTrunk = reader["Out_Trunk"] as string;
                        normalCDR.serviceType = GetReaderValue<int>(reader, "Service_Type");
                        normalCDR.serviceVASName = reader["Service_VAS_Name"] as string;
                        normalCDR.destination = reader["Destination"] as string;
                        normalCDR.mSISDN = reader["MSISDN"] as string;


                        

                        currentIndex++;
                        if (currentIndex == 10000)
                        {
                            count += currentIndex;
                            currentIndex = 0;
                            Console.WriteLine("{0} rows read", count);
                        }

                        onBatchReady(normalCDR);
                    }

                   

                },
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@From", from));
                    cmd.Parameters.Add(new SqlParameter("@To", to));
                });
        }

    }
}
