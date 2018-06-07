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

        int GetIntWithNullHandling(string fieldName);

        int? GetNullableInt(string fieldName);

        long GetLong(string fieldName);

        long GetLongWithNullHandling(string fieldName);

        long? GetNullableLong(string fieldName);

        decimal GetDecimal(string fieldName);

        decimal GetDecimalWithNullHandling(string fieldName);

        decimal? GetNullableDecimal(string fieldName);

        DateTime GetDateTime(string fieldName);

        DateTime GetDateTimeWithNullHandling(string fieldName);

        DateTime? GetNullableDateTime(string fieldName);

        Guid GetGuid(string fieldName);

        Guid GetGuidWithNullHandling(string fieldName);

        Guid? GetNullableGuid(string fieldName);

        Boolean GetBoolean(string fieldName);

        Boolean GetBooleanWithNullHandling(string fieldName);

        Boolean? GetNullableBoolean(string fieldName);

        byte[] GetBytes(string fieldName);

        byte[] GetBytesWithNullHandling(string fieldName);

        byte[] GetNullableBytes(string fieldName);
    }
}
