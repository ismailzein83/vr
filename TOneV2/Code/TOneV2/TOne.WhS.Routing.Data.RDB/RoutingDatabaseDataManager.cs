using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Data.RDB
{
    public class RoutingDatabaseDataManager : IRoutingDatabaseDataManager
    {
        #region Fields/Ctor

        static string TABLE_NAME = "TOneWhS_Routing_RoutingDatabase";
        static string TABLE_ALIAS = "rd";

        const string COL_ID = "ID";
        const string COL_Title = "Title";
        const string COL_Type = "Type";
        const string COL_ProcessType = "ProcessType";
        const string COL_EffectiveTime = "EffectiveTime";
        const string COL_Settings = "Settings";
        const string COL_Information = "Information";
        const string COL_IsReady = "IsReady";
        const string COL_IsDeleted = "IsDeleted";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_ReadyTime = "ReadyTime";
        const string COL_DeletedTime = "DeletedTime";

        static RoutingDatabaseDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Type, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ProcessType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_EffectiveTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Information, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_IsReady, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_IsDeleted, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_ReadyTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_DeletedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_Routing",
                DBTableName = "RoutingDatabase",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create New Routing Database.
        /// </summary>
        /// <param name="name">Routing Database Name</param>
        /// <param name="type">Routing Database Type</param>
        /// <param name="effectiveTime">Effective Date</param>
        /// <returns>Created Routing Database Id</returns>
        public int CreateDatabase(string name, RoutingDatabaseType type, RoutingProcessType processType, DateTime? effectiveTime, RoutingDatabaseInformation information,
            RoutingDatabaseSettings settings, IEnumerable<RoutingCustomerInfo> routingCustomerInfos)
        {
            int routingDatabaseId;
            if (this.Insert(name, type, processType, effectiveTime, information, out routingDatabaseId))
            {
                if (settings == null)
                    settings = new RoutingDatabaseSettings();

                settings.DatabaseName = new RoutingDataManager().CreateDatabase(routingDatabaseId, processType, routingCustomerInfos);

                this.UpdateSettings(routingDatabaseId, settings);
                return routingDatabaseId;
            }
            else
            {
                throw new Exception(String.Format("Could not add Routing Database '{0}' to database table", name));
            }
        }

        /// <summary>
        /// Update Routing Ready Status to true.
        /// </summary>
        /// <param name="databaseId">Routing Database Id</param>
        /// <returns>Returns true if operation is success.</returns>
        public bool SetReady(int databaseId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_IsReady).Value(true);
            updateQuery.Column(COL_ReadyTime).Value(DateTime.Now);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_ID).Value(databaseId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// Get a list of Routing Databases that are not deleted.
        /// </summary>
        /// <returns>List of available Routing Databases</returns>
        public List<RoutingDatabase> GetNotDeletedDatabases()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where(RDBConditionGroupOperator.OR);
            whereContext.ConditionIfColumnNotNull(COL_IsDeleted).EqualsCondition(COL_IsDeleted).Value(false);

            return queryContext.GetItems<RoutingDatabase>(RoutingDatabaseMapper);
        }

        /// <summary>
        /// Drop Routing Database by Id.
        /// </summary>
        /// <param name="databaseId">Routing Database Id</param>
        public void DropDatabase(RoutingDatabase routingDatabase)
        {
            //Check if Delete succeeded
            this.Delete(routingDatabase.ID);

            RoutingDataManager routingDataManager = new RoutingDataManager();
            routingDataManager.RoutingDatabase = routingDatabase;
            routingDataManager.DropDatabaseIfExists();
        }

        public bool AreRoutingDatabasesUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public RoutingDatabase GetRoutingDatabase(int routingDatabaseId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var whereContext = selectQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(routingDatabaseId);

            return queryContext.GetItem<RoutingDatabase>(RoutingDatabaseMapper);
        }

        #endregion

        #region Private Methods

        private bool Insert(string name, RoutingDatabaseType type, RoutingProcessType processType, DateTime? effectiveTime, RoutingDatabaseInformation information, out int routingDatabaseId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);

            insertQuery.Column(COL_Title).Value(name);
            insertQuery.Column(COL_Type).Value((int)type);
            insertQuery.Column(COL_ProcessType).Value((int)processType);

            if (effectiveTime.HasValue)
                insertQuery.Column(COL_EffectiveTime).Value(effectiveTime.Value.ToString());

            if (information != null)
                insertQuery.Column(COL_Information).Value(Vanrise.Common.Serializer.Serialize(information));

            insertQuery.AddSelectGeneratedId();

            int? returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                routingDatabaseId = returnedValue.Value;
                return true;
            }

            routingDatabaseId = -1;
            return false;
        }

        private bool UpdateSettings(int routingDatabaseId, RoutingDatabaseSettings settings)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            if (settings != null)
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(settings));
            else
                updateQuery.Column(COL_Settings).Null();

            var whereContext = updateQuery.Where();
            whereContext.EqualsCondition(COL_ID).Value(routingDatabaseId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        private bool Delete(int routingDatabaseId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_IsDeleted).Value(true);
            updateQuery.Column(COL_DeletedTime).Value(DateTime.Now);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_ID).Value(routingDatabaseId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        private BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_Routing", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        private RoutingDatabase RoutingDatabaseMapper(IRDBDataReader reader)
        {
            RoutingDatabase routingDatabase = new RoutingDatabase
            {
                ID = (int)reader.GetInt("ID"),
                Title = reader.GetString("Title"),
                Type = (RoutingDatabaseType)reader.GetInt("Type"),
                ProcessType = (RoutingProcessType)reader.GetInt("ProcessType"),
                EffectiveTime = reader.GetNullableDateTime("EffectiveTime"),
                IsReady = reader.GetBoolean("IsReady"),
                CreatedTime = reader.GetDateTime("CreatedTime"),
                ReadyTime = reader.GetDateTime("ReadyTime")
            };

            string serializedSettings = reader.GetString(COL_Settings);
            if (!string.IsNullOrEmpty(serializedSettings))
                routingDatabase.Settings = Vanrise.Common.Serializer.Deserialize<RoutingDatabaseSettings>(serializedSettings);

            string serializedInformation = reader.GetString(COL_Information);
            if (!string.IsNullOrEmpty(serializedInformation))
                routingDatabase.Information = Vanrise.Common.Serializer.Deserialize<RoutingDatabaseInformation>(serializedInformation);

            return routingDatabase;
        }

        #endregion
    }
}