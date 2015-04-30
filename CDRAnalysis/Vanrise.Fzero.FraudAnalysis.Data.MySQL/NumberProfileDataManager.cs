using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Data.MySQL;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class NumberProfileDataManager : BaseMySQLDataManager, INumberProfileDataManager
    {
        public NumberProfileDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public void LoadNumberProfile(DateTime from, DateTime to, int? batchSize, Action<List<Vanrise.Fzero.FraudAnalysis.Entities.NumberProfile>> onBatchReady)
        {
            MySQLManager manager =  new MySQLManager();
            string query_GetCDRRange = "SELECT * FROM CDRAnalysisMobile.ts_NumberProfile  where FromDate >= @From and ToDate <=@To    ;";
            manager.ExecuteReader(query_GetCDRRange,
                (cmd) =>
                {
                    cmd.Parameters.AddWithValue("@From", from);
                    cmd.Parameters.AddWithValue("@To", to);
                }, (reader) =>
            {
                List<NumberProfile> numberProfileBatch = new List<NumberProfile>();

                while (reader.Read())
                {
                    NumberProfile numberProfile = new NumberProfile
                    {
                        countOutCalls = Helper.AsNullableInt(reader["Count_Out_Calls"].ToString()),
                        subscriberNumber = reader["SubscriberNumber"].ToString(),
                        fromDate = Helper.AsNullableDateTime(reader["FromDate"].ToString()),
                        toDate = Helper.AsNullableDateTime(reader["ToDate"].ToString()),
                        diffOutputNumb = Helper.AsNullableInt(reader["Diff_Output_Numb"].ToString()),
                        countOutInter = Helper.AsNullableInt(reader["Count_Out_Inter"].ToString()),
                        countInInter = Helper.AsNullableInt(reader["Count_In_Inter"].ToString()),
                        callOutDurAvg = Helper.AsNullableDecimal(reader["Call_Out_Dur_Avg"].ToString()),
                        countOutFail = Helper.AsNullableInt(reader["Count_Out_Fail"].ToString()),
                        countInFail = Helper.AsInt(reader["Count_In_Fail"].ToString()),
                        totalOutVolume = Helper.AsNullableDecimal(reader["Total_Out_Volume"].ToString()),
                        totalInVolume = Helper.AsDecimal(reader["Total_In_Volume"].ToString()),
                        diffInputNumbers = Helper.AsInt(reader["Diff_Input_Numbers"].ToString()),
                        countOutSMS = Helper.AsNullableInt(reader["Count_Out_SMS"].ToString()),
                        totalIMEI = Helper.AsNullableInt(reader["Total_IMEI"].ToString()),
                        totalBTS = Helper.AsNullableInt(reader["Total_BTS"].ToString()),
                        isOnNet = Helper.AsNullableInt(reader["isOnNet"].ToString()),
                        totalDataVolume = Helper.AsNullableDecimal(reader["Total_Data_Volume"].ToString()),
                        periodId = Helper.AsNullableInt(reader["PeriodId"].ToString()),
                        countInCalls = Helper.AsInt(reader["Count_In_Calls"].ToString()),
                        callInDurAvg = Helper.AsDecimal(reader["Call_In_Dur_Avg"].ToString()),
                        countOutOnNet = Helper.AsNullableInt(reader["Count_Out_OnNet"].ToString()),
                        countInOnNet = Helper.AsNullableInt(reader["Count_In_OnNet"].ToString()),
                        countOutOffNet = Helper.AsNullableInt(reader["Count_Out_OffNet"].ToString()),
                        countInOffNet = Helper.AsNullableInt(reader["Count_In_OffNet"].ToString()),
                    };
                    numberProfileBatch.Add(numberProfile);
                    if (batchSize.HasValue && numberProfileBatch.Count == batchSize)
                    {
                        onBatchReady(numberProfileBatch);
                        numberProfileBatch = new List<NumberProfile>();
                    }
                }
                if (numberProfileBatch.Count > 0)
                    onBatchReady(numberProfileBatch);
            });
        }

    }
}
