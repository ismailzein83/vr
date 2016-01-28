using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Linq;

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
            ExecuteNonQuerySP("FraudAnalysis.sp_RelatedNumbers_CreateTempTable");
        }
        public void SavetoDB(AccountRelatedNumbers record)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (KeyValuePair<string, HashSet<string>> entry in record)
            {
                if (entry.Value.Count > 0)
                    stream.WriteRecord("{0}*{1}",
                                    entry.Key,
                                    string.Join<string>(",", entry.Value)
                                    );
            }

            stream.Close();

            InsertBulkToTable(
                new StreamBulkInsertInfo
                {
                    TableName = "[FraudAnalysis].[RelatedNumbers_temp]",
                    Stream = stream,
                    TabLock = true,
                    KeepIdentity = false,
                    FieldSeparator = '*'
                });
        }
        public void SwapTableWithTemp()
        {
            ExecuteNonQuerySP("FraudAnalysis.sp_RelatedNumbers_SwapTableWithTemp");
        }

        public List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber)
        {
            string result = ExecuteScalarSP("FraudAnalysis.sp_RelatedNumbers_GetRelatedNumbersByAccountNumber", accountNumber) as string;

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
