using System.Collections.Generic;
using System.Data;
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
                    foreach (var relatedNumber in record.Value)
                        stream.WriteRecord("{0}*{1}",
                                        record.Key,
                                        relatedNumber
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
            return GetItemsSP("FraudAnalysis.sp_RelatedNumber_GetRelatedNumbersByAccountNumber", RelatedNumberMapper, accountNumber);
        }

        #endregion

        # region Mappers
        private RelatedNumber RelatedNumberMapper(IDataReader reader)
        {
            RelatedNumber relatedNumber = new RelatedNumber();
            relatedNumber.AccountNumber = reader["RelatedNumber"] as string;
            return relatedNumber;
        }
        # endregion

    }
}
