using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBInsertQuery : RDBNoDataQuery, IInsertQueryReady, IInsertQueryColumnsAssigned
    {
        public RDBTableDefinition Table { get; set; }

        public List<RDBInsertColumn> ColumnValues { get; set; }

        public RDBInsertQuery(RDBTableDefinition table)
        {
            this.Table = table;
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

        public override RDBResolvedNoDataQuery GetResolvedQuery()
        {
            var context = new RDBDataProviderResolveInsertQueryContext(this.DataProvider, this);
            return this.DataProvider.ResolveInsertQuery(context);
        }

        #region Private Classes

        private class RDBDataProviderResolveInsertQueryContext : RDBDataProviderResolveQueryContext, IRDBDataProviderResolveInsertQueryContext
        {
            public RDBDataProviderResolveInsertQueryContext(BaseRDBDataProvider dataProvider, RDBInsertQuery insertQuery)
                : base(dataProvider)
            {
                this.InsertQuery = insertQuery;
            }
            public RDBInsertQuery InsertQuery
            {
                get;
                private set;
            }
        }


        #endregion
    }

    public class RDBInsertColumn
    {
        public string ColumnName { get; set; }

        public BaseRDBExpression Value { get; set; }
    }

    public interface IInsertQueryReady
    {

    }

    public interface IInsertQueryColumnsAssigned : IInsertQueryReady
    {
    }
}
