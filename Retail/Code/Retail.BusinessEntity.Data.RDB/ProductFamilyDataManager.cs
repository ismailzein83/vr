//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Retail.BusinessEntity.Entities;
//using Vanrise.Data.RDB;
//using Vanrise.Entities;

//namespace Retail.BusinessEntity.Data.RDB
//{
//    public class ProductFamilyDataManager : IProductFamilyDataManager
//    {
//        #region Local Variables
//        static string TABLE_NAME = "Retail_BE_ProductFamily";
//        static string TABLE_ALIAS = "vrProductFamily";
//        const string COL_ID = "ID";
//        const string COL_Name = "Name";
//        const string COL_Settings = "Settings";
//        const string COL_CreatedTime = "CreatedTime";
//        const string COL_LastModifiedTime = "LastModifiedTime";
//        #endregion

//        #region Constructors
//        static ProductFamilyDataManager()
//        {
//            var columns = new Dictionary<string, RDBTableColumnDefinition>();
//            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
//            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
//            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
//            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
//            {
//                DBSchemaName = "Retail_BE",
//                DBTableName = "ProductFamily",
//                Columns = columns,
//                CreatedTimeColumnName = COL_CreatedTime,
//                ModifiedTimeColumnName = COL_LastModifiedTime
//            });
//        }
//        #endregion

//        #region Public Methods
//        public bool AreProductFamilyUpdated(ref object updateHandle)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
//        }

//        public List<ProductFamily> GetProductFamilies()
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var selectQuery = queryContext.AddSelectQuery();
//            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
//            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
//            return queryContext.GetItems(ProductFamilyMapper);
//        }

//        public bool Insert(ProductFamily productFamily, out int insertedId)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var insertQuery = queryContext.AddInsertQuery();
//            insertQuery.IntoTable(TABLE_NAME);

//            var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
//            ifNotExist.EqualsCondition(COL_Name).Value(productFamily.Name);

//            insertQuery.AddSelectGeneratedId();
//            insertQuery.Column(COL_Name).Value(productFamily.Name);
//            if (productFamily.Settings != null)
//                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(productFamily.Settings));
//            else
//                insertQuery.Column(COL_Settings).Null();

//            var insertedID = queryContext.ExecuteScalar().NullableIntValue;
//            if (insertedID.HasValue)
//            {
//                insertedId = insertedID.Value;
//                return true;
//            }
//            insertedId = -1;
//            return false;
//        }

//        public bool Update(ProductFamily productFamily)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var updateQuery = queryContext.AddUpdateQuery();
//            updateQuery.FromTable(TABLE_NAME);

//            var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
//            ifNotExist.NotEqualsCondition(COL_ID).Value(productFamily.ProductFamilyId);
//            ifNotExist.EqualsCondition(COL_Name).Value(productFamily.Name);

//            updateQuery.Column(COL_Name).Value(productFamily.Name);
//            if (productFamily.Settings != null)
//                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(productFamily.Settings));
//            else
//                updateQuery.Column(COL_Settings).Null();

//            updateQuery.Where().EqualsCondition(COL_ID).Value(productFamily.ProductFamilyId);
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
//        ProductFamily ProductFamilyMapper(IRDBDataReader reader)
//        {
//            ProductFamily productFamily = new ProductFamily
//            {
//                ProductFamilyId = reader.GetInt(COL_ID),
//                Name = reader.GetString(COL_Name),
//                Settings = Vanrise.Common.Serializer.Deserialize<ProductFamilySettings>(reader.GetString(COL_Settings))
//            };
//            return productFamily;
//        }
//        #endregion
//    }
//}
