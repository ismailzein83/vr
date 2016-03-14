using System.Collections.Generic;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class RelatedNumberDataManager : BaseSQLDataManager, IRelatedNumberDataManager
    {
      
        #region ctor
        public RelatedNumberDataManager()
            : base("CDRDBConnectionString")
        {

        }
        #endregion
        
        #region Public Methods
        public void CreateTempTable()
        {
            ExecuteNonQuerySP("FraudAnalysis.sp_RelatedNumber_CreateTempTable");
        }
        public void SavetoDB(AccountRelatedNumberDictionary records)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (KeyValuePair<string, HashSet<string>> record in records)
            {
                if (record.Value.Count > 0)
                    stream.WriteRecord("{0}*{1}",
                                    record.Key,
                                    string.Join<string>(",", record.Value)
                                    );
            }

            stream.Close();

            InsertBulkToTable(
                new StreamBulkInsertInfo
                {
                    TableName = "[FraudAnalysis].[RelatedNumber_temp]",
                    Stream = stream,
                    TabLock = true,
                    KeepIdentity = false,
                    FieldSeparator = '*'
                });
        }
        public void SwapTableWithTemp()
        {
            ExecuteNonQuerySP("FraudAnalysis.sp_RelatedNumber_SwapTableWithTemp");
        }

        public List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber)
        {
            string result = ExecuteScalarSP("FraudAnalysis.sp_RelatedNumber_GetRelatedNumbersByAccountNumber", accountNumber) as string;

            List<RelatedNumber> list = new List<RelatedNumber>();

            if (result != null)
            {
                List<string> relatedNumbers = result.ToString().Split(',').ToList();

                foreach (string number in relatedNumbers)
                {
                    list.Add(new RelatedNumber() { AccountNumber = number });
                }
            }

            return list;

        }
        #endregion
       
    }
}
