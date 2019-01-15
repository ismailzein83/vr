//using System;
//using System.Collections.Generic;
//using TOne.WhS.RouteSync.Entities;
//using Vanrise.Common;
//using Vanrise.Data.RDB;
//using Vanrise.Entities;

//namespace TOne.WhS.RouteSync.Data.RDB
//{
//    public class RouteSyncDefinitionDataManager : IRouteSyncDefinitionDataManager
//    {
//        static string TABLE_NAME = "TOneWhS_RouteSync_RouteSyncDefinition";
//        static string TABLE_ALIAS = "rsd";

//        const string COL_ID = "ID";
//        const string COL_Name = "Name";
//        const string COL_Settings = "Settings";
//        const string COL_CreatedTime = "CreatedTime";
//        const string COL_LastModifiedTime = "LastModifiedTime";

//        static RouteSyncDefinitionDataManager()
//        {
//            var columns = new Dictionary<string, RDBTableColumnDefinition>();
//            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
//            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
//            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
//            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
//            {
//                DBSchemaName = "TOneWhS_RouteSync",
//                DBTableName = "RouteSyncDefinition",
//                Columns = columns,
//                IdColumnName = COL_ID,
//                CreatedTimeColumnName = COL_CreatedTime,
//                ModifiedTimeColumnName = COL_LastModifiedTime

//            });
//        }
//        BaseRDBDataProvider GetDataProvider()
//        {
//            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
//        }

//        #region Public Methods
//        public bool AreRouteSyncDefinitionsUpdated(ref object lastReceivedDataInfo)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            return queryContext.IsDataUpdated(TABLE_NAME, ref lastReceivedDataInfo);
//        }

//        public List<RouteSync.Entities.RouteSyncDefinition> GetRouteSyncDefinitions()
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());

//            var selectQuery = queryContext.AddSelectQuery();
//            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
//            selectQuery.SelectColumns().Columns(COL_ID, COL_Name, COL_Settings);

//            return queryContext.GetItems(RouteSyncDefinitionMapper);
//        }

//        public bool Insert(RouteSync.Entities.RouteSyncDefinition routeSyncDefinitionItem, out int insertedId)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());

//            var insertQuery = queryContext.AddInsertQuery();
//            insertQuery.IntoTable(TABLE_NAME);
//            insertQuery.AddSelectGeneratedId();
//            insertQuery.Column(COL_Name).Value(routeSyncDefinitionItem.Name);

//            if (routeSyncDefinitionItem.Settings != null)
//                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(routeSyncDefinitionItem.Settings));

//            var ifNotExistContext = insertQuery.IfNotExists(TABLE_ALIAS);
//            ifNotExistContext.EqualsCondition(COL_Name).Value(routeSyncDefinitionItem.Name);

//            var reprocessDefinitionID = queryContext.ExecuteScalar().NullableIntValue;

//            if (reprocessDefinitionID.HasValue)
//            {
//                insertedId = reprocessDefinitionID.Value;
//                return true;
//            }
//            else
//            {
//                insertedId = -1;
//                return false;
//            }
//        }

//        public bool Update(RouteSync.Entities.RouteSyncDefinition routeSyncDefinitionItem)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());

//            var updateQuery = queryContext.AddUpdateQuery();
//            updateQuery.FromTable(TABLE_NAME);
//            updateQuery.Column(COL_Name).Value(routeSyncDefinitionItem.Name);
//            if (routeSyncDefinitionItem.Settings != null)
//                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(routeSyncDefinitionItem.Settings));
//            else
//                updateQuery.Column(COL_Settings).Null();

//            var ifNotExistContext = updateQuery.IfNotExists(TABLE_ALIAS);
//            ifNotExistContext.EqualsCondition(COL_Name).Value(routeSyncDefinitionItem.Name);
//            ifNotExistContext.NotEqualsCondition(COL_ID).Value(routeSyncDefinitionItem.RouteSyncDefinitionId);

//            return queryContext.ExecuteNonQuery() > 0;
//        }
//        #endregion
//        #region Mappers
//        private RouteSyncDefinition RouteSyncDefinitionMapper(IRDBDataReader reader)
//        {
//            return new RouteSyncDefinition()
//            {
//                RouteSyncDefinitionId = reader.GetInt(COL_ID),
//                Name = reader.GetString(COL_Name),
//                Settings = Serializer.Deserialize<RouteSyncDefinitionSettings>(reader.GetString(COL_Settings))
//            };
//        }
//        #endregion
//    }
//}
