using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Data;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.RDB
{
    public class RouteOptionsDataManager : IRouteOptionsDataManager
    {
        #region Properties/Ctor

        string TABLE_NAME;
        string TABLE_Schema;
        string RouteOptionsTableName = "RouteOptions";

        string TABLE_ALIAS = "ro";

        const string COL_RouteId = "RouteId";
        const string COL_Options = "Options";
        const string COL_Synced = "Synced";

        private Dictionary<string, RDBTableColumnDefinition> RDBTableColumnDefinitionDictionary = new Dictionary<string, RDBTableColumnDefinition>()
            {
                { COL_RouteId, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt }},
                { COL_Options, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar}},
                { COL_Synced, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean }}
            };

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        string _switchId;

        public string SwitchId
        {
            get
            {
                return _switchId;
            }
            set
            {
                _switchId = value;
                TABLE_Schema = $"WhS_RouteSync_HuaweiSoftX3000_{value}";
                TABLE_NAME = ($"{TABLE_Schema}.[{RouteOptionsTableName}]");
            }
        }

        #endregion

        #region First Step

        #region Public Methods

        public void Initialize(IRouteOptionsInitializeContext context)
        {
            TryRegisterTable();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var CheckIfTableExistsQuery = queryContext.AddCheckIfTableExistsQuery();
            CheckIfTableExistsQuery.TableName(($"{TABLE_Schema}.[{RouteOptionsTableName}]"));

            if (queryContext.ExecuteScalar().IntValue > 0)
                return;

            var createTableQueryContext = new RDBQueryContext(GetDataProvider());

            var createTableQuery = createTableQueryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(TABLE_Schema, RouteOptionsTableName);

            createTableQuery.AddColumn(COL_RouteId, RDBDataType.BigInt, true);
            createTableQuery.AddColumn(COL_Options, RDBDataType.Varchar, true);
            createTableQuery.AddColumn(COL_Synced, RDBDataType.Boolean, true);

            var createIndexQuery = createTableQueryContext.AddCreateIndexQuery();
            createIndexQuery.DBTableName(TABLE_Schema, RouteOptionsTableName);
            createIndexQuery.IndexName($"[PK_{TABLE_Schema}.{RouteOptionsTableName}]");
            createIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createIndexQuery.AddColumn(COL_Synced);

            createTableQueryContext.ExecuteNonQuery();
        }

        #endregion

        #endregion

        #region Second Step

        #region Public Methods
        public List<RouteOptions> GetAllRoutesOptions()
        {
            TryRegisterTable();
            List<RouteOptions> routesOptions = new List<RouteOptions>();

            var queryContext = new RDBQueryContext(GetDataProvider());
            CreateGetRoutesOptionsQuery(queryContext);
            routesOptions = queryContext.GetItems(RouteOptionsMapper);

            return routesOptions.Count > 0 ? routesOptions : null;
        }

        public Dictionary<string, RouteOptions> GetRouteOptionsAfterRouteNumber(long maxRouteNumber)
        {
            TryRegisterTable();

            var queryContext = new RDBQueryContext(GetDataProvider());
            Dictionary<string, RouteOptions> routesOptions = new Dictionary<string, RouteOptions>();

            CreateGetRoutesOptionsQuery(queryContext).GreaterThanCondition(COL_RouteId).Value(maxRouteNumber);

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    RouteOptions routeOptions = RouteOptionsMapper(reader);
                    routesOptions.Add(routeOptions.RouteOptionsAsString, routeOptions);
                }
            });

            return routesOptions.Count > 0 ? routesOptions : null;
        }

        public void ApplyRouteOptionsForDB(object preparedRouteOptions)
        {
            preparedRouteOptions.CastWithValidate<RDBBulkInsertQueryContext>("preparedRouteOptions").Apply();
        }

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();

            streamForBulkInsert.IntoTable(TABLE_NAME, '=', COL_RouteId, COL_Options, COL_Synced);

            return streamForBulkInsert;
        }

        public void WriteRecordToStream(RouteOptions record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            if (record.RouteId == default(long))
                throw new ArgumentNullException($"record.RouteId : {record.RouteId}");

            record.RouteOptionsAsString.ThrowIfNull("record.RouteOptionsAsString", record.RouteId);

            recordContext.Value(record.RouteId);
            recordContext.Value(record.RouteOptionsAsString);
            recordContext.Value(false);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        #endregion

        #endregion

        #endregion

        #region Third Step

        public List<RouteOptions> GetNotSyncedRoutesOptions()
        {
            TryRegisterTable();

            var queryContext = new RDBQueryContext(GetDataProvider());
            CreateGetRoutesOptionsQuery(queryContext).EqualsCondition(COL_Synced).Value(false);

            return queryContext.GetItems(RouteOptionsMapper);
        }

        public void UpdateSyncedRoutesOptions(IEnumerable<long> routeOptionsIds)
        {
            TryRegisterTable();
            if (routeOptionsIds == null || routeOptionsIds.Count() == 0)
                return;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Synced).Value(true);

            updateQuery.Where().ListCondition(COL_RouteId, RDBListConditionOperator.IN, routeOptionsIds);

            queryContext.ExecuteNonQuery();
        }

        #endregion

        #region Common Private Methods
        private void TryRegisterTable()
        {
            RDBSchemaManager.Current.TryRegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = TABLE_Schema,
                DBTableName = RouteOptionsTableName,
                Columns = RDBTableColumnDefinitionDictionary,
                IdColumnName = COL_RouteId
            });
        }

        private RDBConditionContext CreateGetRoutesOptionsQuery(RDBQueryContext queryContext)
        {
            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_RouteId, COL_Options);

            return selectQuery.Where();
        }

        #endregion

        #region Mappers

        private RouteOptions RouteOptionsMapper(IRDBDataReader reader)
        {
            return new RouteOptions()
            {
                RouteId = reader.GetLong(COL_RouteId),
                RouteOptionsAsString = reader.GetString(COL_Options)
            };
        }

        #endregion
    }
}
