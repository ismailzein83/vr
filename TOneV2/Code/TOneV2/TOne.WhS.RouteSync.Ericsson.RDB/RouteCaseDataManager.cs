using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Ericsson.RDB
{
    public class RouteCaseDataManager : Data.IRouteCaseDataManager
    {
        string TABLE_NAME;
        string TABLE_Schema;
        string RouteCaseTableName = "RouteCase";

        string TABLE_ALIAS = "rc";

        const string COL_RCNumber = "RCNumber";
        const string COL_Options = "Options";
        const string COL_Synced = "Synced";

        public string SwitchId { get; set; }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        public void Initialize(IRouteCaseInitializeContext context)
        {
            var blockedRC = context.BranchRouteSettings.GetBlockedBRWithRouteCaseOptions(new GetBlockedBRWithRouteCaseOptionsContext());
            blockedRC.ThrowIfNull("blocked route case is null.");

            var serializedBlockedRCOption = Helper.SerializeBRWithRouteCaseOptionsList(blockedRC);

            TryRegisterTable();

            var checkIfTableExistsQueryContext = new RDBQueryContext(GetDataProvider());
            var CheckIfTableExistsQuery = checkIfTableExistsQueryContext.AddCheckIfTableExistsQuery();
            CheckIfTableExistsQuery.TableName(($"{TABLE_Schema}.[{RouteCaseTableName}]"));

            var queryContext = new RDBQueryContext(GetDataProvider());

            if (checkIfTableExistsQueryContext.ExecuteScalar().IntValue <= 0)
            {
                var createTableQuery = queryContext.AddCreateTableQuery();

                createTableQuery.DBTableName(TABLE_Schema, RouteCaseTableName);

                var columns = GetRDBTableColumnDefinitionDictionary();

                createTableQuery.AddColumn(COL_RCNumber, COL_RCNumber, RDBDataType.Int, null, null, true, true, false);
                createTableQuery.AddColumn(COL_Options, RDBDataType.Varchar, true);
                createTableQuery.AddColumn(COL_Synced, RDBDataType.Boolean, true);


                var createIndexQuery = queryContext.AddCreateIndexQuery();
                createIndexQuery.DBTableName(TABLE_Schema, RouteCaseTableName);
                //createIndexQuery.IndexName("Synced");
                createIndexQuery.IndexType(RDBCreateIndexType.NonClustered);
                createIndexQuery.AddColumn(COL_Synced);
            }

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.IfNotExists(RouteCaseTableName).EqualsCondition(COL_RCNumber).Value(context.FirstRCNumber);
            insertQuery.Column(COL_RCNumber).Value(context.FirstRCNumber);
            insertQuery.Column(COL_Options).Value(serializedBlockedRCOption);
            insertQuery.Column(COL_Synced).Value(false);

            queryContext.ExecuteNonQuery();
        }

        public List<RouteCase> GetAllRouteCases()
        {
            TryRegisterTable();

            List<RouteCase> routeCases = new List<RouteCase>();

            var queryContext = new RDBQueryContext(GetDataProvider());

            CreateGetRouteCasesQuery(queryContext);

            routeCases = queryContext.GetItems(RouteCaseMapper);

            return routeCases.Count > 0 ? routeCases : null;
        }

        public List<RouteCase> GetNotSyncedRouteCases()
        {
            TryRegisterTable();

            var queryContext = new RDBQueryContext(GetDataProvider());

            CreateGetRouteCasesQuery(queryContext).EqualsCondition(COL_Synced).Value(false);

            return queryContext.GetItems(RouteCaseMapper);
        }

        public Dictionary<string, RouteCase> GetRouteCasesAfterRCNumber(int routeCaseNumber)
        {
            TryRegisterTable();

            var queryContext = new RDBQueryContext(GetDataProvider());
            Dictionary<string, RouteCase> routeCases = new Dictionary<string, RouteCase>();

            CreateGetRouteCasesQuery(queryContext).GreaterThanCondition(COL_RCNumber).Value(routeCaseNumber);

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    RouteCase routeCase = RouteCaseMapper(reader);
                    routeCases.Add(routeCase.RouteCaseAsString, routeCase);
                }
            });
            return routeCases;
        }

        public void UpdateSyncedRouteCases(IEnumerable<int> rcNumbers)
        {
            TryRegisterTable();
            if (rcNumbers == null || !rcNumbers.Any())
                return;

            var queryContext = new RDBQueryContext(GetDataProvider());


            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Synced).Value(true);

            updateQuery.Where().ListCondition(COL_RCNumber, RDBListConditionOperator.IN, rcNumbers);

            queryContext.ExecuteNonQuery();

        }

        #region IBulkApplyDataManager

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();

            streamForBulkInsert.IntoTable(TABLE_NAME, '=', COL_RCNumber, COL_Options, COL_Synced);

            return streamForBulkInsert;
        }

        public void WriteRecordToStream(RouteCase record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();

            recordContext.Value(record.RCNumber);

            if (record.RouteCaseAsString != null)
                recordContext.Value(record.RCNumber);
            else
                recordContext.Value(string.Empty);

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

        #region Private Methods

        private RDBConditionContext CreateGetRouteCasesQuery(RDBQueryContext queryContext)
        {
            List<RouteCase> routeCases = new List<RouteCase>();

            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_RCNumber, COL_Options, COL_Synced);

            return selectQuery.Where();
        }

        private static Dictionary<string, RDBTableColumnDefinition> GetRDBTableColumnDefinitionDictionary()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_RCNumber, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Options, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_Synced, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            return columns;
        }

        private void TryRegisterTable()
        {
            TABLE_Schema = ($"WhS_RouteSync_Ericsson_{SwitchId}");
            TABLE_NAME = ($"{TABLE_Schema}.[{RouteCaseTableName}]");


            RDBSchemaManager.Current.TryRegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = TABLE_Schema,
                DBTableName = RouteCaseTableName,
                Columns = GetRDBTableColumnDefinitionDictionary(),
                IdColumnName = COL_RCNumber
            });

        }

        #endregion

        #region Mappers

        private RouteCase RouteCaseMapper(IRDBDataReader reader)
        {
            return new RouteCase()
            {
                RCNumber = reader.GetInt(COL_RCNumber),
                RouteCaseAsString = reader.GetString(COL_Options)
            };
        }

        #endregion
    }
}
