using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using TOne.WhS.Sales.Entities;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Data.RDB
{
    public class NewCustomerCountryDataManager : INewCustomerCountryDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "cc";
        static string TABLE_NAME = "TOneWhS_Sales_RP_CustomerCountry_New";
        const string COL_ID = "ID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_CustomerID = "CustomerID";
        const string COL_CountryID = "CountryID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";


        static NewCustomerCountryDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_ProcessInstanceID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CustomerID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_Sales",
                DBTableName = "RP_CustomerCountry_New",
                Columns = columns
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_Sales", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region INewCustomerCountryDataManager Members

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

        private long _processInstanceId;

        public long ProcessInstanceId
        {
            set
            {
                _processInstanceId = value;
            }
        }

        public void ApplyNewCustomerCountriesToDB(IEnumerable<NewCustomerCountry> newCustomerCountries)
        {
            object dbApplyStream = InitialiazeStreamForDBApply();

            foreach (var customerCountry in newCustomerCountries)
            {
                WriteRecordToStream(customerCountry, dbApplyStream);
            }

            object prepareToApplyInfo = FinishDBApplyStream(dbApplyStream);
            ApplyForDB(prepareToApplyInfo);
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            streamForBulkInsert.IntoTable(TABLE_NAME, '^', COL_ID, COL_ProcessInstanceID, COL_CustomerID, COL_CountryID, COL_BED, COL_EED);
            return streamForBulkInsert;
        }

        public void WriteRecordToStream(NewCustomerCountry record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            recordContext.Value(record.CustomerCountryId);
            recordContext.Value(_processInstanceId);
            recordContext.Value(record.CustomerId);
            recordContext.Value(record.CountryId);
            recordContext.Value(record.BED);

            if (record.EED != null)
                recordContext.Value(record.EED.Value);
            else
                recordContext.Value(string.Empty);
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
    }
}
