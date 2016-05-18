using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public abstract class DataRecordFieldType
    {
        public int ConfigId { get; set; }

        public abstract Type GetRuntimeType();

        public abstract Type GetNonNullableRuntimeType();

        public abstract string GetDescription(Object value);

        public abstract bool IsMatched(Object fieldValue, Object filterValue);

        public abstract bool IsMatched(Object fieldValue, RecordFilter recordFilter);

        public abstract GridColumnAttribute GetGridColumnAttribute();

        protected Type GetNullableType(Type type)
        {
            return (type.IsValueType) ? typeof(Nullable<>).MakeGenericType(type) : type;
        }
    }   
}
