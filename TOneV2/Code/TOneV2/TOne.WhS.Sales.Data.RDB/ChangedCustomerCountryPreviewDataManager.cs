using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using TOne.WhS.Sales.Entities;
using System.Collections.Generic;
using System.Linq;

namespace TOne.WhS.Sales.Data.RDB
{
    public class ChangedCustomerCountryPreviewDataManager : IChangedCustomerCountryPreviewDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "ccp";
        static string TABLE_NAME = "TOneWhS_Sales_RP_CustomerCountry_ChangedPreview";
        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_EED = "EED";
        const string COL_CustomerID = "CustomerID";


        static ChangedCustomerCountryPreviewDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CustomerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_Sales",
                DBTableName = "RP_CustomerCountry_ChangedPreview",
                Columns = columns
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_Sales", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region Members

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceId = value;
            }
        }


        public IEnumerable<ChangedCustomerCountryPreview> GetChangedCustomerCountryPreviews(RatePlanPreviewQuery query)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(query.ProcessInstanceId);

            if (query.CustomerIds != null && query.CustomerIds.Any())
                whereContext.ListCondition(COL_CustomerID, RDBListConditionOperator.IN, query.CustomerIds);

            return queryContext.GetItems(ChangedCustomerCountryPreviewMapper);
        }

        public IEnumerable<int> GetAffectedCustomerIds(long processInstanceId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_CustomerID);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ProcessInstanceID).Value(processInstanceId);

            var groupByContext = selectQuery.GroupBy();
            groupByContext.Select().Column(COL_CustomerID);

            var lstAffectedCustomerIds = new List<int>();
            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    lstAffectedCustomerIds.Add(reader.GetInt(COL_CustomerID));
                }
            });
            return lstAffectedCustomerIds;
        }

        public void ApplyChangedCustomerCountryPreviewsToDB(IEnumerable<ChangedCustomerCountryPreview> changedCustomerCountryPreviews)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (var customerCountry in changedCustomerCountryPreviews)
            {
                WriteRecordToStream(customerCountry, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }

        #endregion

        #region Bulk
        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', COL_ID, COL_CustomerID, COL_ProcessInstanceID, COL_EED);
            return streamForBulkInsert;
        }
        public void WriteRecordToStream(ChangedCustomerCountryPreview record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            recordContext.Value(record.CountryId);
            recordContext.Value(record.CustomerId);
            recordContext.Value(_processInstanceId);
            recordContext.Value(record.EED);
        }

        public object FinishDBApplyStream(object dbApplyStream)
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

        #region Mappers

        private ChangedCustomerCountryPreview ChangedCustomerCountryPreviewMapper(IRDBDataReader reader)
        {
            return new ChangedCustomerCountryPreview
            {
                CountryId = reader.GetInt(COL_ID),
                CustomerId = reader.GetIntWithNullHandling(COL_CustomerID),
                EED = reader.GetDateTime(COL_EED)
            };
        }

        #endregion
    }
}
