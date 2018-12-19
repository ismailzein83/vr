using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;

namespace Vanrise.Data.RDB
{
    public class RDBInsertMultipleRowsQuery : BaseRDBQuery
    {
        RDBQueryBuilderContext _queryBuilderContext;

        internal RDBInsertMultipleRowsQuery(RDBQueryBuilderContext queryBuilderContext)
        {
            _queryBuilderContext = queryBuilderContext;
        }

        IRDBTableQuerySource _table;
        string[] _columns;
        List<RDBInsertMultipleRowsQueryRow> _rows = new List<RDBInsertMultipleRowsQueryRow>();

        public void IntoTable(IRDBTableQuerySource table)
        {
            this._table = table;
            _queryBuilderContext.SetMainQueryTable(table);
        }

        public void IntoTable(string tableName)
        {
            IntoTable(new RDBTableDefinitionQuerySource(tableName));
        }

        public void Columns(params string[] columnNames)
        {
            this._columns = columnNames;
        }

        public RDBInsertMultipleRowsQueryRowContext AddRow()
        {
            var row = new RDBInsertMultipleRowsQueryRow();
            this._rows.Add(row);
            return new RDBInsertMultipleRowsQueryRowContext(this._queryBuilderContext, row);
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            _columns.ThrowIfNull("_columns");
            var resolvedQuery = new RDBResolvedQuery();
            foreach (var row in _rows)
            {
                List<RDBInsertColumn> insertRowQueryColumnValues = new List<RDBInsertColumn>();
                for (int i = 0; i < _columns.Length; i++)
                {
                    if (row.Values.Count <= i)
                        throw new Exception($"Number of Row Values '{row.Values.Count}' is less than number of columns '{_columns.Length}'. Row Index '{_rows.IndexOf(row)}'");
                    insertRowQueryColumnValues.Add(new RDBInsertColumn
                    {
                        ColumnName = _columns[i],
                        Value = row.Values[i]
                    });
                }
                var resolveInsertQueryContext = new RDBDataProviderResolveInsertQueryContext(this._table, insertRowQueryColumnValues, null, false, context, _queryBuilderContext);
                var insertRowResolvedQuery = context.DataProvider.ResolveInsertQuery(resolveInsertQueryContext);
                resolvedQuery.Statements.AddRange(insertRowResolvedQuery.Statements);
            }
            return resolvedQuery;
        }
    }

    public class RDBInsertMultipleRowsQueryRow
    {
        public RDBInsertMultipleRowsQueryRow()
        {
            this.Values = new List<BaseRDBExpression>();
        }

        public List<BaseRDBExpression> Values { get; private set; }
    }
}
