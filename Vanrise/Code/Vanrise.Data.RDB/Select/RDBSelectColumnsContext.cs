﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public enum RDBNonCountAggregateType { SUM = 0, AVG = 1, MAX = 2, MIN = 3 }
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
            foreach (var colName in tableQuerySource.GetColumnNames(new RDBTableQuerySourceGetColumnNamesContext()))
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

        public void Column(string tableAlias, string columnName, string alias)
        {
            Column(new RDBColumnExpression(_queryBuilderContext, tableAlias, columnName),
                    alias);
        }

        public void Column(string columnName, string alias)
        {
            Column(this._tableAlias, columnName, alias);
        }

        public void Column(string columnName)
        {
            Column(columnName, columnName);
        }

        public RDBExpressionContext Expression(string alias)
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => Column(exp, alias), _tableAlias);
        }

        public void Count(string alias)
        {
            Column(new RDBCountExpression(), alias);
        }

        public void Aggregate(RDBNonCountAggregateType aggregateType, BaseRDBExpression expression, string alias)
        {
            BaseRDBExpression aggregateExpression = RDBExpressionContext.CreateNonCountAggregate(aggregateType, expression);
            Column(aggregateExpression, alias);
        }
        
        public void Aggregate(RDBNonCountAggregateType aggregateType, string tableAlias, string columnName, string alias)
        {
            Aggregate(aggregateType, new RDBColumnExpression(_queryBuilderContext, tableAlias, columnName), alias);
        }

        public void Aggregate(RDBNonCountAggregateType aggregateType, string columnName, string alias)
        {
            Aggregate(aggregateType, _tableAlias, columnName, alias);
        }

        public void Aggregate(RDBNonCountAggregateType aggregateType, string columnName)
        {
            Aggregate(aggregateType, columnName, columnName);
        }

        public RDBExpressionContext ExpressionAggregate(RDBNonCountAggregateType aggregateType, string alias)
        {
            return new RDBExpressionContext(_queryBuilderContext, (exp) => Aggregate(aggregateType, exp, alias), _tableAlias);
        }
    }
}