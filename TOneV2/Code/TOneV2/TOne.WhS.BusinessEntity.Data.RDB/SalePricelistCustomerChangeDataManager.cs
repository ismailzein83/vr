using Vanrise.Entities;
using Vanrise.Data.RDB;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SalePricelistCustomerChangeDataManager
    {
        #region RDB

        static string TABLE_ALIAS = "spcustc";
        static string TABLE_NAME = "TOneWhS_BE_SalePricelistCustomerChange";
        const string COL_CustomerID = "CustomerID";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_CreatedTime = "CreatedTime";


        internal const string COL_BatchID = "BatchID";
        internal const string COL_CountryID = "CountryID";
        internal const string COL_PricelistID = "PricelistID";

        static SalePricelistCustomerChangeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_BatchID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_PricelistID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_CountryID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_CustomerID, new RDBTableColumnDefinition {DataType = RDBDataType.Int});
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime});
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SalePricelistCustomerChange",
                Columns = columns,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }
        #endregion

        #region Public Methods
        public void JoinCustomerChange(RDBJoinContext joinContext, string customerChangeTableAlias, string originalTableAlias, string originalTableCountryIdCol, string originalTableBatchIdCol)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, customerChangeTableAlias);
            joinStatement.JoinType(RDBJoinType.Inner);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTableCountryIdCol, customerChangeTableAlias, COL_CountryID);
            joinCondition.EqualsCondition(originalTableAlias, originalTableBatchIdCol, customerChangeTableAlias, COL_BatchID);
        }

        public void BuildInsertQuery(RDBInsertQuery insertQuery)
        {
            insertQuery.IntoTable(TABLE_NAME);
        }
        #endregion
    }
}
