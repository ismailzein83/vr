using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBUpdateQuery : BaseRDBNoDataQuery, IUpdateQueryFiltered, IUpdateQueryColumnsAssigned, IUpdateQueryNotExistsConditionDefined, IUpdateQueryJoined
    {
        public RDBUpdateQuery(string tableName)
            :this(new RDBTableDefinitionQuerySource(tableName))
        {
        }

        public RDBUpdateQuery(IRDBTableQuerySource table)
        {
            this.Table = table;
            this.ColumnValues = new List<RDBUpdateColumn>();
            this.Joins = new List<RDBJoin>();
        }

        public RDBUpdateQuery(RDBUpdateQuery original)
        {
            this.Table = original.Table;
            this.ColumnValues = original.ColumnValues;
            this.Joins = original.Joins;
            this.Condition = original.Condition;
            this.NotExistCondition = original.NotExistCondition;
        }
        
        public IRDBTableQuerySource Table { get; set; }

        public List<RDBUpdateColumn> ColumnValues { get; private set; }

        public BaseRDBCondition Condition { get; private set; }

        public BaseRDBCondition NotExistCondition { get; private set; }

        public List<RDBJoin> Joins
        {
            get;
            private set;
        }

        public RDBConditionContext<IUpdateQueryNotExistsConditionDefined> IfNotExists()
        {
            return new RDBConditionContext<IUpdateQueryNotExistsConditionDefined>(this, (condition) => this.NotExistCondition = condition, this.Table);
        }

        public IUpdateQueryColumnsAssigned ColumnValue(string columnName, BaseRDBExpression value)
        {
            this.ColumnValues.Add(new RDBUpdateColumn
            {
                ColumnName = columnName,
                Value = value
            });
            return this;
        }

        public IUpdateQueryColumnsAssigned ColumnValue(string columnName, string value)
        {
            return this.ColumnValue(columnName, new RDBFixedTextExpression { Value = value });
        }

        public IUpdateQueryColumnsAssigned ColumnValue(string columnName, int value)
        {
            return this.ColumnValue(columnName, new RDBFixedIntExpression { Value = value });
        }

        public IUpdateQueryColumnsAssigned ColumnValue(string columnName, long value)
        {
            return this.ColumnValue(columnName, new RDBFixedLongExpression { Value = value });
        }

        public IUpdateQueryColumnsAssigned ColumnValue(string columnName, decimal value)
        {
            return this.ColumnValue(columnName, new RDBFixedDecimalExpression { Value = value });
        }

        public IUpdateQueryColumnsAssigned ColumnValue(string columnName, float value)
        {
            return this.ColumnValue(columnName, new RDBFixedFloatExpression { Value = value });
        }

        public IUpdateQueryColumnsAssigned ColumnValue(string columnName, DateTime value)
        {
            return this.ColumnValue(columnName, new RDBFixedDateTimeExpression { Value = value });
        }

        public RDBJoinContext<IUpdateQueryJoined> StartJoin()
        {
            return new RDBJoinContext<IUpdateQueryJoined>(this, this.Joins);
        }

        public RDBConditionContext<IUpdateQueryFiltered> Where()
        {
            return new RDBConditionContext<IUpdateQueryFiltered>(this, (condition) => this.Condition = condition, this.Table);
        }

        public override RDBResolvedNoDataQuery GetResolvedQuery(IRDBNoDataQueryGetResolvedQueryContext context)
        {
            if (this.NotExistCondition != null)
            {
                var selectQuery = new RDBSelectQuery(this.Table, 1);
                selectQuery.Where().Condition(this.NotExistCondition).StartSelect().Column(new RDBNullExpression(), "nullColumn").EndSelect();
                var rdbNotExistsCondition = new RDBNotExistsCondition()
                {
                    SelectQuery = selectQuery
                };
                var clonedUpdateQuery = new RDBUpdateQuery(this);
                clonedUpdateQuery.NotExistCondition = null;
                RDBIfQuery ifQuery = new RDBIfQuery
                {
                    Condition = rdbNotExistsCondition,
                    TrueQuery = clonedUpdateQuery
                };
                return ifQuery.GetResolvedQuery(context);
            }
            else
            {
                var resolveUpdateQueryContext = new RDBDataProviderResolveUpdateQueryContext(this, context, true);
                return this.DataProvider.ResolveUpdateQuery(resolveUpdateQueryContext);
            }
        }

        #region Private Classes

        #endregion
    }

    public class RDBUpdateColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }

    public interface IUpdateQueryReady
    {
        RDBResolvedNoDataQuery GetResolvedQuery(IRDBNoDataQueryGetResolvedQueryContext context);
    }

    public interface IUpdateQueryColumnsAssigned : IUpdateQueryReady, IUpdateQueryCanAssignColumns
    {
        
    }

    public interface IUpdateQueryFiltered : IUpdateQueryReady, IUpdateQueryCanAssignColumns
    {
        
    }
    public interface IUpdateQueryNotExistsConditionDefined : IUpdateQueryReady, IUpdateQueryCanFilter, IUpdateQueryCanAssignColumns, IUpdateQueryCanJoin
    {

    }
    public interface IUpdateQueryJoined : IUpdateQueryReady, IUpdateQueryCanFilter, IUpdateQueryCanAssignColumns
    {
        
    }

    public interface IUpdateQueryCanAssignColumns : IUpdateQueryReady
    {
        IUpdateQueryColumnsAssigned ColumnValue(string columnName, BaseRDBExpression value);

        IUpdateQueryColumnsAssigned ColumnValue(string columnName, string value);

        IUpdateQueryColumnsAssigned ColumnValue(string columnName, int value);

        IUpdateQueryColumnsAssigned ColumnValue(string columnName, long value);

        IUpdateQueryColumnsAssigned ColumnValue(string columnName, decimal value);

        IUpdateQueryColumnsAssigned ColumnValue(string columnName, float value);

        IUpdateQueryColumnsAssigned ColumnValue(string columnName, DateTime value);
    }

    public interface IUpdateQueryCanFilter
    {
        RDBConditionContext<IUpdateQueryFiltered> Where();
    }

    public interface IUpdateQueryCanJoin
    {
    }

    public interface IUpdateQueryCanDefineNotExistsCondition
    {
        RDBConditionContext<IUpdateQueryNotExistsConditionDefined> IfNotExists();
    }
}
