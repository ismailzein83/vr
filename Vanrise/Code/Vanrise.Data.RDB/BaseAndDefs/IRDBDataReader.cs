using System;
using System.Collections.Generic;
using System.Data;
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

        Double GetDouble(string fieldName);

        Double GetDoubleWithNullHandling(string fieldName);

        Double? GetNullableDouble(string fieldName);

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

        byte[] GetNullableBytes(string fieldName);
    }

    public class CommonRDBDataReader : IRDBDataReader
    {
        IDataReader _originalReader;
        public CommonRDBDataReader(IDataReader originalReader)
        {
            _originalReader = originalReader;
        }

        public virtual bool NextResult()
        {
            return _originalReader.NextResult();
        }

        public virtual bool Read()
        {
            return _originalReader.Read();
        }

        public virtual string GetString(string fieldName)
        {
            return _originalReader[fieldName] as string;
        }

        protected virtual T GetFieldValue<T>(string fieldName)
            {
                var value = _originalReader[fieldName];
                if (value == null)
                    throw new NullReferenceException(String.Format("value of field '{0}'", fieldName));
                if (value == DBNull.Value)
                    throw new Exception(string.Format("value is DBNull. Field '{0}'", fieldName));
                return (T)value;
            }

        protected virtual T GetFieldValueWithNullHandling<T>(string fieldName)
        {
            var value = _originalReader[fieldName];
            if (value == null || value == DBNull.Value)
                return default(T);
            else
                return (T)value;
        }

        public virtual int GetInt(string fieldName)
        {
            return GetFieldValue<int>(fieldName);
        }

        public virtual int? GetNullableInt(string fieldName)
        {
            return GetFieldValueWithNullHandling<int?>(fieldName);
        }

        public virtual long GetLong(string fieldName)
        {
            return GetFieldValue<long>(fieldName);
        }

        public virtual long? GetNullableLong(string fieldName)
        {
            return GetFieldValueWithNullHandling<long?>(fieldName);
        }

        public virtual DateTime GetDateTime(string fieldName)
        {
            return GetFieldValue<DateTime>(fieldName);
        }

        public virtual DateTime? GetNullableDateTime(string fieldName)
        {
            return GetFieldValueWithNullHandling<DateTime?>(fieldName);
        }


        public virtual int GetIntWithNullHandling(string fieldName)
        {
            return GetFieldValueWithNullHandling<int>(fieldName);
        }

        public virtual long GetLongWithNullHandling(string fieldName)
        {
            return GetFieldValueWithNullHandling<long>(fieldName);
        }

        public virtual DateTime GetDateTimeWithNullHandling(string fieldName)
        {
            return GetFieldValueWithNullHandling<DateTime>(fieldName);
        }

        public virtual Guid GetGuid(string fieldName)
        {
            return GetFieldValue<Guid>(fieldName);
        }

        public virtual Guid GetGuidWithNullHandling(string fieldName)
        {
            return GetFieldValueWithNullHandling<Guid>(fieldName);
        }

        public virtual Guid? GetNullableGuid(string fieldName)
        {
            return GetFieldValueWithNullHandling<Guid?>(fieldName);
        }

        public virtual bool GetBoolean(string fieldName)
        {
            return GetFieldValue<bool>(fieldName);
        }

        public virtual bool GetBooleanWithNullHandling(string fieldName)
        {
            return GetFieldValueWithNullHandling<bool>(fieldName);
        }

        public virtual bool? GetNullableBoolean(string fieldName)
        {
            return GetFieldValueWithNullHandling<bool?>(fieldName);
        }


        public virtual decimal GetDecimal(string fieldName)
        {
            return GetFieldValue<decimal>(fieldName);
        }

        public virtual decimal GetDecimalWithNullHandling(string fieldName)
        {
            return GetFieldValueWithNullHandling<decimal>(fieldName);
        }

        public virtual decimal? GetNullableDecimal(string fieldName)
        {
            return GetFieldValueWithNullHandling<decimal?>(fieldName);
        }

        public double GetDouble(string fieldName)
        {
            return GetFieldValue<double>(fieldName);
        }
        public double GetDoubleWithNullHandling(string fieldName)
        {
            return GetFieldValueWithNullHandling<double>(fieldName);
        }

        public double? GetNullableDouble(string fieldName)
        {
            return GetFieldValueWithNullHandling<double?>(fieldName);
        }


        public virtual byte[] GetBytes(string fieldName)
        {
            return GetFieldValue<byte[]>(fieldName);
        }

        public virtual byte[] GetNullableBytes(string fieldName)
        {
            return GetFieldValueWithNullHandling<byte[]>(fieldName);
        }
    }
}
