﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBQueryContext
    {
        BaseRDBDataProvider _dataProvider;

        public BaseRDBDataProvider DataProvider { get { return _dataProvider; } }

        public BaseRDBQueryContext(BaseRDBDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }
    }

    public class RDBQueryContext : BaseRDBQueryContext
    {
        BaseRDBQuery _query;

        public RDBQueryBuilderContext QueryBuilderContext { get; private set; }

        internal protected BaseRDBQuery Query
        {
            get
            {
                return _query;
            }
            set
            {
                _query = value;
            }
        }

        internal List<BaseRDBQuery> Queries { get; private set; }

        public RDBQueryContext(BaseRDBDataProvider dataProvider)
            : this(dataProvider, false)
        {
        }

        public RDBQueryContext(BaseRDBDataProvider dataProvider, bool dontGenerateParameters)
            : this(new RDBQueryBuilderContext(dataProvider, dontGenerateParameters))
        {
        }

        internal RDBQueryContext(RDBQueryBuilderContext queryBuilderContext)
            : base(queryBuilderContext.DataProvider)
        {
            QueryBuilderContext = queryBuilderContext;
            this.Queries = new List<BaseRDBQuery>();
        }

        //internal RDBQueryContext(RDBQueryContext parentContext)
        //    : this(parentContext.QueryBuilderContext.CreateChildContext())
        //{

        //}

        public RDBSelectQuery AddSelectQuery()
        {
            var query = new RDBSelectQuery(QueryBuilderContext.CreateChildContext(), true);
            Queries.Add(query);
            return query;
        }

        public RDBSelectTableRowsCountQuery AddSelectTableRowsCountQuery()
        {
            var query = new RDBSelectTableRowsCountQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBInsertQuery AddInsertQuery()
        {
            var query = new RDBInsertQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBInsertMultipleRowsQuery AddInsertMultipleRowsQuery()
        {
            var query = new RDBInsertMultipleRowsQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBUpdateQuery AddUpdateQuery()
        {
            var query = new RDBUpdateQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBDeleteQuery AddDeleteQuery()
        {
            var query = new RDBDeleteQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        //public RDBIfQuery AddIfQuery()
        //{
        //    var query = new RDBIfQuery(QueryBuilderContext.CreateChildContext());
        //    Queries.Add(query);
        //    return query;

        //}

        public RDBTempTableQuery CreateTempTable()
        {
            var query = new RDBTempTableQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBCreateTableQuery AddCreateTableQuery()
        {
            var query = new RDBCreateTableQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBRenameTableQuery AddRenameTableQuery()
        {
            var query = new RDBRenameTableQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBAddColumnsQuery AddAddColumnsQuery()
        {
            var query = new RDBAddColumnsQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBAlterColumnsQuery AddAlterColumnsQuery()
        {
            var query = new RDBAlterColumnsQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBCreateSchemaIfNotExistsQuery AddCreateSchemaIfNotExistsQuery()
        {
            var query = new RDBCreateSchemaIfNotExistsQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBDropTableQuery AddDropTableQuery()
        {
            var query = new RDBDropTableQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBCheckIfTableExistsQuery AddCheckIfTableExistsQuery()
        {
            var query = new RDBCheckIfTableExistsQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBCreateIndexQuery AddCreateIndexQuery()
        {
            var query = new RDBCreateIndexQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBSwapTablesQuery AddSwapTablesQuery()
        {
            var query = new RDBSwapTablesQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public string GetConnectionString()
        {
            return GetOverridenConnectionString(null);
        }

        public string GetOverridenConnectionString(GetOverridenConnectionStringInput input)
        {
            string overridingDatabaseName = input != null ? input.OverridingDatabaseName : null;

            RDBDataProviderGetConnectionStringContext context = new RDBDataProviderGetConnectionStringContext(overridingDatabaseName);
            return this.DataProvider.GetConnectionString(context);
        }

        public string GetDataBaseName(GetDataBaseNameInput input)
        {
            RDBDataProviderGetDataBaseNameContext context = new RDBDataProviderGetDataBaseNameContext();
            return this.DataProvider.GetDataBaseName(context);
        }

        public void CreateDatabase(CreateDatabaseInput input)
        {
            RDBDataProviderCreateDatabaseContext context = new RDBDataProviderCreateDatabaseContext(input.DatabaseName, input.DataFileDirectory, input.LogFileDirectory);
            this.DataProvider.CreateDatabase(context);
        }

        public void DropDatabase(DropDatabaseInput input)
        {
            RDBDataProviderDropDatabaseContext context = new RDBDataProviderDropDatabaseContext(input.DatabaseName);
            this.DataProvider.DropDatabase(context);
        }

        public bool IsDataUpdated(string tableName, ref object lastReceivedDataInfo)
        {
            return IsDataUpdated(RDBSchemaManager.Current, tableName, ref lastReceivedDataInfo);
        }

        public bool IsDataUpdated(RDBSchemaManager schemaManager, string tableName, ref object lastReceivedDataInfo)
        {
            var tableDefinition = schemaManager.GetTableDefinitionWithValidate(this.DataProvider, tableName);
            tableDefinition.ModifiedTimeColumnName.ThrowIfNull("tableDefinition.ModifiedTimeColumnName", tableName);
            return this.DataProvider.IsDataUpdated(schemaManager, tableName, tableDefinition, ref lastReceivedDataInfo);
        }

        public bool IsDataUpdated<T>(string tableName, T cachePartitionColumnValue, ref object lastReceivedDataInfo)
        {
            return IsDataUpdated<T>(RDBSchemaManager.Current, tableName, cachePartitionColumnValue, ref lastReceivedDataInfo);
        }

        public bool IsDataUpdated<T>(RDBSchemaManager schemaManager, string tableName, T cachePartitionColumnValue, ref object lastReceivedDataInfo)
        {
            var tableDefinition = schemaManager.GetTableDefinitionWithValidate(this.DataProvider, tableName);
            tableDefinition.ModifiedTimeColumnName.ThrowIfNull("tableDefinition.ModifiedTimeColumnName", tableName);
            tableDefinition.CachePartitionColumnName.ThrowIfNull("tableDefinition.CachePartitionColumnName", tableName);
            return this.DataProvider.IsDataUpdated(schemaManager, tableName, tableDefinition, tableDefinition.CachePartitionColumnName, cachePartitionColumnValue, ref lastReceivedDataInfo);
        }

        public int GetDBQueryMaxParameterNumber()
        {
            return 2000;
        }

        public object GetMaxReceivedDataInfo(string tableName)
        {
            var tableDefinition = RDBSchemaManager.Current.GetTableDefinitionWithValidate(this.DataProvider, tableName);
            tableDefinition.ModifiedTimeColumnName.ThrowIfNull("tableDefinition.ModifiedTimeColumnName", tableName);
            return this.DataProvider.GetMaxReceivedDataInfo(tableName, tableDefinition);
        }

        public void AddGreaterThanReceivedDataInfoCondition(string tableName, RDBConditionContext conditionContext, object lastReceivedDataInfo)
        {
            var tableDefinition = RDBSchemaManager.Current.GetTableDefinitionWithValidate(this.DataProvider, tableName);
            tableDefinition.ModifiedTimeColumnName.ThrowIfNull("tableDefinition.ModifiedTimeColumnName", tableName);
            this.DataProvider.AddGreaterThanReceivedDataInfoCondition(tableName, tableDefinition, conditionContext, lastReceivedDataInfo);
        }

        //public RDBSetParameterValuesQuery SetParameterValues()
        //{
        //    var query = new RDBSetParameterValuesQuery(this.QueryBuilderContext.CreateChildContext());
        //    Queries.Add(query);
        //    return query;
        //}

        public RDBBulkInsertQueryContext StartBulkInsert()
        {
            return new RDBBulkInsertQueryContext(QueryBuilderContext.CreateChildContext());
        }

        public int ExecuteNonQuery()
        {
            return this.ExecuteNonQuery(false);
        }

        public int ExecuteNonQuery(int commandTimeoutInSeconds)
        {
            return this.ExecuteNonQuery(false, commandTimeoutInSeconds);
        }

        public int ExecuteNonQuery(bool executeTransactional, int? commandTimeoutInSeconds = null)
        {
            var resolveQueryContext = new RDBQueryGetResolvedQueryContext(this.DataProvider);
            var resolvedQuery = this.GetResolvedQuery(resolveQueryContext);
            if (resolvedQuery.Statements.Count == 0)
                return 0;
            var context = new RDBDataProviderExecuteNonQueryContext(resolveQueryContext, resolvedQuery, executeTransactional, resolveQueryContext.Parameters, commandTimeoutInSeconds);
            return this.DataProvider.ExecuteNonQuery(context);
        }

        public RDBFieldValue ExecuteScalar()
        {
            return ExecuteScalar(false);
        }

        public RDBFieldValue ExecuteScalar(int commandTimeoutInSeconds)
        {
            return ExecuteScalar(false, commandTimeoutInSeconds);
        }

        public RDBFieldValue ExecuteScalar(bool executeTransactional, int? commandTimeoutInSeconds = null)
        {
            var resolveQueryContext = new RDBQueryGetResolvedQueryContext(this.DataProvider);
            var resolvedQuery = GetResolvedQuery(resolveQueryContext);
            if (resolvedQuery.Statements.Count == 0)
                return null;
            var context = new RDBDataProviderExecuteScalarContext(resolveQueryContext, resolvedQuery, executeTransactional, resolveQueryContext.Parameters, commandTimeoutInSeconds);
            return this.DataProvider.ExecuteScalar(context);
        }

        public List<Q> GetItems<Q>(Func<IRDBDataReader, Q> objectBuilder)
        {
            return GetItems(objectBuilder, false);
        }

        public List<Q> GetItems<Q>(Func<IRDBDataReader, Q> objectBuilder, bool executeTransactional)
        {
            List<Q> items = new List<Q>();
            ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    items.Add(objectBuilder(reader));
                }
            }, executeTransactional);
            return items;
        }

        public Q GetItem<Q>(Func<IRDBDataReader, Q> objectBuilder)
        {
            Q item = default(Q);
            ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    item = objectBuilder(reader);
                }
            }, false);
            return item;
        }

        public void ExecuteReader(Action<IRDBDataReader> onReaderReady)
        {
            ExecuteReader(onReaderReady, false);
        }

        public void ExecuteReader(Action<IRDBDataReader> onReaderReady, int commandTimeoutInSeconds)
        {
            ExecuteReader(onReaderReady, false, commandTimeoutInSeconds);
        }

        public void ExecuteReader(Action<IRDBDataReader> onReaderReady, bool executeTransactional, int? commandTimeoutInSeconds = null)
        {
            var resolveQueryContext = new RDBQueryGetResolvedQueryContext(this.DataProvider);
            var resolvedQuery = GetResolvedQuery(resolveQueryContext);
            if (resolvedQuery.Statements.Count == 0)
                return;
            var context = new RDBDataProviderExecuteReaderContext(resolveQueryContext, resolvedQuery, executeTransactional, resolveQueryContext.Parameters, commandTimeoutInSeconds, onReaderReady);
            this.DataProvider.ExecuteReader(context);
        }

        public RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext resolveQueryContext)
        {
            return GetResolvedQuery(resolveQueryContext, false);
        }

        internal RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext resolveQueryContext, bool isSubContext)
        {
            var resolvedQuery = new RDBResolvedQuery();
            foreach (var subQuery in Queries)
            {
                var subQueryContext = new RDBQueryGetResolvedQueryContext(resolveQueryContext);
                RDBResolvedQuery resolvedSubQuery = subQuery.GetResolvedQuery(subQueryContext);
                if (resolvedSubQuery.Statements.Count > 0)
                    resolvedQuery.Statements.AddRange(resolvedSubQuery.Statements);
            }

            if (!isSubContext)
            {
                //var resolveParametersContext = new RDBDataProviderResolveParameterDeclarationsContext(resolveQueryContext);
                //var resolvedParameterDeclarations = this.DataProvider.ResolveParameterDeclarations(resolveParametersContext);
                //if (resolvedParameterDeclarations != null && resolvedParameterDeclarations.Statements.Count > 0)
                //{
                //    for (int i = resolvedParameterDeclarations.Statements.Count - 1; i >= 0; i--)
                //    {
                //        resolvedQuery.Statements.Insert(0, resolvedParameterDeclarations.Statements[i]);
                //    }
                //}

                if (resolveQueryContext.TempTableNames.Count > 0)
                {
                    foreach (var tempTableName in resolveQueryContext.TempTableNames)
                    {
                        var dropTableResolvedQuery = resolveQueryContext.DataProvider.ResolveTempTableDropQuery(new RDBDataProviderResolveTempTableDropQueryContext(tempTableName, resolveQueryContext));
                        if (dropTableResolvedQuery != null && dropTableResolvedQuery.Statements.Count > 0)
                            resolvedQuery.Statements.AddRange(dropTableResolvedQuery.Statements);
                    }
                }
            }
            return resolvedQuery;
        }

        public DateTimeRange GetDBDateTimeRange()
        {
            return new DateTimeRange()
            {
                From = new DateTime(1753, 1, 1, 0, 0, 0),
                To = new DateTime(9999, 12, 31, 23, 59, 59)
            };
        }

        public bool CheckIfDefaultOrInvalidDBTime(DateTime? dateTime)
        {
            if (!dateTime.HasValue || dateTime.Value == default(DateTime))
                return true;

            if (!IsWithinDBDateTimeRange(dateTime.Value))
                return true;

            return false;
        }

        public bool IsWithinDBDateTimeRange(DateTime dateTime)
        {
            DateTimeRange dateTimeRange = GetDBDateTimeRange();
            if (dateTime < dateTimeRange.From || dateTime > dateTimeRange.To)
                return false;

            return true;
        }

        #region Private Classes

        private class RDBDataProviderExecuteNonQueryContext : BaseRDBDataProviderExecuteQueryContext, IRDBDataProviderExecuteNonQueryContext
        {
            public RDBDataProviderExecuteNonQueryContext(IBaseRDBResolveQueryContext resolveQueryContext, RDBResolvedQuery query, bool executeTransactional,
                Dictionary<string, RDBParameter> parameters, int? commandTimeoutInSeconds)
                : base(resolveQueryContext, query, executeTransactional, parameters, commandTimeoutInSeconds)
            {
            }
        }

        private class RDBDataProviderExecuteReaderContext : BaseRDBDataProviderExecuteQueryContext, IRDBDataProviderExecuteReaderContext
        {
            Action<IRDBDataReader> _onReaderReady;

            public RDBDataProviderExecuteReaderContext(IBaseRDBResolveQueryContext resolveQueryContext, RDBResolvedQuery query, bool executeTransactional,
                Dictionary<string, RDBParameter> parameters, int? commandTimeoutInSeconds, Action<IRDBDataReader> onReaderReady)
                : base(resolveQueryContext, query, executeTransactional, parameters, commandTimeoutInSeconds)
            {
                _onReaderReady = onReaderReady;
            }

            public void OnReaderReady(IRDBDataReader reader)
            {
                _onReaderReady(reader);
            }
        }

        private class RDBDataProviderExecuteScalarContext : BaseRDBDataProviderExecuteQueryContext, IRDBDataProviderExecuteScalarContext
        {
            public RDBDataProviderExecuteScalarContext(IBaseRDBResolveQueryContext resolveQueryContext, RDBResolvedQuery query, bool executeTransactional,
                Dictionary<string, RDBParameter> parameters, int? commandTimeoutInSeconds)
                : base(resolveQueryContext, query, executeTransactional, parameters, commandTimeoutInSeconds)
            {
            }
        }

        #endregion
    }

    public class RDBQueryBuilderContext
    {
        public BaseRDBDataProvider DataProvider { get; private set; }

        Dictionary<string, IRDBTableQuerySource> _tableAliases = new Dictionary<string, IRDBTableQuerySource>();
        IRDBTableQuerySource _mainQueryTable;
        bool _dontGenerateParameters;

        //this is used for tables not directly in the query like subqueries added for NotExists condition for InsertQuery and UpdateQuery
        Dictionary<string, IRDBTableQuerySource> _indirectTableAliases = new Dictionary<string, IRDBTableQuerySource>();

        public RDBQueryBuilderContext(BaseRDBDataProvider dataProvider, bool dontGenerateParameters)
        {
            this.DataProvider = dataProvider;
            this._dontGenerateParameters = dontGenerateParameters;
        }

        public void AddTableAlias(IRDBTableQuerySource table, string tableAlias)
        {
            if (_tableAliases.ContainsKey(tableAlias))
                throw new Exception(String.Format("Table Alias '{0}' already exists", tableAlias));
            _tableAliases.Add(tableAlias, table);
        }

        public void AddIndirectTableAlias(IRDBTableQuerySource table, string tableAlias)
        {
            if (_indirectTableAliases.ContainsKey(tableAlias))
                throw new Exception(String.Format("Table Alias '{0}' already exists", tableAlias));
            _indirectTableAliases.Add(tableAlias, table);
        }

        public IRDBTableQuerySource GetTableFromAlias(string tableAlias)
        {
            IRDBTableQuerySource table;
            if (!_tableAliases.TryGetValue(tableAlias, out table) && !_indirectTableAliases.TryGetValue(tableAlias, out table))
                throw new Exception(String.Format("Table Alias '{0}' not exists", tableAlias));
            return table;
        }

        public IRDBTableQuerySource GetMainQueryTable()
        {
            return _mainQueryTable;
        }

        public void SetMainQueryTable(IRDBTableQuerySource mainQueryTable)
        {
            if (_mainQueryTable != null)
                throw new Exception("MainQueryTable");
            _mainQueryTable = mainQueryTable;
        }

        Func<RDBJoinContext> _getQueryJoinContext;

        public void SetGetQueryJoinContext(Func<RDBJoinContext> getQueryJoinContext)
        {
            if (_getQueryJoinContext != null)
                throw new Exception("_getQueryJoinContext already set");
            _getQueryJoinContext = getQueryJoinContext;
        }

        public RDBJoinContext GetQueryJoinContext()
        {
            _getQueryJoinContext.ThrowIfNull("_getQueryJoinContext");
            return _getQueryJoinContext();
        }

        public RDBQueryBuilderContext CreateChildContext()
        {
            RDBQueryBuilderContext childContext = new RDBQueryBuilderContext(this.DataProvider, _dontGenerateParameters);
            return childContext;
        }

        public bool DontGenerateParameters
        {
            get
            {
                return this._dontGenerateParameters;
            }
        }
    }

    public class TestQ
    {
        void Test()
        {
            //var dataProvider = new RDB.DataProvider.Providers.MSSQLRDBDataProvider();
            //new RDBQueryContext(dataProvider).Select.FromTable("SEC_USER").SelectColumns.Columns("ID", "Name").EndColumns().Sort.ByColumn("ID", RDBSortDirection.ASC).EndSort().EndSelect();
            //new RDBQueryContext(dataProvider)
            //    .StartBatchQuery()
            //        .AddQuery.Select.From("SEC_USER").SelectColumns.Columns("ID", "Name").EndColumns().Sort.ByColumn("ID", RDBSortDirection.ASC).EndSort().EndSelect()
            //        .AddQuery.Select.From("SEC_GROUP").SelectColumns.Columns("Name").EndColumns().EndSelect()
            //        .AddQuery.Insert.IntoTable("SEC_User").IfNotExists.EqualsCondition("Email", "tess@ffds.com").ColumnValue("Email", "tess@ffds.com").ColumnValue("Name", "TEst").EndInsert()
            //        .AddQuery.If.IfCondition.CompareCondition(new RDBFixedIntExpression(), RDBCompareConditionOperator.Eq, new RDBFixedIntExpression()).ThenQuery.Select.From("RRR").SelectColumns.Column("fffff").EndColumns().Sort.ByColumn("Name", RDBSortDirection.ASC).EndSort().EndSelect().EndIf()
            //    .EndBatchQuery().GetItems<int>(null);
        }
    }
}
