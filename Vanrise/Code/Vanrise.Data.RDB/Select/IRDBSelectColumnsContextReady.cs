using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public interface IRDBSelectColumnsContextReady<T>
    {
        IRDBSelectColumnsContextReady<T> Columns(params string[] columnNames);

        IRDBSelectColumnsContextReady<T> Column(BaseRDBExpression expression, string alias);

        IRDBSelectColumnsContextReady<T> Column(IRDBTableQuerySource table, string columnName, string alias);

        IRDBSelectColumnsContextReady<T> Column(string tableName, string columnName, string alias);

        IRDBSelectColumnsContextReady<T> Column(string columnName, string alias);

        IRDBSelectColumnsContextReady<T> Column(string columnName);

        T EndColumns();
    }
}
