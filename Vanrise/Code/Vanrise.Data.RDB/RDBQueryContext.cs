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

        internal BaseRDBQueryContext(BaseRDBQueryContext parentContext) 
            : 
            this(parentContext.DataProvider)
        {

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
            : base(dataProvider)
        {
        }

        internal RDBQueryContext(BaseRDBQueryContext parentContext)
            : base(parentContext)
        {
        }

        internal RDBQueryContext(T parent, BaseRDBDataProvider dataProvider)
            :base(dataProvider)
        {
            _parent = parent;
        }

        internal RDBQueryContext(T parent, BaseRDBQueryContext parentContext)
            : base(parentContext)
        {
            _parent = parent;
        }

        public IRDBSelectQuery<T> Select()
            {
                var query = new RDBSelectQuery<T>(_parent, this);
                this._query = query;
                return query;
            }
        

        public IInsertQuery<T> Insert()
            {
                var query = new RDBInsertQuery<T>(_parent, this);
                this._query = query;
                return query;
            }
        

        public IUpdateQuery<T> Update()
            {
                var query = new RDBUpdateQuery<T>(_parent, this);
                this._query = query;
                return query;
            }
        

        public IRDBIfQuery<T> If()
            {
                var query = new RDBIfQuery<T>(_parent, this);
                this._query = query;
                return query;

            }
        

        public T CreateTempTable(RDBTempTableQuery tempTable)
        {
            this._query = tempTable;
            return _parent;
        }

        RDBQueryGetResolvedQueryContext _resolveQueryContext;
        RDBResolvedQuery IRDBQueryContextReady.GetResolvedQuery()
        {
            var parameterValues = new Dictionary<string, object>();
            _resolveQueryContext = new RDBQueryGetResolvedQueryContext(this, this.DataProvider, parameterValues);
            return ((IRDBQueryReady)this._query).GetResolvedQuery(_resolveQueryContext);
        }

        int IRDBQueryContextReady.ExecuteNonQuery(out Dictionary<string, Object> outputParameters)
        {
            var resolvedQuery = ((IRDBQueryContextReady)this).GetResolvedQuery();
            var context = new RDBDataProviderExecuteNonQueryContext(resolvedQuery, _resolveQueryContext.ParameterValues, _resolveQueryContext.OutputParameters);
            var rslt = this.DataProvider.ExecuteNonQuery(context);
            outputParameters = ResolveOutputParameters(context);
            return rslt;
        }

        int IRDBQueryContextReady.ExecuteNonQuery()
        {
            Dictionary<string, Object> outputParameters;
            return ((IRDBQueryContextReady)this).ExecuteNonQuery(out outputParameters);
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

        List<T> IRDBQueryContextReady.GetItems<T>(Func<IRDBDataReader, T> objectBuilder)
        {
            List<T> items = new List<T>();
            ((IRDBQueryContextReady)this).ExecuteReader((reader) =>
            {
                while (reader.Read())
                {
                    items.Add(objectBuilder(reader));
                }
            });
            return items;
        }

        object IRDBQueryContextReady.ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        void IRDBQueryContextReady.ExecuteReader(Action<IRDBDataReader> onReaderReady)
        {
            var resolvedQuery = ((IRDBQueryContextReady)this).GetResolvedQuery();
            var context = new RDBDataProviderExecuteReaderContext(resolvedQuery, _resolveQueryContext.ParameterValues, _resolveQueryContext.OutputParameters, onReaderReady);
            this.DataProvider.ExecuteReader(context);
        }

        #region Private Classes

        private class RDBDataProviderExecuteNonQueryContext : BaseRDBDataProviderExecuteQueryContext, IRDBDataProviderExecuteNonQueryContext
        {
            public RDBDataProviderExecuteNonQueryContext(RDBResolvedQuery resolvedQuery, Dictionary<string, object> parameterValues, Dictionary<string, RDBDataType> outputParameters)
                : base(resolvedQuery, parameterValues, outputParameters)
            {
            }
        }

        private class RDBDataProviderExecuteReaderContext : BaseRDBDataProviderExecuteQueryContext, IRDBDataProviderExecuteReaderContext
        {
            Action<IRDBDataReader> _onReaderReady;
            public RDBDataProviderExecuteReaderContext(RDBResolvedQuery resolvedQuery, Dictionary<string, object> parameterValues, Dictionary<string, RDBDataType> outputParameters, Action<IRDBDataReader> onReaderReady)
                : base(resolvedQuery, parameterValues, outputParameters)
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

        public IRDBBatchQuery<IRDBQueryContextReady> StartBatchQuery()
        {
            var batchQuery = new RDBBatchQuery<IRDBQueryContextReady>(base.Parent, this);
            base.Query = batchQuery;
            return batchQuery;
        }
    }

    public interface IRDBQueryContextReady
    {
        RDBResolvedQuery GetResolvedQuery();

        int ExecuteNonQuery(out Dictionary<string, Object> outputParameters);

        int ExecuteNonQuery();

        List<T> GetItems<T>(Func<IRDBDataReader, T> objectBuilder);

        object ExecuteScalar();

        void ExecuteReader(Action<IRDBDataReader> onReaderReady);
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
