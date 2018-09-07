using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.AccountBalance.Data.RDB
{
    public class BillingTransactionTypeDataManager : IBillingTransactionTypeDataManager
    {
        static string TABLE_NAME = "VR_AccountBalance_BillingTransactionType";

        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_IsCredit = "IsCredit";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";

        static BillingTransactionTypeDataManager()
        {
            var columns = new Dictionary<string,RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition{ DataType = RDBDataType.UniqueIdentifier});
            columns.Add(COL_Name, new RDBTableColumnDefinition{ DataType = RDBDataType.NVarchar, Size= 255});
            columns.Add(COL_IsCredit, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_Settings, new RDBTableColumnDefinition {  DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "BillingTransactionType",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
            
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_AccountBalance", "VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString");
        }

        private BillingTransactionType BillingTransactionTypeMapper(IRDBDataReader reader)
        {
            return new BillingTransactionType
            {
                BillingTransactionTypeId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                IsCredit = reader.GetBooleanWithNullHandling(COL_IsCredit),
                Settings = Common.Serializer.Deserialize<BillingTransactionTypeSettings>(reader.GetString(COL_Settings))
            };
        }

        #endregion

        #region IBillingTransactionTypeDataManager
        public IEnumerable<BillingTransactionType> GetBillingTransactionTypes()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "transType");
            selectQuery.SelectColumns().AllTableColumns("transType");            
            return queryContext.GetItems(BillingTransactionTypeMapper);
            //return new RDBQueryContext(GetDataProvider())
            //    .Select()
            //    .From(TABLE_NAME, "transType")
            //    .SelectColumns().AllTableColumns("transType").EndColumns()
            //    .EndSelect()
            //    .GetItems(BillingTransactionTypeMapper);
        }

        public bool AreBillingTransactionTypeUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
