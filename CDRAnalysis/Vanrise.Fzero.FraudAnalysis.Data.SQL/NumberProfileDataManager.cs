﻿using System;
using System.Data.SqlClient;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;


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
                                                 with(nolock)    where (connectDateTime >= @From and connectDateTime <=@To)  order by MSISDN; ";



            ExecuteReaderText(query_GetCDRRange, (reader) =>
                {

                    NormalCDR normalCDR = new NormalCDR();
                    int count = 0;
                    int currentIndex = 0;


                    while (reader.Read())
                    {
                        normalCDR.CallType = GetReaderValue<int>(reader, "Call_Type");
                        normalCDR.BTSId = GetReaderValue<int>(reader, "BTS_Id");
                        normalCDR.ConnectDateTime = GetReaderValue<DateTime>(reader, "ConnectDateTime");
                        normalCDR.Id = (int)reader["Id"];
                        normalCDR.IMSI = reader["IMSI"] as string;
                        normalCDR.DurationInSeconds = GetReaderValue<Decimal>(reader, "DurationInSeconds");
                        normalCDR.DisconnectDateTime = GetReaderValue<DateTime>(reader, "DisconnectDateTime");
                        normalCDR.CallClass = reader["Call_Class"] as string;
                        normalCDR.IsOnNet = GetReaderValue<Byte>(reader, "IsOnNet");
                        normalCDR.SubType = reader["Sub_Type"] as string;
                        normalCDR.IMEI = reader["IMEI"] as string;
                        normalCDR.CellId = reader["Cell_Id"] as string;
                        normalCDR.SwitchRecordId = GetReaderValue<int>(reader, "SwitchRecordId");
                        normalCDR.UpVolume = GetReaderValue<Decimal>(reader, "Up_Volume");
                        normalCDR.DownVolume = GetReaderValue<Decimal>(reader, "Down_Volume");
                        normalCDR.CellLatitude = GetReaderValue<Decimal>(reader, "Cell_Latitude");
                        normalCDR.CellLongitude = GetReaderValue<Decimal>(reader, "Cell_Longitude");
                        normalCDR.InTrunk = reader["In_Trunk"] as string;
                        normalCDR.OutTrunk = reader["Out_Trunk"] as string;
                        normalCDR.ServiceType = GetReaderValue<int>(reader, "Service_Type");
                        normalCDR.ServiceVASName = reader["Service_VAS_Name"] as string;
                        normalCDR.Destination = reader["Destination"] as string;
                        normalCDR.MSISDN = reader["MSISDN"] as string;


                       

                        currentIndex++;
                        if (currentIndex == 10000)
                        {
                            count += currentIndex;
                            currentIndex = 0;
                            Console.WriteLine("{0} rows read ", count);
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
