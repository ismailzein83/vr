using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBInsertQuery<T> : BaseRDBQuery, IInsertQuery<T>, IInsertQueryGenerateIdCalled<T>, IInsertQueryTableDefined<T>, IInsertQueryReady<T>, IInsertQueryColumnsAssigned<T>, IInsertQueryNotExistsConditionDefined<T>
    {
        T _parent;
        BaseRDBQueryContext _queryContext;

        public RDBInsertQuery(T parent, BaseRDBQueryContext queryContext)
        {
            _parent = parent;
            _queryContext = queryContext;
        }

        public RDBInsertQuery(RDBInsertQuery<T> original)
        {
            this._parent = original._parent;
            this._queryContext = original._queryContext;
            this.Table = original.Table;
            this.ColumnValues = original.ColumnValues;
            this.NotExistCondition = original.NotExistCondition;
            this._idOutputParameterName = original._idOutputParameterName;
        }

        public IRDBTableQuerySource Table { get; private set; }

        public List<RDBInsertColumn> ColumnValues { get; private set; }

        public BaseRDBCondition NotExistCondition { get; private set; }

        string _idOutputParameterName;

        public IInsertQueryTableDefined<T> IntoTable(IRDBTableQuerySource table)
        {
            this.Table = table;
            this.ColumnValues = new List<RDBInsertColumn>();
            return this;
        }

        public IInsertQueryTableDefined<T> IntoTable(string tableName)
        {
            return IntoTable(new RDBTableDefinitionQuerySource(tableName));
        }

        public RDBConditionContext<IInsertQueryNotExistsConditionDefined<T>> IfNotExists()
            {
                return new RDBConditionContext<IInsertQueryNotExistsConditionDefined<T>>(this, (condition) => this.NotExistCondition = condition, this.Table);
            }

        public IInsertQueryGenerateIdCalled<T> GenerateIdAndAssignToParameter(string outputParameterName)
        {
            _idOutputParameterName = outputParameterName;            
            return this;
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, BaseRDBExpression value)
        {
            this.ColumnValues.Add(new RDBInsertColumn
            {
                ColumnName = columnName,
                Value = value
            });
            return this;
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, string value)
        {
            return this.ColumnValue(columnName, new RDBFixedTextExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, int value)
        {
            return this.ColumnValue(columnName, new RDBFixedIntExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, long value)
        {
            return this.ColumnValue(columnName, new RDBFixedLongExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, decimal value)
        {
            return this.ColumnValue(columnName, new RDBFixedDecimalExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, float value)
        {
            return this.ColumnValue(columnName, new RDBFixedFloatExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, DateTime value)
        {
            return this.ColumnValue(columnName, new RDBFixedDateTimeExpression { Value = value });
        }

        protected override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            if (this.NotExistCondition != null)
            {
                var selectQuery = new RDBSelectQuery<RDBInsertQuery<T>>(this, _queryContext);
                selectQuery.From(this.Table, 1).Where().Condition(this.NotExistCondition).SelectColumns().Column(new RDBNullExpression(), "nullColumn").EndColumns();
                var rdbNotExistsCondition = new RDBNotExistsCondition<RDBInsertQuery<T>>()
                {
                    SelectQuery = selectQuery
                };
                var clonedInsertQuery = new RDBInsertQuery<T>(this);
                clonedInsertQuery.NotExistCondition = null;
                IRDBQueryReady ifQuery = new RDBIfQuery<RDBInsertQuery<T>>(this, _queryContext)
                {
                    Condition = rdbNotExistsCondition,
                    TrueQuery = clonedInsertQuery
                };
                return ifQuery.GetResolvedQuery(context);
            }
            else
            {
                var resolveInsertQueryContext = new RDBDataProviderResolveInsertQueryContext(this.Table, this.ColumnValues, _idOutputParameterName, context, true);
                return context.DataProvider.ResolveInsertQuery(resolveInsertQueryContext);
            }
        }
        
        public T EndInsert()
        {
            return _parent;
        }

        #region Private Classes




        #endregion
    }

    public class RDBInsertColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }

    public interface IInsertQuery<T> : IInsertQueryCanDefineTable<T>
    {
    }

    public interface IInsertQueryReady<T> : IRDBQueryReady
    {
        T EndInsert();
    }

    public interface IInsertQueryTableDefined<T> : IInsertQueryCanAssignColumns<T>, IInsertQueryCanDefineNotExistsCondition<T>, IInsertQueryCanCallGenerateId<T>
    {

    }

    public interface IInsertQueryColumnsAssigned<T> : IInsertQueryReady<T>, IInsertQueryCanAssignColumns<T>
    {

    }

    public interface IInsertQueryNotExistsConditionDefined<T> : IInsertQueryReady<T>, IInsertQueryCanAssignColumns<T>, IInsertQueryCanCallGenerateId<T>
    {

    }

    public interface IInsertQueryGenerateIdCalled<T> : IInsertQueryReady<T>, IInsertQueryCanAssignColumns<T>
    {

    }

    public interface IInsertQueryCanDefineTable<T>
    {
        IInsertQueryTableDefined<T> IntoTable(IRDBTableQuerySource table);

        IInsertQueryTableDefined<T> IntoTable(string tableName);
    }

    public interface IInsertQueryCanCallGenerateId<T>
    {
        IInsertQueryGenerateIdCalled<T> GenerateIdAndAssignToParameter(string outputParameterName);
    }

    public interface IInsertQueryCanAssignColumns<T>
    {
        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, BaseRDBExpression value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, string value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, int value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, long value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, decimal value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, float value);

        IInsertQueryColumnsAssigned<T> ColumnValue(string columnName, DateTime value);
    }

    public interface IInsertQueryCanDefineNotExistsCondition<T>
    {
        RDBConditionContext<IInsertQueryNotExistsConditionDefined<T>> IfNotExists();
    }
}