//using Retail.BusinessEntity.Entities;
//using System.Collections.Generic;
//using Vanrise.Data.RDB;
//using Vanrise.Entities;

//namespace Retail.BusinessEntity.Data.RDB
//{
//    public class PackageDataManager : IPackageDataManager
//    {
//        #region Local Variables
//        static string TABLE_NAME = "Retail_Package";
//        static string TABLE_ALIAS = "vrPackage";
//        const string COL_ID = "ID";
//        const string COL_Name = "Name";
//        const string COL_Description = "Description";
//        const string COL_Settings = "Settings";
//        const string COL_CreatedTime = "CreatedTime";
//        const string COL_LastModifiedTime = "LastModifiedTime";
//        #endregion

//        #region Constructors
//        static PackageDataManager()
//        {
//            var columns = new Dictionary<string, RDBTableColumnDefinition>();
//            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
//            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
//            columns.Add(COL_Description, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
//            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
//            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
//            {
//                DBSchemaName = "Retail",
//                DBTableName = "Package",
//                Columns = columns,
//                IdColumnName = COL_ID,
//                CreatedTimeColumnName = COL_CreatedTime,
//                ModifiedTimeColumnName = COL_LastModifiedTime

//            });
//        }
//        #endregion

//        #region Public Methods
//        public bool ArePackagesUpdated(ref object updateHandle)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
//        }

//        public List<Package> GetPackages()
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var selectQuery = queryContext.AddSelectQuery();
//            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
//            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
//            return queryContext.GetItems(PackageMapper);
//        }

//        public bool Insert(Package package, out int insertedId)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var insertQuery = queryContext.AddInsertQuery();
//            insertQuery.IntoTable(TABLE_NAME);

//            var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
//            ifNotExist.EqualsCondition(COL_Name).Value(package.Name);

//            insertQuery.AddSelectGeneratedId();
//            insertQuery.Column(COL_Name).Value(package.Name);
//            insertQuery.Column(COL_Description).Value(package.Description);
//            if (package.Settings != null)
//                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(package.Settings));
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

//        public bool Update(Package package)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var updateQuery = queryContext.AddUpdateQuery();
//            updateQuery.FromTable(TABLE_NAME);

//            var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
//            ifNotExist.NotEqualsCondition(COL_ID).Value(package.PackageId);
//            ifNotExist.EqualsCondition(COL_Name).Value(package.Name);

//            updateQuery.Column(COL_Name).Value(package.Name);
//            if (package.Settings != null)
//                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(package.Settings));
//            else
//                updateQuery.Column(COL_Settings).Null();

//            updateQuery.Column(COL_Description).Value(package.Description);

//            updateQuery.Where().EqualsCondition(COL_ID).Value(package.PackageId);
//            return queryContext.ExecuteNonQuery() > 0;
//        }

//        #endregion

//        #region Private Methods
         
//        private BaseRDBDataProvider GetDataProvider()
//        {
//            return RDBDataProviderFactory.CreateProvider("Retail_BE", "Retail_BE_DBConnStringKey", "Retail_BE_DBConnString");
//        }

//        #endregion

//        #region Mappers
//        private Package PackageMapper(IRDBDataReader reader)
//        {
//            Package package = new Package
//            {
//                PackageId = reader.GetInt(COL_ID),
//                Description = reader.GetString(COL_Description),
//                Name = reader.GetString(COL_Name),
//                Settings = Vanrise.Common.Serializer.Deserialize<PackageSettings>(reader.GetString(COL_Settings))
//            };
//            return package;
//        }

//        #endregion

//    }
//}
