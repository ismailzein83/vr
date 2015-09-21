using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class RelatedNumberDataManager : BaseSQLDataManager, IRelatedNumberDataManager
    {
        public RelatedNumberDataManager()
            : base("CDRDBConnectionString")
        {

        }


        public void CreateTempTable()
        {
            ExecuteNonQuerySP("FraudAnalysis.sp_RelatedNumbers_CreateTempTable");
        }


        public void SavetoDB(AccountRelatedNumbers record)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (KeyValuePair<string, HashSet<string>> entry in record)
            {
                stream.WriteRecord("{0}~{1}",
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
                    FieldSeparator = '~'
                });
        }


        public void SwapTableWithTemp()
        {
            ExecuteNonQuerySP("FraudAnalysis.sp_RelatedNumbers_SwapTableWithTemp");
        }
    }
}
