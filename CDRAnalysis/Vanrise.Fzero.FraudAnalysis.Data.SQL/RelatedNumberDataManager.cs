using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class RelatedNumberDataManager : BaseSQLDataManager, IRelatedNumberDataManager
    {
        public RelatedNumberDataManager()
            : base("CDRDBConnectionString")
        {

        }


        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[FraudAnalysis].[RelatedNumbers]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^'
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void ApplyAccountRelatedNumbersToDB(object preparedAccountRelatedNumbers)
        {
            InsertBulkToTable(preparedAccountRelatedNumbers as BaseBulkInsertInfo);
        }


        public void WriteRecordToStream(AccountRelatedNumbers record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;

            foreach (KeyValuePair<string, HashSet<string>> entry in record)
            {
                streamForBulkInsert.WriteRecord("{0}^{1}",
                                entry.Key,
                                string.Join<string>(",", entry.Value)
                                );
            }



        }


        public void SavetoDB(AccountRelatedNumbers record)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            WriteRecordToStream(record, dbApplyStream);

            Object preparedItemsForDBApply = FinishDBApplyStream(dbApplyStream);

            ApplyAccountRelatedNumbersToDB(preparedItemsForDBApply);
        }




    }
}
