using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.RDB
{
    public class RouteCaseDataManager : Data.IRouteCaseDataManager
    {
        #region Properties/Ctor

        string TABLE_NAME;
        string TABLE_Schema;
        string RouteCaseTableName = "RouteCase";

        string TABLE_ALIAS = "rc";

        const string COL_RouteCaseId = "RouteCaseId"; // Route Selection Code
        const string COL_RAN = "RAN"; // Route Analysis Name
        const string COL_RouteId = "RouteId";
        const string COL_RSSC = "RSSC";
        const string COL_Synced = "Synced";

        private Dictionary<string, RDBTableColumnDefinition> RDBTableColumnDefinitionDictionary = new Dictionary<string, RDBTableColumnDefinition>()
            {
                { COL_RouteCaseId, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt }},
                { COL_RAN, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar}},
                { COL_RouteId, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt }},
                { COL_RSSC, new RDBTableColumnDefinition { DataType = RDBDataType.Int }},
                { COL_Synced, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean }}
            };

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
                TABLE_NAME = ($"{TABLE_Schema}.[{RouteCaseTableName}]");
            }
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        #endregion

        #region First Step 

        #region Public Methods 
        public void Initialize(IRouteCaseInitializeContext context)
        {
            TryRegisterTable();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var CheckIfTableExistsQuery = queryContext.AddCheckIfTableExistsQuery();
            CheckIfTableExistsQuery.TableName(($"{TABLE_Schema}.[{RouteCaseTableName}]"));

            if (queryContext.ExecuteScalar().IntValue > 0)
                return;

            var createTableQueryContext = new RDBQueryContext(GetDataProvider());

            var createTableQuery = createTableQueryContext.AddCreateTableQuery();

            createTableQuery.DBTableName(TABLE_Schema, RouteCaseTableName);

            createTableQuery.AddColumn(COL_RouteCaseId, RDBDataType.BigInt);
            createTableQuery.AddColumn(COL_RAN, RDBDataType.Varchar, true);
            createTableQuery.AddColumn(COL_RouteId, RDBDataType.BigInt, true);
            createTableQuery.AddColumn(COL_RSSC, RDBDataType.Int, true);
            createTableQuery.AddColumn(COL_Synced, RDBDataType.Boolean, true);


            var createIndexQuery = createTableQueryContext.AddCreateIndexQuery();
            createIndexQuery.DBTableName(TABLE_Schema, RouteCaseTableName);
            createIndexQuery.IndexName($"[PK_{TABLE_Schema}.{RouteCaseTableName}]");
            createIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
            createIndexQuery.AddColumn(COL_Synced);

            createTableQueryContext.ExecuteNonQuery();
        }

        #endregion

        #endregion

        #region Second Step

        #region Public Methods
        public List<RouteCase> GetAllRouteCases()
        {
            TryRegisterTable();

            List<RouteCase> routeCases = new List<RouteCase>();

            var queryContext = new RDBQueryContext(GetDataProvider());

            CreateGetRouteCasesQuery(queryContext);

            routeCases = queryContext.GetItems(RouteCaseMapper);

            return routeCases.Count > 0 ? routeCases : null;
        }

        public List<RouteCase> GetRouteCasesAfterRCNumber(long routeCaseId)
        {
            TryRegisterTable();

            var queryContext = new RDBQueryContext(GetDataProvider());
            List<RouteCase> routeCases = new List<RouteCase>();

            CreateGetRouteCasesQuery(queryContext).GreaterThanCondition(COL_RouteCaseId).Value(routeCaseId);

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    RouteCase routeCase = RouteCaseMapper(reader);
                    routeCases.Add(routeCase);
                }
            });
            return routeCases.Count > 0 ? routeCases : null;
        }

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();

            streamForBulkInsert.IntoTable(TABLE_NAME, '=', COL_RouteCaseId, COL_RAN, COL_RouteId, COL_RSSC, COL_Synced);

            return streamForBulkInsert;
        }

        public void WriteRecordToStream(RouteCase record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            if (record.RouteCaseId == default(long))
                throw new ArgumentNullException("record.RouteCaseId");

            if (record.RouteId == default(long))
                throw new ArgumentNullException("record.RouteId");

            record.RAN.ThrowIfNull("record.RAN for RouteId", record.RouteId);

            recordContext.Value(record.RouteCaseId);
            recordContext.Value(record.RAN);
            recordContext.Value(record.RouteId);
            recordContext.Value(record.RSSC);
            recordContext.Value(false);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }
        public void ApplyRouteCaseForDB(object preparedRouteCase)
        {
            preparedRouteCase.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream").Apply();
        }

        #endregion

        #endregion

        #endregion

        #region Third Step 

        public List<RouteCase> GetNotSyncedRouteCases()
        {
            TryRegisterTable();

            var queryContext = new RDBQueryContext(GetDataProvider());

            CreateGetRouteCasesQuery(queryContext).EqualsCondition(COL_Synced).Value(false);

            return queryContext.GetItems(RouteCaseMapper);
        }

        public void UpdateSyncedRouteCases(IEnumerable<long> routeCaseIds)
        {
            TryRegisterTable();
            if (routeCaseIds == null || !routeCaseIds.Any())
                return;

            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Synced).Value(true);

            updateQuery.Where().ListCondition(COL_RouteCaseId, RDBListConditionOperator.IN, routeCaseIds);

            queryContext.ExecuteNonQuery();
        }

        #endregion

        #region Common Private Methods

        private void TryRegisterTable()
        {
            RDBSchemaManager.Current.TryRegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = TABLE_Schema,
                DBTableName = RouteCaseTableName,
                Columns = RDBTableColumnDefinitionDictionary,
                IdColumnName = COL_RouteCaseId
            });

        }
        private RDBConditionContext CreateGetRouteCasesQuery(RDBQueryContext queryContext)
        {
            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_RouteCaseId, COL_RAN, COL_RouteId, COL_RSSC);

            return selectQuery.Where();
        }
        
        #endregion

        #region Mappers
        private RouteCase RouteCaseMapper(IRDBDataReader reader)
        {
            return new RouteCase()
            {
                RouteCaseId = reader.GetLong(COL_RouteCaseId),
                RAN = reader.GetString(COL_RAN),
                RouteId = reader.GetLong(COL_RouteId),
                RSSC = reader.GetInt(COL_RSSC)
            };
        }

        #endregion
    }
}
