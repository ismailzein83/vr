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

        IRDBSelectColumnsContextReady<T> AllTableColumns(string tableAlias);

        IRDBSelectColumnsContextReady<T> Column(BaseRDBExpression expression, string alias);

        IRDBSelectColumnsContextReady<T> ColumnToParameter(BaseRDBExpression expression, string parameterName);

        IRDBSelectColumnsContextReady<T> Column(string tableAlias, string columnName, string alias);

        IRDBSelectColumnsContextReady<T> ColumnToParameter(string tableAlias, string columnName, string parameterName);

        IRDBSelectColumnsContextReady<T> Column(string columnName, string alias);

        IRDBSelectColumnsContextReady<T> ColumnToParameter(string columnName, string parameterName);

        IRDBSelectColumnsContextReady<T> Column(string columnName);

        IRDBSelectColumnsContextReady<T> Parameter(string parameterName, string alias);

        IRDBSelectColumnsContextReady<T> FixedValue(string value, string alias);

        IRDBSelectColumnsContextReady<T> FixedValue(int value, string alias);

        IRDBSelectColumnsContextReady<T> FixedValue(long value, string alias);

        IRDBSelectColumnsContextReady<T> FixedValue(Decimal value, string alias);

        IRDBSelectColumnsContextReady<T> FixedValue(float value, string alias);

        IRDBSelectColumnsContextReady<T> FixedValue(DateTime value, string alias);

        IRDBSelectColumnsContextReady<T> FixedValue(bool value, string alias);

        IRDBSelectColumnsContextReady<T> FixedValue(Guid value, string alias);

        T EndColumns();
    }
}
