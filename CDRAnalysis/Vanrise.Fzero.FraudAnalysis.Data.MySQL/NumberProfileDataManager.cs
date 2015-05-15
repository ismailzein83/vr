using System;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class NumberProfileDataManager : BaseMySQLDataManager, INumberProfileDataManager
    {
        public NumberProfileDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }


        public void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<NormalCDR> onBatchReady)
        {
            MySQLManager manager = new MySQLManager();
            string query_GetCDRRange = @"SELECT  `Id` ,`MSISDN` ,`IMSI` ,`ConnectDateTime` ,`Destination` ,`DurationInSeconds` ,`DisconnectDateTime` ,`Call_Class`  ,`IsOnNet` ,`Call_Type` ,`Sub_Type` ,`IMEI`
                                                ,`BTS_Id`  ,`Cell_Id`  ,`SwitchRecordId`  ,`Up_Volume`  ,`Down_Volume` ,`Cell_Latitude`  ,`Cell_Longitude`  ,`In_Trunk`  ,`Out_Trunk`  ,`Service_Type`  ,`Service_VAS_Name` FROM NormalCDR
                                                    where connectDateTime >= @From and connectDateTime <=@To  order by MSISDN; ";



            manager.ExecuteReader(query_GetCDRRange,
                (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@From", from);
                    cmd.Parameters.AddWithValue("@To", to);
                }, (reader) =>
            {

                NormalCDR normalCDR = new NormalCDR();
                int count = 0;
                int currentIndex = 0;


                while (reader.Read())
                {
                    normalCDR.MSISDN = reader["MSISDN"] as string;
                    normalCDR.Destination = reader["Destination"].ToString();
                    normalCDR.CallType = Helper.AsInt(reader["Call_Type"].ToString());
                    normalCDR.BTSId = Helper.AsInt(reader["BTS_Id"].ToString());
                    normalCDR.ConnectDateTime = Helper.AsDateTime(reader["ConnectDateTime"].ToString());
                    normalCDR.Id = Helper.AsInt(reader["Id"].ToString());
                    normalCDR.IMSI = reader["IMSI"].ToString();
                    normalCDR.DurationInSeconds = Helper.AsDecimal(reader["DurationInSeconds"].ToString());
                    normalCDR.DisconnectDateTime = Helper.AsDateTime(reader["DisconnectDateTime"].ToString());
                    normalCDR.CallClass = reader["Call_Class"].ToString();
                    normalCDR.IsOnNet = Helper.AsShortInt(reader["IsOnNet"].ToString());
                    normalCDR.SubType = reader["Sub_Type"].ToString();
                    normalCDR.IMEI = reader["IMEI"].ToString();
                    normalCDR.CellId = reader["Cell_Id"].ToString();
                    normalCDR.SwitchRecordId = Helper.AsInt(reader["SwitchRecordId"].ToString());
                    normalCDR.UpVolume = Helper.AsDecimal(reader["Up_Volume"].ToString());
                    normalCDR.DownVolume = Helper.AsDecimal(reader["Down_Volume"].ToString());
                    normalCDR.CellLatitude = Helper.AsDecimal(reader["Cell_Latitude"].ToString());
                    normalCDR.CellLongitude = Helper.AsDecimal(reader["Cell_Longitude"].ToString());
                    normalCDR.InTrunk = reader["In_Trunk"].ToString();
                    normalCDR.OutTrunk = reader["Out_Trunk"].ToString();
                    normalCDR.ServiceType = Helper.AsInt(reader["Service_Type"].ToString());
                    normalCDR.ServiceVASName = reader["Service_VAS_Name"].ToString();

                    currentIndex++;
                    if (currentIndex == 10000)
                    {
                        count += currentIndex;
                        currentIndex = 0;
                        Console.WriteLine("{0} rows read", count);
                    }

                    onBatchReady(normalCDR);
                }



            });
        }
    }
}
