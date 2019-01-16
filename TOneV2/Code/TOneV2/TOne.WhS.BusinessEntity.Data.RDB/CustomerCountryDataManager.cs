using System;
using Vanrise.Common;
using Vanrise.Data.RDB;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class CustomerCountryDataManager : ICustomerCountryDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "cc";
        static string TABLE_NAME = "TOneWhS_BE_CustomerCountry";
        public const string COL_ID = "ID";
        const string COL_CustomerID = "CustomerID";
        const string COL_CountryID = "CountryID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_ProcessInstanceID = "ProcessInstanceID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";


        static CustomerCountryDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CustomerID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_CountryID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_ProcessInstanceID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CustomerCountry",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region ICustomerCountryDataManager Members

        public IEnumerable<CustomerCountry2> GetAll()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(CustomerCountryMapper);
        }

        public bool AreAllCustomerCountriesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        #endregion

        #region StateBackup Methods

        //Not Yet implemented

        #endregion

        #region Mappers

        private CustomerCountry2 CustomerCountryMapper(IRDBDataReader reader)
        {
            return new CustomerCountry2
            {
                CustomerCountryId = reader.GetInt(COL_ID),
                CustomerId = reader.GetInt(COL_CustomerID),
                CountryId = reader.GetInt(COL_CountryID),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED),
                ProcessInstanceId = reader.GetNullableLong(COL_ProcessInstanceID)
            };
        }

        #endregion

        #region Public Methods
        public void BuildUpdateQuery(RDBUpdateQuery updateQuery, long processInstanceID, string joinTableAlias, string columnName)
        {
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_EED).Column(joinTableAlias, COL_EED);
            updateQuery.Where().EqualsCondition(joinTableAlias, columnName).Value(processInstanceID);
        }
        #endregion
    }
}
