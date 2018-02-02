using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Data.RDB
{
    public interface IRDBDataReader
    {
        bool NextResult();

        bool Read();

        string GetString(string fieldName);

        int GetInt(string fieldName);

        int? GetNullableInt(string fieldName);

        long GetLong(string fieldName);

        long? GetNullableLong(string fieldName);

        DateTime GetDateTime(string fieldName);

        DateTime? GetNullableDateTime(string fieldName);
    }
}
