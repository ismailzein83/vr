using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class SuspiciousNumberDataManager : BaseSQLDataManager, ISuspiciousNumberDataManager
    {
        public SuspiciousNumberDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void SaveSuspiciousNumbers(List<SuspiciousNumber> suspiciousNumbers)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (SuspiciousNumber suspiciousNumber in suspiciousNumbers)
            {
                List<string> sValues = new List<string>();

                for (int i = 1; i <= 15; i++)
                {
                    if (suspiciousNumber.CriteriaValues.Where(x => x.Key == i).Count() == 1)
                    {
                        sValues.Add(Math.Round(suspiciousNumber.CriteriaValues.Where(x => x.Key == i).FirstOrDefault().Value).ToString());
                    }
                    else
                    {
                        sValues.Add("");
                    }
                }

               

                stream.WriteRecord("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}",
                             new[] { suspiciousNumber.DateDay.Value.ToString(), suspiciousNumber.Number.ToString(), sValues[0], sValues[1], sValues[2], sValues[3], sValues[4], sValues[5], sValues[6], sValues[7], sValues[8], sValues[9], sValues[10], sValues[11], sValues[12], sValues[13], sValues[14], suspiciousNumber.SuspectionLevel.ToString(), suspiciousNumber.StrategyId.ToString(), ((int)suspiciousNumber.Period).ToString() }
               );

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
