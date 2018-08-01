using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBSelectColumnsContext
    {
        RDBQueryBuilderContext _queryBuilderContext;
        List<RDBSelectColumn> _columns;
        IRDBTableQuerySource _table;
        string _tableAlias;

        internal RDBSelectColumnsContext(RDBQueryBuilderContext queryBuilderContext, List<RDBSelectColumn> columns, IRDBTableQuerySource table, string tableAlias)
        {
            _queryBuilderContext = queryBuilderContext;
            _columns = columns;
            _table = table;
            _tableAlias = tableAlias;
        }

        public void Columns(params string[] columnNames)
        {
            foreach (var colName in columnNames)
            {
                Column(colName);
            }
        }

        public void AllTableColumns(string tableAlias)
        {
            var tableQuerySource = _queryBuilderContext.GetTableFromAlias(tableAlias);
            RDBTableDefinitionQuerySource table = tableQuerySource.CastWithValidate<RDBTableDefinitionQuerySource>("tableQuerySource", tableAlias);
            foreach (var colName in RDBSchemaManager.Current.GetTableColumns(table.TableName))
            {
                Column(tableAlias, colName, colName);
            }
        }

        public void Column(BaseRDBExpression expression, string alias)
        {
            _columns.Add(new RDBSelectColumn
            {
                Expression = expression,
                Alias = alias
            });
        }

        public void ColumnToParameter(BaseRDBExpression expression, string parameterName)
        {
            string dbParameterName = _queryBuilderContext.DataProvider.ConvertToDBParameterName(parameterName);
            _columns.Add(new RDBSelectColumn
            {
                Expression = expression,
                SetDBParameterName = dbParameterName
            });
        }

        public void Column(string tableAlias, string columnName, string alias)
        {
            Column(new RDBColumnExpression
           {
               TableAlias = tableAlias,
               ColumnName = columnName
           },
                    alias);
        }

        public void ColumnToParameter(string tableAlias, string columnName, string parameterName)
        {
            ColumnToParameter(new RDBColumnExpression
           {
               TableAlias = tableAlias,
               ColumnName = columnName
           },
                    parameterName);
        }

        public void Column(string columnName, string alias)
        {
            Column(this._tableAlias, columnName, alias);
        }

        public void ColumnToParameter(string columnName, string parameterName)
        {
            ColumnToParameter(this._tableAlias, columnName, parameterName);
        }

        public void Column(string columnName)
        {
            Column(columnName, columnName);
        }

        public RDBExpressionContext Expression(string alias)
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => Column(exp, alias), _tableAlias);
        }
    }
}