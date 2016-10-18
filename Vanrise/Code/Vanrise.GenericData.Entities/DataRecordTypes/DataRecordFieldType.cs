using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public enum DataRecordFieldOrderType { ByFieldValue = 0, ByFieldDescription = 1 }
    public abstract class DataRecordFieldType
    {
        public abstract Guid ConfigId { get;}

        public abstract Type GetRuntimeType();

        public abstract Type GetNonNullableRuntimeType();

        public abstract string GetDescription(Object value);

        public abstract bool IsMatched(Object fieldValue, Object filterValue);

        public abstract bool IsMatched(Object fieldValue, RecordFilter recordFilter);

        public abstract GridColumnAttribute GetGridColumnAttribute();

        public abstract RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues);

        public abstract string GetFilterDescription(RecordFilter filter);

        protected Type GetNullableType(Type type)
        {
            return (type.IsValueType) ? typeof(Nullable<>).MakeGenericType(type) : type;
        }

        public virtual DataRecordFieldOrderType OrderType
        {
            get
            {
                return DataRecordFieldOrderType.ByFieldValue;
            }
        }

        public string GetValueMethod { get; set; }

        public string ConvertFilterMethod { get; set; }
    }   

    public interface IDataRecordFieldEvaluator
    {
        dynamic GetFieldValue(string fieldName, IDataRecordFieldGetValueContext context);

        RecordFilter ConvertRecordFilter(string fieldName, RecordFilter recordFilter, IDataRecordFieldConvertFilterContext context);
    }

    public interface IDataRecordFieldGetValueContext
    {
        dynamic GetFieldValue(string fieldName);
    }

    public interface IDataRecordFieldConvertFilterContext
    {

    }
}
