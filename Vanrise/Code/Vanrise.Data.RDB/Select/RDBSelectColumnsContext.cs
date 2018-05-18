using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBSelectColumnsContext<T> : IRDBSelectColumnsContextReady<T>
    {
        T _parent;
        RDBQueryBuilderContext _queryBuilderContext;
        List<RDBSelectColumn> _columns;
        IRDBTableQuerySource _table;
        string _tableAlias;

        public RDBSelectColumnsContext(T parent, RDBQueryBuilderContext queryBuilderContext, List<RDBSelectColumn> columns, IRDBTableQuerySource table, string tableAlias)
        {
            _parent = parent;
            _queryBuilderContext = queryBuilderContext;
            _columns = columns;
            _table = table;
            _tableAlias = tableAlias;
        }

        public IRDBSelectColumnsContextReady<T> Columns(params string[] columnNames)
        {
            foreach (var colName in columnNames)
            {
                Column(colName);
            }
            return this;
        }

        public IRDBSelectColumnsContextReady<T> AllTableColumns(string tableAlias)
        {
            var tableQuerySource = _queryBuilderContext.GetTableFromAlias(tableAlias);
            RDBTableDefinitionQuerySource table = tableQuerySource.CastWithValidate<RDBTableDefinitionQuerySource>("tableQuerySource", tableAlias);
            foreach (var colName in RDBSchemaManager.Current.GetTableColumns(table.TableName))
            {
                Column(tableAlias, colName, colName);
            }
            return this;
        }

        public IRDBSelectColumnsContextReady<T> Column(BaseRDBExpression expression, string alias)
        {
            _columns.Add(new RDBSelectColumn
            {
                Expression = expression,
                Alias = alias
            });
            return this;
        }

        public IRDBSelectColumnsContextReady<T> ColumnToParameter(BaseRDBExpression expression, string parameterName)
        {
            string dbParameterName = _queryBuilderContext.DataProvider.ConvertToDBParameterName(parameterName);
            _columns.Add(new RDBSelectColumn
            {
                Expression = expression,
                SetDBParameterName = dbParameterName
            });
            return this;
        }

        public IRDBSelectColumnsContextReady<T> Column(string tableAlias, string columnName, string alias)
        {
            return Column(new RDBColumnExpression
            {
                TableAlias = tableAlias,
                ColumnName = columnName
            },
                     alias);
        }

        public IRDBSelectColumnsContextReady<T> ColumnToParameter(string tableAlias, string columnName, string parameterName)
        {
            return ColumnToParameter(new RDBColumnExpression
            {
                TableAlias = tableAlias,
                ColumnName = columnName
            },
                     parameterName);
        }

        //public IRDBSelectColumnsContextReady<T> Column(string tableName, string columnName, string alias)
        //{
        //    return Column(new RDBTableDefinitionQuerySource(tableName), columnName, alias);
        //}

        public IRDBSelectColumnsContextReady<T> Column(string columnName, string alias)
        {
            return Column(this._tableAlias, columnName, alias);
        }

        public IRDBSelectColumnsContextReady<T> ColumnToParameter(string columnName, string parameterName)
        {
            return ColumnToParameter(this._tableAlias, columnName, parameterName);
        }

        public IRDBSelectColumnsContextReady<T> Column(string columnName)
        {
            return Column(columnName, columnName);
        }

        public IRDBSelectColumnsContextReady<T> Parameter(string parameterName, string alias)
        {
            return Column(new RDBParameterExpression { ParameterName = parameterName }, alias);
        }

        public IRDBSelectColumnsContextReady<T> FixedValue(string value, string alias)
        {
            return Column(new RDBFixedTextExpression { Value = value }, alias);
        }

        public IRDBSelectColumnsContextReady<T> FixedValue(int value, string alias)
        {
            return Column(new RDBFixedIntExpression { Value = value }, alias);
        }

        public IRDBSelectColumnsContextReady<T> FixedValue(long value, string alias)
        {
            return Column(new RDBFixedLongExpression { Value = value }, alias);
        }

        public IRDBSelectColumnsContextReady<T> FixedValue(Decimal value, string alias)
        {
            return Column(new RDBFixedDecimalExpression { Value = value }, alias);
        }

        public IRDBSelectColumnsContextReady<T> FixedValue(float value, string alias)
        {
            return Column(new RDBFixedFloatExpression { Value = value }, alias);
        }

        public IRDBSelectColumnsContextReady<T> FixedValue(DateTime value, string alias)
        {
            return Column(new RDBFixedDateTimeExpression { Value = value }, alias);
        }

        public IRDBSelectColumnsContextReady<T> FixedValue(bool value, string alias)
        {
            return Column(new RDBFixedBooleanExpression { Value = value }, alias);
        }

        public IRDBSelectColumnsContextReady<T> FixedValue(Guid value, string alias)
        {
            return Column(new RDBFixedGuidExpression { Value = value }, alias);
        }

        T IRDBSelectColumnsContextReady<T>.EndColumns()
        {
            return _parent;
        }
    }
}
