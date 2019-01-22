using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePricelistCustomerChangeNewDataManager
    {
        #region RDB
        static string TABLE_NAME = "TOneWhS_BE_SalePricelistCustomerChange_New";
        static string TABLE_ALIAS = "spcucnew";
        const string COL_BatchID = "BatchID";
        const string COL_PricelistID = "PricelistID";
        const string COL_CountryID = "CountryID";
        const string COL_CustomerID = "CustomerID";

        static SalePricelistCustomerChangeNewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_BatchID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_PricelistID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CustomerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePricelistCustomerChange_New",
                Columns = columns
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region Public Methods

        public void Bulk(IEnumerable<SalePriceListCustomerChange> salePriceListsCustomerChange)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (var salePriceListCustomerChange in salePriceListsCustomerChange)
            {
                WriteRecordToStream(salePriceListCustomerChange, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }
        public void DeleteRecords(RDBDeleteQuery deleteQuery, long processInstanceId)
        {
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_BatchID).Value(processInstanceId);
        }

        public void BuildSelectQuery(RDBSelectQuery selectQuery, long processInstanceId)
        {
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_BatchID, COL_PricelistID, COL_CountryID, COL_CustomerID);
            selectQuery.Where().EqualsCondition(COL_BatchID).Value(processInstanceId);
        }
        #endregion

        #region Bulk Methods
        private object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', COL_BatchID, COL_PricelistID, COL_CountryID, COL_CustomerID);
            return streamForBulkInsert;
        }
        private void WriteRecordToStream(SalePriceListCustomerChange record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            recordContext.Value(record.BatchId);
            recordContext.Value(record.PriceListId);
            recordContext.Value(record.CountryId);
            recordContext.Value(record.CustomerId);
        }
        private object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }
        private void ApplyForDB(object preparedObject)
        {
            preparedObject.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();

        }

        #endregion
    }
}
