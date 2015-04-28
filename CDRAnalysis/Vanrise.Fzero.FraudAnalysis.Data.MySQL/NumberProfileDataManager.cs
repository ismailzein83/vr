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

namespace Vanrise.Fzero.FraudAnalysis.Data.MYSQL
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
            string query_GetCDRRange = string.Empty;
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
                        countOutCalls =(int) reader["countOutCalls"],
                        subscriberNumber =(string) reader["subscriberNumber"],
                        fromDate =(DateTime?) reader["fromDate"],
                        toDate =(DateTime?) reader["toDate"],
                        diffOutputNumb =(int?) reader["diffOutputNumb"],
                        countOutInter =(int?) reader["countOutInter"],
                        countInInter =(int?) reader["countInInter"],
                        callOutDurAvg =(decimal?) reader["callOutDurAvg"],
                        countOutFail =(int?) reader["countOutFail"],
                        countInFail =(int) reader["countInFail"],
                        totalOutVolume =(decimal?) reader["totalOutVolume"],
                        totalInVolume =(decimal) reader["totalInVolume"],
                        diffInputNumbers =(int) reader["diffInputNumbers"],
                        countOutSMS =(int?) reader["countOutSMS"],
                        totalIMEI =(int?) reader["totalIMEI"],
                        totalBTS =(int?) reader["totalBTS"],
                        isOnNet =(int?) reader["isOnNet"],
                        totalDataVolume =(decimal?) reader["totalDataVolume"],
                        periodId =(int?) reader["periodId"],              
                        countInCalls =(int) reader["countInCalls"],
                        callInDurAvg =(decimal) reader["callInDurAvg"],
                        countOutOnNet =(int?) reader["countOutOnNet"],
                        countInOnNet =(int?) reader["countInOnNet"],
                        countOutOffNet =(int?) reader["countOutOffNet"],
                        countInOffNet =(int?) reader["countInOffNet"],
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
