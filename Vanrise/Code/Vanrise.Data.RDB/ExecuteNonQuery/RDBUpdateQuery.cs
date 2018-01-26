using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBUpdateQuery : RDBNoDataQuery, IUpdateQueryFiltered, IUpdateQueryColumnsAssigned
    {
        public RDBUpdateQuery(RDBTableDefinition table)
        {
            this.Table = table;
            this.ColumnValues = new List<RDBUpdateColumn>();
        }

        public RDBTableDefinition Table { get; private set; }

        public List<RDBUpdateColumn> ColumnValues { get; private set; }

        public BaseRDBCondition Condition { get; private set; }

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

        RDBConditionContext<IUpdateQueryFiltered> IUpdateQueryColumnsAssigned.Where()
        {
            return new RDBConditionContext<IUpdateQueryFiltered>(this, (condition) => this.Condition = condition);
        }

        public override RDBResolvedNoDataQuery GetResolvedQuery()
        {
            var context = new RDBDataProviderResolveUpdateQueryContext(this.DataProvider, this);
            return this.DataProvider.ResolveUpdateQuery(context);
        }

        #region Private Classes

        private class RDBDataProviderResolveUpdateQueryContext : RDBDataProviderResolveQueryContext, IRDBDataProviderResolveUpdateQueryContext
        {
            public RDBDataProviderResolveUpdateQueryContext(BaseRDBDataProvider dataProvider, RDBUpdateQuery updateQuery)
                : base(dataProvider)
            {
                this.UpdateQuery = updateQuery;
            }
            public RDBUpdateQuery UpdateQuery
            {
                get;
                private set;
            }
        }


        #endregion
    }

    public class RDBUpdateColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }

    public interface IUpdateQueryReady
    {

    }

    public interface IUpdateQueryColumnsAssigned : IUpdateQueryReady
    {
        RDBConditionContext<IUpdateQueryFiltered> Where();
    }

    public interface IUpdateQueryFiltered : IUpdateQueryReady
    {

    }
}
