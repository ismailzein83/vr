//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Retail.BusinessEntity.Entities;
//using Vanrise.Data.RDB;
//using Vanrise.Entities;

//namespace Retail.BusinessEntity.Data.RDB
//{
//    public class AccountPackageDataManager : IAccountPackageDataManager
//    {
//        #region Local Variables
//        static string TABLE_NAME = "Retail_AccountPackage";
//        static string TABLE_ALIAS = "vrAccountPackage";
//        const string COL_ID = "ID";
//        const string COL_AccountID = "AccountID";
//        const string COL_PackageID = "PackageID";
//        const string COL_AccountBEDefinitionId = "AccountBEDefinitionId";
//        const string COL_BED = "BED";
//        const string COL_EED = "EED";
//        const string COL_CreatedTime = "CreatedTime";
//        const string COL_LastModifiedTime = "LastModifiedTime";
//        #endregion

//        #region Constructors
//        static AccountPackageDataManager()
//        {
//            var columns = new Dictionary<string, RDBTableColumnDefinition>();
//            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
//            columns.Add(COL_AccountID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
//            columns.Add(COL_PackageID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
//            columns.Add(COL_AccountBEDefinitionId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
//            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
//            {
//                DBSchemaName = "Retail",
//                DBTableName = "AccountPackage",
//                Columns = columns,
//                IdColumnName = COL_ID,
//                CreatedTimeColumnName = COL_CreatedTime,
//                ModifiedTimeColumnName = COL_LastModifiedTime
//            });
//        }
//        #endregion

//        #region Public Methods

//        public bool AreAccountPackagesUpdated(ref object updateHandle)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
//        }

//        public IEnumerable<AccountPackage> GetAccountPackages()
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var selectQuery = queryContext.AddSelectQuery();
//            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
//            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
//            return queryContext.GetItems(AccountPackageMapper);
//        }

//        public bool Insert(AccountPackage accountPackage, out long insertedId)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var insertQuery = queryContext.AddInsertQuery();
//            insertQuery.IntoTable(TABLE_NAME);

//            insertQuery.AddSelectGeneratedId();
//            insertQuery.Column(COL_AccountID).Value(accountPackage.AccountId);
//            insertQuery.Column(COL_PackageID).Value(accountPackage.PackageId);
//            insertQuery.Column(COL_AccountBEDefinitionId).Value(accountPackage.AccountBEDefinitionId);
//            insertQuery.Column(COL_BED).Value(accountPackage.BED);
//            if (accountPackage.EED.HasValue)
//                insertQuery.Column(COL_EED).Value(accountPackage.EED.Value);

//            var insertedID = queryContext.ExecuteScalar().NullableLongValue;
//            if (insertedID.HasValue)
//            {
//                insertedId = insertedID.Value;
//                return true;
//            }
//            insertedId = -1;
//            return false;
//        }

//        public bool Update(AccountPackageToEdit accountPackage)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var updateQuery = queryContext.AddUpdateQuery();
//            updateQuery.FromTable(TABLE_NAME);

//            updateQuery.Column(COL_BED).Value(accountPackage.BED);
//            if (accountPackage.EED.HasValue)
//                updateQuery.Column(COL_EED).Value(accountPackage.EED.Value);

//            updateQuery.Where().EqualsCondition(COL_ID).Value(accountPackage.AccountPackageId);
//            return queryContext.ExecuteNonQuery() > 0;
//        }

//        #endregion

//        #region Private Methods
//        private BaseRDBDataProvider GetDataProvider()
//        {
//            return RDBDataProviderFactory.CreateProvider("Retail_BE", "Retail_BE_DBConnStringKey", "RetailDBConnString");
//        }
//        #endregion

//        #region Mappers
//        private AccountPackage AccountPackageMapper(IRDBDataReader reader)
//        {
//            return new AccountPackage()
//            {
//                AccountPackageId = reader.GetLong(COL_ID),
//                AccountId = reader.GetLong(COL_AccountID),
//                PackageId = reader.GetInt(COL_PackageID),
//                AccountBEDefinitionId = reader.GetGuid(COL_AccountBEDefinitionId),
//                BED = reader.GetDateTime(COL_BED),
//                EED = reader.GetNullableDateTime(COL_EED)
//            };
//        }
//        #endregion
//    }
//}
