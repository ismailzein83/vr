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
            string query_GetCDRRange = "";
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
                        countOutCalls = Helper.AsNullableInt(reader["countOutCalls"].ToString()),
                        subscriberNumber = reader["subscriberNumber"].ToString(),
                        fromDate = Helper.AsNullableDateTime(reader["fromDate"].ToString()),
                        toDate = Helper.AsNullableDateTime(reader["toDate"].ToString()),
                        diffOutputNumb = Helper.AsNullableInt(reader["diffOutputNumb"].ToString()),
                        countOutInter = Helper.AsNullableInt(reader["countOutInter"].ToString()),
                        countInInter = Helper.AsNullableInt(reader["countInInter"].ToString()),
                        callOutDurAvg = Helper.AsNullableDecimal(reader["callOutDurAvg"].ToString()),
                        countOutFail = Helper.AsNullableInt(reader["countOutFail"].ToString()),
                        countInFail = Helper.AsInt(reader["countInFail"].ToString()),
                        totalOutVolume = Helper.AsNullableDecimal(reader["totalOutVolume"].ToString()),
                        totalInVolume = Helper.AsDecimal(reader["totalInVolume"].ToString()),
                        diffInputNumbers = Helper.AsInt(reader["diffInputNumbers"].ToString()),
                        countOutSMS = Helper.AsNullableInt(reader["countOutSMS"].ToString()),
                        totalIMEI = Helper.AsNullableInt(reader["totalIMEI"].ToString()),
                        totalBTS = Helper.AsNullableInt(reader["totalBTS"].ToString()),
                        isOnNet = Helper.AsNullableInt(reader["isOnNet"].ToString()),
                        totalDataVolume = Helper.AsNullableDecimal(reader["totalDataVolume"].ToString()),
                        periodId = Helper.AsNullableInt(reader["periodId"].ToString()),
                        countInCalls = Helper.AsInt(reader["countInCalls"].ToString()),
                        callInDurAvg = Helper.AsDecimal(reader["callInDurAvg"].ToString()),
                        countOutOnNet = Helper.AsNullableInt(reader["countOutOnNet"].ToString()),
                        countInOnNet = Helper.AsNullableInt(reader["countInOnNet"].ToString()),
                        countOutOffNet = Helper.AsNullableInt(reader["countOutOffNet"].ToString()),
                        countInOffNet = Helper.AsNullableInt(reader["countInOffNet"].ToString()),
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
