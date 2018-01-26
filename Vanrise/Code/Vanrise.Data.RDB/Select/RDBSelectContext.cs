using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public class RDBSelectContext<T> : IRDBSelectContextReady<T>
    {
        T _parent;
        List<RDBSelectColumn> _columns;
        IRDBTableQuerySource _table;

        public RDBSelectContext(T parent, List<RDBSelectColumn> columns, IRDBTableQuerySource table)
        {
            _parent = parent;
            _columns = columns;
            _table = table;
        }

        public IRDBSelectContextReady<T> Columns(params string[] columnNames)
        {
            foreach (var colName in columnNames)
            {
                Column(colName);
            }
            return this;
        }

        public IRDBSelectContextReady<T> Column(RDBColumnExpression expression, string alias)
        {
            _columns.Add(new RDBSelectColumn
            {
                Expression = expression,
                Alias = alias
            });
            return this;
        }

        public IRDBSelectContextReady<T> Column(IRDBTableQuerySource table, string columnName, string alias)
        {
            return Column(new RDBColumnExpression
            {
                Table = table,
                ColumnName = columnName
            },
                     alias);
        }

        public IRDBSelectContextReady<T> Column(string columnName, string alias)
        {
            return Column(_table, columnName, alias);
        }

        public IRDBSelectContextReady<T> Column(string columnName)
        {
            return Column(columnName, columnName);
        }

        T IRDBSelectContextReady<T>.EndSelect()
        {
            return _parent;
        }
    }
}
