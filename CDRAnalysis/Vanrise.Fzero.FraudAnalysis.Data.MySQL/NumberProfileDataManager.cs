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
