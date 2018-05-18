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

    public class RDBQueryContext<T> : BaseRDBQueryContext, IRDBQueryContextReady
    {
        T _parent;
        protected T Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }
        
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

        internal RDBQueryContext(BaseRDBDataProvider dataProvider)
            : this(default(T), dataProvider)
        {
        }

        internal RDBQueryContext(RDBQueryBuilderContext queryBuilderContext)
            : this(default(T), queryBuilderContext)
        {
        }

        internal RDBQueryContext(T parent, BaseRDBDataProvider dataProvider)
            :this(parent, new RDBQueryBuilderContext(dataProvider))
        {
        }

        internal RDBQueryContext(T parent, RDBQueryBuilderContext queryBuilderContext)
            : base(queryBuilderContext.DataProvider)
        {
            _parent = parent;
            QueryBuilderContext = queryBuilderContext;
        }

        public IRDBSelectQuery<T> Select()
            {
                var query = new RDBSelectQuery<T>(_parent, QueryBuilderContext);
                this._query = query;
                return query;
            }
        

        public IInsertQuery<T> Insert()
            {
                var query = new RDBInsertQuery<T>(_parent, QueryBuilderContext);
                this._query = query;
                return query;
            }
        

        public IUpdateQuery<T> Update()
            {
                var query = new RDBUpdateQuery<T>(_parent, QueryBuilderContext);
                this._query = query;
                return query;
            }
        

        public IRDBIfQuery<T> If()
            {
                var query = new RDBIfQuery<T>(_parent, QueryBuilderContext);
                this._query = query;
                return query;

            }


        public IRDBBatchQuery<T> StartBatchQuery()
        {
            var batchQuery = new RDBBatchQuery<T>(_parent, this.QueryBuilderContext);
            this._query= batchQuery;
            return batchQuery;
        }

        public T CreateTempTable(RDBTempTableQuery tempTable)
        {
            this._query = tempTable;
            return _parent;
        }

        public IRDBParameterDeclarationQuery<T> DeclareParameters()
        {
            var query = new RDBParameterDeclarationQuery<T>(_parent, this.QueryBuilderContext);
            this._query = query;
            return query;
        }

        public ISetParameterValuesQuery<T> SetParameterValues()
        {
            var query = new SetParameterValuesQuery<T>(_parent, this.QueryBuilderContext);
            this._query = query;
            return query;
        }

        RDBQueryGetResolvedQueryContext _resolveQueryContext;
        RDBResolvedQuery IRDBQueryContextReady.GetResolvedQuery()
        {
            
            _resolveQueryContext = new RDBQueryGetResolvedQueryContext(this.DataProvider);
            var resolvedQuery = ((IRDBQueryReady)this._query).GetResolvedQuery(_resolveQueryContext);
            var resolveParametersContext = new RDBDataProviderResolveParameterDeclarationsContext(_resolveQueryContext);
            var resolvedParameterDeclarations = this.DataProvider.ResolveParameterDeclarations(resolveParametersContext);
            if (resolvedParameterDeclarations != null && !string.IsNullOrEmpty(resolvedParameterDeclarations.QueryText))
                resolvedQuery.QueryText = String.Concat(resolvedParameterDeclarations.QueryText, "\n", resolvedQuery.QueryText);
            return resolvedQuery;
        }

        int IRDBQueryContextReady.ExecuteNonQuery(bool executeTransactional, out Dictionary<string, Object> outputParameters)
        {
            var resolvedQuery = ((IRDBQueryContextReady)this).GetResolvedQuery();
            var context = new RDBDataProviderExecuteNonQueryContext(resolvedQuery, executeTransactional, _resolveQueryContext.ParameterValues, _resolveQueryContext.OutputParameters);
            var rslt = this.DataProvider.ExecuteNonQuery(context);
            outputParameters = ResolveOutputParameters(context);
            return rslt;
        }

        int IRDBQueryContextReady.ExecuteNonQuery(out Dictionary<string, Object> outputParameters)
        {
            return ((IRDBQueryContextReady)this).ExecuteNonQuery(false, out outputParameters);
        }

        int IRDBQueryContextReady.ExecuteNonQuery(bool executeTransactional)
        {
            Dictionary<string, Object> outputParameters;
            return ((IRDBQueryContextReady)this).ExecuteNonQuery(executeTransactional, out outputParameters);
        }

        int IRDBQueryContextReady.ExecuteNonQuery()
        {
            return ((IRDBQueryContextReady)this).ExecuteNonQuery(false);
        }

        private Dictionary<string, object> ResolveOutputParameters(RDBDataProviderExecuteNonQueryContext context)
        {
            Dictionary<string, Object> outputParameters;
            if (context.OutputParameterValues != null && context.OutputParameterValues.Count > 0)
            {
                outputParameters = new Dictionary<string, object>();
                foreach(var prm in context.OutputParameterValues)
                {
                    string originalPrmName = this.DataProvider.ParameterNamePrefix != null ? prm.Key.Substring(this.DataProvider.ParameterNamePrefix.Length) : prm.Key;
                    outputParameters.Add(originalPrmName, prm.Value);
                }
            }
            else
            {
                outputParameters = null;
            }
            return outputParameters;
        }

        List<Q> IRDBQueryContextReady.GetItems<Q>(Func<IRDBDataReader, Q> objectBuilder, bool executeTransactional)
        {
            List<Q> items = new List<Q>();
            ((IRDBQueryContextReady)this).ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    items.Add(objectBuilder(reader));
                }
            }, executeTransactional);
            return items;
        }


        List<Q> IRDBQueryContextReady.GetItems<Q>(Func<IRDBDataReader, Q> objectBuilder)
        {
            return ((IRDBQueryContextReady)this).GetItems(objectBuilder, false);
        }

        Q IRDBQueryContextReady.GetItem<Q>(Func<IRDBDataReader, Q> objectBuilder)
        {
            Q item = default(Q);
            ((IRDBQueryContextReady)this).ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    item = objectBuilder(reader);
                }
            }, false);
            return item;
        }

        object IRDBQueryContextReady.ExecuteScalar(bool executeTransactional)
        {
            throw new NotImplementedException();
        }

        object IRDBQueryContextReady.ExecuteScalar()
        {
            return ((IRDBQueryContextReady)this).ExecuteScalar(false);
        }

        void IRDBQueryContextReady.ExecuteReader(Action<IRDBDataReader> onReaderReady, bool executeTransactional)
        {
            var resolvedQuery = ((IRDBQueryContextReady)this).GetResolvedQuery();
            var context = new RDBDataProviderExecuteReaderContext(resolvedQuery, executeTransactional, _resolveQueryContext.ParameterValues, _resolveQueryContext.OutputParameters, onReaderReady);
            this.DataProvider.ExecuteReader(context);
        }

        void IRDBQueryContextReady.ExecuteReader(Action<IRDBDataReader> onReaderReady)
        {
            ((IRDBQueryContextReady)this).ExecuteReader(onReaderReady, false);
        }

        #region Private Classes

        private class RDBDataProviderExecuteNonQueryContext : BaseRDBDataProviderExecuteQueryContext, IRDBDataProviderExecuteNonQueryContext
        {
            public RDBDataProviderExecuteNonQueryContext(RDBResolvedQuery resolvedQuery, bool executeTransactional, Dictionary<string, object> parameterValues, Dictionary<string, RDBDataType> outputParameters)
                : base(resolvedQuery, executeTransactional, parameterValues, outputParameters)
            {
            }
        }

        private class RDBDataProviderExecuteReaderContext : BaseRDBDataProviderExecuteQueryContext, IRDBDataProviderExecuteReaderContext
        {
            Action<IRDBDataReader> _onReaderReady;
            public RDBDataProviderExecuteReaderContext(RDBResolvedQuery resolvedQuery, bool executeTransactional, Dictionary<string, object> parameterValues, Dictionary<string, RDBDataType> outputParameters, Action<IRDBDataReader> onReaderReady)
                : base(resolvedQuery, executeTransactional, parameterValues, outputParameters)
            {
                _onReaderReady = onReaderReady;
            }

            public void OnReaderReady(IRDBDataReader reader)
            {
                _onReaderReady(reader);
            }
        }

        #endregion
    }

    public class RDBQueryContext : RDBQueryContext<IRDBQueryContextReady>
    {
        public RDBQueryContext(BaseRDBDataProvider dataProvider)
            : base(dataProvider)
        {
            base.Parent = this;
        }

        public IRDBBulkInsertQueryContext StartBulkInsert()
        {
            return new RDBBulkInsertQueryContext(this);
        }
    }

    public interface IRDBQueryContextReady
    {
        RDBResolvedQuery GetResolvedQuery();

        int ExecuteNonQuery(bool executeTransactional, out Dictionary<string, Object> outputParameters);
        
        int ExecuteNonQuery(out Dictionary<string, Object> outputParameters);

        int ExecuteNonQuery(bool executeTransactional);

        int ExecuteNonQuery();

        List<T> GetItems<T>(Func<IRDBDataReader, T> objectBuilder, bool executeTransactional);

        List<T> GetItems<T>(Func<IRDBDataReader, T> objectBuilder);

        T GetItem<T>(Func<IRDBDataReader, T> objectBuilder);

        object ExecuteScalar(bool executeTransactional);

        object ExecuteScalar();

        void ExecuteReader(Action<IRDBDataReader> onReaderReady, bool executeTransactional);

        void ExecuteReader(Action<IRDBDataReader> onReaderReady);
    }

    public class RDBQueryBuilderContext
    {
        public BaseRDBDataProvider DataProvider { get; private set; }

        Dictionary<string, IRDBTableQuerySource> _tableAliases = new Dictionary<string, IRDBTableQuerySource>();

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
