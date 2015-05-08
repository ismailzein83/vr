using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Vanrise.Data.SQL;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class SuspiciousNumberDataManager : BaseSQLDataManager, ISuspiciousNumberDataManager
    {
        public SuspiciousNumberDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void SaveSuspiciousNumbers(List<SuspiciousNumber> suspiciousNumbers, Strategy strategy)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (SuspiciousNumber suspiciousNumber in suspiciousNumbers)
            {
                string sValues = "";

                for (int i = 1; i <= 15; i++)
                {
                    if (suspiciousNumber.CriteriaValues.Where(x => x.Key == i).Count() == 1)
                    {
                        sValues = sValues + ", '" + suspiciousNumber.CriteriaValues.Where(x => x.Key == i).FirstOrDefault().Value.ToString() + "'";
                    }
                    else
                    {
                        sValues = sValues + ", null";
                    }
                }

                stream.WriteRecord("'0', '" + suspiciousNumber.DateDay.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + suspiciousNumber.Number + "' " + sValues + ", '" + suspiciousNumber.SuspectionLevel.ToString() + "', '" + strategy.Id.ToString() + "', '" + suspiciousNumber.PeriodId.ToString() + "'");

            }

            stream.Close();

            InsertBulkToTable(
                new StreamBulkInsertInfo
                {
                    TableName = "[dbo].[SubscriberThresholds]",
                    Stream = stream,
                    TabLock = false,
                    KeepIdentity = false,
                    FieldSeparator = ','
                });
        }

    }
}
