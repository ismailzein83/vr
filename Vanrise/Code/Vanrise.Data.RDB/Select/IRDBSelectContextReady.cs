using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public interface IRDBSelectContextReady<T>
    {
        IRDBSelectContextReady<T> Columns(params string[] columnNames);

        IRDBSelectContextReady<T> Column(BaseRDBExpression expression, string alias);

        IRDBSelectContextReady<T> Column(IRDBTableQuerySource table, string columnName, string alias);

        IRDBSelectContextReady<T> Column(string tableName, string columnName, string alias);

        IRDBSelectContextReady<T> Column(string columnName, string alias);

        IRDBSelectContextReady<T> Column(string columnName);

        T EndSelect();
    }
}
