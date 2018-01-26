using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public interface IRDBTableQuerySource
    {
        string GetDescription();

        string ToDBQuery(IRDBTableQuerySourceToDBQueryContext context);
    }

    public interface IRDBTableQuerySourceToDBQueryContext
    {
        RDBDataProviderType DataProviderType { get; }

        Dictionary<string, Object> ParameterValues { get; }
    }
}
