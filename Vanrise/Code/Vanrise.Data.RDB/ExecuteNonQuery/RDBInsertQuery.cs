using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBInsertQuery : BaseRDBNoDataQuery, IInsertQueryReady, IInsertQueryColumnsAssigned, IInsertQueryNotExistsConditionDefined
    {
        public RDBInsertQuery(string tableName)
            :this(new RDBTableDefinitionQuerySource(tableName))
        {
        }

        public RDBInsertQuery(IRDBTableQuerySource table)
        {
            this.Table = table;
            this.ColumnValues = new List<RDBInsertColumn>();
        }

        public RDBInsertQuery(RDBInsertQuery original)
        {
            this.Table = original.Table;
            this.ColumnValues = original.ColumnValues;
            this.NotExistCondition = original.NotExistCondition;
        }

        public IRDBTableQuerySource Table { get; set; }

        public List<RDBInsertColumn> ColumnValues { get; private set; }

        public BaseRDBCondition NotExistCondition { get; set; }

        public RDBConditionContext<IInsertQueryNotExistsConditionDefined> IfNotExists()
        {
            return new RDBConditionContext<IInsertQueryNotExistsConditionDefined>(this, (condition) => this.NotExistCondition = condition, this.Table);
        }

        public IInsertQueryColumnsAssigned ColumnValue(string columnName, BaseRDBExpression value)
        {
            this.ColumnValues.Add(new RDBInsertColumn
            {
                ColumnName = columnName,
                Value = value
            });
            return this;
        }

        public IInsertQueryColumnsAssigned ColumnValue(string columnName, string value)
        {
            return this.ColumnValue(columnName, new RDBFixedTextExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned ColumnValue(string columnName, int value)
        {
            return this.ColumnValue(columnName, new RDBFixedIntExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned ColumnValue(string columnName, long value)
        {
            return this.ColumnValue(columnName, new RDBFixedLongExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned ColumnValue(string columnName, decimal value)
        {
            return this.ColumnValue(columnName, new RDBFixedDecimalExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned ColumnValue(string columnName, float value)
        {
            return this.ColumnValue(columnName, new RDBFixedFloatExpression { Value = value });
        }

        public IInsertQueryColumnsAssigned ColumnValue(string columnName, DateTime value)
        {
            return this.ColumnValue(columnName, new RDBFixedDateTimeExpression { Value = value });
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
                var clonedInsertQuery = new RDBInsertQuery(this);
                clonedInsertQuery.NotExistCondition = null;
                RDBIfQuery ifQuery = new RDBIfQuery
                {
                    Condition = rdbNotExistsCondition,
                    TrueQuery = clonedInsertQuery
                };
                return ifQuery.GetResolvedQuery(context);
            }
            else
            {
                var resolveInsertQueryContext = new RDBDataProviderResolveInsertQueryContext(this, context, true);
                return this.DataProvider.ResolveInsertQuery(resolveInsertQueryContext);
            }
        }

        #region Private Classes

       


        #endregion
    }

    public class RDBInsertColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }

    public interface IInsertQueryReady
    {
        RDBResolvedNoDataQuery GetResolvedQuery(IRDBNoDataQueryGetResolvedQueryContext context);
    }

    public interface IInsertQueryColumnsAssigned : IInsertQueryReady, IInsertQueryCanAssignColumns
    {
        
    }

    public interface IInsertQueryNotExistsConditionDefined : IInsertQueryReady, IInsertQueryCanAssignColumns
    {

    }

    public interface IInsertQueryCanAssignColumns
    {
        IInsertQueryColumnsAssigned ColumnValue(string columnName, BaseRDBExpression value);

        IInsertQueryColumnsAssigned ColumnValue(string columnName, string value);

        IInsertQueryColumnsAssigned ColumnValue(string columnName, int value);

        IInsertQueryColumnsAssigned ColumnValue(string columnName, long value);

        IInsertQueryColumnsAssigned ColumnValue(string columnName, decimal value);

        IInsertQueryColumnsAssigned ColumnValue(string columnName, float value);

        IInsertQueryColumnsAssigned ColumnValue(string columnName, DateTime value);
    }

    public interface IInsertQueryCanDefineNotExistsCondition
    {
        RDBConditionContext<IInsertQueryNotExistsConditionDefined> IfNotExists();
    }
}
