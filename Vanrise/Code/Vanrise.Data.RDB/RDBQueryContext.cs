using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public abstract class BaseRDBQueryContext
    {
        BaseRDBDataProvider _dataProvider;

        public BaseRDBDataProvider DataProvider
        {
            get
            {
                return _dataProvider;
            }
        }

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

        internal List<BaseRDBQuery> Queries
        {
            get;
            private set;
        }

        public RDBQueryContext(BaseRDBDataProvider dataProvider)
            :this(new RDBQueryBuilderContext(dataProvider))
        {
        }

        internal RDBQueryContext(RDBQueryBuilderContext queryBuilderContext)
            : base(queryBuilderContext.DataProvider)
        {
            QueryBuilderContext = queryBuilderContext;
            this.Queries = new List<BaseRDBQuery>();
        }

        public RDBSelectQuery AddSelectQuery()
        {
            var query = new RDBSelectQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }


        public RDBInsertQuery AddInsertQuery()
        {
            var query = new RDBInsertQuery(QueryBuilderContext.CreateChildContext());
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

        public RDBIfQuery AddIfQuery()
        {
            var query = new RDBIfQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;

        }

        public RDBTempTableQuery CreateTempTable()
        {
            var query = new RDBTempTableQuery(QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBParameterDeclarationQuery DeclareParameters()
        {
            var query = new RDBParameterDeclarationQuery(this.QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public RDBSetParameterValuesQuery SetParameterValues()
        {
            var query = new RDBSetParameterValuesQuery(this.QueryBuilderContext.CreateChildContext());
            Queries.Add(query);
            return query;
        }

        public IRDBBulkInsertQueryContext StartBulkInsert()
        {
            return new RDBBulkInsertQueryContext(QueryBuilderContext.CreateChildContext());
        }

        RDBQueryGetResolvedQueryContext _resolveQueryContext;
        public RDBResolvedQuery GetResolvedQuery()
        {            
            _resolveQueryContext = new RDBQueryGetResolvedQueryContext(this.DataProvider);

            StringBuilder queryBuilder = new StringBuilder();
            foreach (var subQuery in Queries)
            {
                var subQueryContext = new RDBQueryGetResolvedQueryContext(_resolveQueryContext);
                RDBResolvedQuery resolvedSubQuery = subQuery.GetResolvedQuery(subQueryContext);
                queryBuilder.AppendLine(resolvedSubQuery.QueryText);
                queryBuilder.AppendLine();
            }
            
            var resolveParametersContext = new RDBDataProviderResolveParameterDeclarationsContext(_resolveQueryContext);
            var resolvedParameterDeclarations = this.DataProvider.ResolveParameterDeclarations(resolveParametersContext);
            var resolvedQuery = new RDBResolvedQuery();
            if (resolvedParameterDeclarations != null && !string.IsNullOrEmpty(resolvedParameterDeclarations.QueryText))
                resolvedQuery.QueryText = String.Concat(resolvedParameterDeclarations.QueryText, "\n", queryBuilder.ToString());
            else
                resolvedQuery.QueryText = queryBuilder.ToString();
            return resolvedQuery;
        }

        public int ExecuteNonQuery(bool executeTransactional)
        {
            var resolvedQuery = this.GetResolvedQuery();
            var context = new RDBDataProviderExecuteNonQueryContext(resolvedQuery, executeTransactional, _resolveQueryContext.Parameters);
            return this.DataProvider.ExecuteNonQuery(context);
        }

        public int ExecuteNonQuery()
        {
            return this.ExecuteNonQuery(false);
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


        public List<Q> GetItems<Q>(Func<IRDBDataReader, Q> objectBuilder)
        {
            return GetItems(objectBuilder, false);
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

        public RDBFieldValue ExecuteScalar(bool executeTransactional)
        {
            var resolvedQuery = GetResolvedQuery();
            var context = new RDBDataProviderExecuteScalarContext(resolvedQuery, executeTransactional, _resolveQueryContext.Parameters);
            return this.DataProvider.ExecuteScalar(context);
        }

        public RDBFieldValue ExecuteScalar()
        {
            return ExecuteScalar(false);
        }

        public void ExecuteReader(Action<IRDBDataReader> onReaderReady, bool executeTransactional)
        {
            var resolvedQuery = GetResolvedQuery();
            var context = new RDBDataProviderExecuteReaderContext(resolvedQuery, executeTransactional, _resolveQueryContext.Parameters, onReaderReady);
            this.DataProvider.ExecuteReader(context);
        }

        public void ExecuteReader(Action<IRDBDataReader> onReaderReady)
        {
            ExecuteReader(onReaderReady, false);
        }

        #region Private Classes

        private class RDBDataProviderExecuteNonQueryContext : BaseRDBDataProviderExecuteQueryContext, IRDBDataProviderExecuteNonQueryContext
        {
            public RDBDataProviderExecuteNonQueryContext(RDBResolvedQuery resolvedQuery, bool executeTransactional, Dictionary<string, RDBParameter> parameters)
                : base(resolvedQuery, executeTransactional, parameters)
            {
            }
        }

        private class RDBDataProviderExecuteReaderContext : BaseRDBDataProviderExecuteQueryContext, IRDBDataProviderExecuteReaderContext
        {
            Action<IRDBDataReader> _onReaderReady;
            public RDBDataProviderExecuteReaderContext(RDBResolvedQuery resolvedQuery, bool executeTransactional, Dictionary<string, RDBParameter> parameters, Action<IRDBDataReader> onReaderReady)
                : base(resolvedQuery, executeTransactional, parameters)
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
            public RDBDataProviderExecuteScalarContext(RDBResolvedQuery resolvedQuery, bool executeTransactional, Dictionary<string, RDBParameter> parameters)
                : base(resolvedQuery, executeTransactional, parameters)
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

        public RDBQueryBuilderContext(BaseRDBDataProvider dataProvider)
        {
            this.DataProvider = dataProvider;
        }

        public void AddTableAlias(IRDBTableQuerySource table, string tableAlias)
        {
            if (_tableAliases.ContainsKey(tableAlias))
                throw new Exception(String.Format("Table Alias '{0}' already exists", tableAlias));
            _tableAliases.Add(tableAlias, table);
        }

        public IRDBTableQuerySource GetTableFromAlias(string tableAlias)
        {
            IRDBTableQuerySource table;
            if (!_tableAliases.TryGetValue(tableAlias, out table))
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

        public RDBQueryBuilderContext CreateChildContext()
        {
            RDBQueryBuilderContext childContext = new RDBQueryBuilderContext(this.DataProvider);
            return childContext;
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
