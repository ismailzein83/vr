using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class DataRecordFieldType
    {
        public int ConfigId { get; set; }

        public abstract Type GetRuntimeType();

        public abstract string GetDescription(Object value);

        public abstract bool IsMatched(Object fieldValue, Object filterValue);

        protected Type GetNullableType(Type type)
        {
            return (type.IsValueType) ? typeof(Nullable<>).MakeGenericType(type) : type;
        }
    }   
}
