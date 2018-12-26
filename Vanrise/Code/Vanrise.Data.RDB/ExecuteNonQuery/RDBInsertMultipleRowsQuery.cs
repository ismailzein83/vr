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
        List<RDBInsertMultipleRowsQueryRow> _rows = new List<RDBInsertMultipleRowsQueryRow>();
        HashSet<string> _columns = new HashSet<string>();


        public void IntoTable(IRDBTableQuerySource table)
        {
            this._table = table;
            _queryBuilderContext.SetMainQueryTable(table);
        }

        public void IntoTable(string tableName)
        {
            IntoTable(new RDBTableDefinitionQuerySource(tableName));
        }
        
        public RDBInsertMultipleRowsQueryRowContext AddRow()
        {
            var row = new RDBInsertMultipleRowsQueryRow();
            this._rows.Add(row);
            return new RDBInsertMultipleRowsQueryRowContext(this._queryBuilderContext, _columns, row);
        }

        public override RDBResolvedQuery GetResolvedQuery(IRDBQueryGetResolvedQueryContext context)
        {
            var resolvedQuery = new RDBResolvedQuery();
            foreach (var row in _rows)
            {
                var resolveInsertQueryContext = new RDBDataProviderResolveInsertQueryContext(this._table, row.ColumnValues, null, false, context, _queryBuilderContext);
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
            this.ColumnValues = new List<RDBInsertColumn>();
        }
        
        public List<RDBInsertColumn> ColumnValues { get; private set; }

    }
}
