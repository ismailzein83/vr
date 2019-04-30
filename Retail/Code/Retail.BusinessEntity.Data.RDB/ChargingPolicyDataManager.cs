//using Retail.BusinessEntity.Entities;
//using System.Collections.Generic;
//using Vanrise.Data.RDB;
//using Vanrise.Entities;

//namespace Retail.BusinessEntity.Data.RDB
//{
//    public class ChargingPolicyDataManager : IChargingPolicyDataManager
//    {
//        #region Local Variables
//        static string TABLE_NAME = "Retail_ChargingPolicy";
//        static string TABLE_ALIAS = "vrChargingPolicy";
//        const string COL_ID = "ID";
//        const string COL_Name = "Name";
//        const string COL_ServiceTypeId = "ServiceTypeId";
//        const string COL_OldServiceTypeId = "OldServiceTypeId";
//        const string COL_Settings = "Settings";
//        const string COL_CreatedTime = "CreatedTime";
//        const string COL_LastModifiedTime = "LastModifiedTime";
//        #endregion

//        #region Constructors
//        static ChargingPolicyDataManager()
//        {
//            var columns = new Dictionary<string, RDBTableColumnDefinition>();
//            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
//            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
//            columns.Add(COL_ServiceTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
//            columns.Add(COL_OldServiceTypeId, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
//            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
//            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
//            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
//            {
//                DBSchemaName = "Retail",
//                DBTableName = "ChargingPolicy",
//                Columns = columns,
//                IdColumnName = COL_ID,
//                CreatedTimeColumnName = COL_CreatedTime,
//                ModifiedTimeColumnName = COL_LastModifiedTime
//            });
//        }

//        #endregion

//        #region Public Methods

//        public bool AreChargingPoliciesUpdated(ref object updateHandle)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
//        }

//        public IEnumerable<ChargingPolicy> GetChargingPolicies()
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var selectQuery = queryContext.AddSelectQuery();
//            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
//            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
//            return queryContext.GetItems(ChargingPolicyMapper);
//        }

//        public bool Insert(ChargingPolicy chargingPolicy, out int insertedId)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var insertQuery = queryContext.AddInsertQuery();
//            insertQuery.IntoTable(TABLE_NAME);

//            var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
//            ifNotExist.EqualsCondition(COL_Name).Value(chargingPolicy.Name);

//            insertQuery.AddSelectGeneratedId();
//            insertQuery.Column(COL_Name).Value(chargingPolicy.Name);
//            insertQuery.Column(COL_ServiceTypeId).Value(chargingPolicy.ServiceTypeId);
//            if (chargingPolicy.Settings != null)
//                insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(chargingPolicy.Settings));
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

//        public bool Update(ChargingPolicyToEdit chargingPolicy)
//        {
//            var queryContext = new RDBQueryContext(GetDataProvider());
//            var updateQuery = queryContext.AddUpdateQuery();
//            updateQuery.FromTable(TABLE_NAME);

//            var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
//            ifNotExist.NotEqualsCondition(COL_ID).Value(chargingPolicy.ChargingPolicyId);
//            ifNotExist.EqualsCondition(COL_Name).Value(chargingPolicy.Name);

//            updateQuery.Column(COL_Name).Value(chargingPolicy.Name);
//            if (chargingPolicy.Settings != null)
//                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(chargingPolicy.Settings));
//            else
//                updateQuery.Column(COL_Settings).Null();

//            updateQuery.Where().EqualsCondition(COL_ID).Value(chargingPolicy.ChargingPolicyId);
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
//        private ChargingPolicy ChargingPolicyMapper(IRDBDataReader reader)
//        {
//            ChargingPolicy chargingPolicy = new ChargingPolicy
//            {
//                ChargingPolicyId = reader.GetInt(COL_ID),
//                Name = reader.GetString(COL_Name),
//                ServiceTypeId = reader.GetGuid(COL_ServiceTypeId),
//                Settings = Vanrise.Common.Serializer.Deserialize<ChargingPolicySettings>(reader.GetString(COL_Settings))
//            };

//            return chargingPolicy;
//        }
//        #endregion

//    }
//}
