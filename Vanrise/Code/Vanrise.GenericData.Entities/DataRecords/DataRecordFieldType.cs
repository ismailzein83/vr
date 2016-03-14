using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.Entities
{
    public abstract class DataRecordFieldType
    {
        public int ConfigId { get; set; }

        public abstract Type GetRuntimeType();

        public abstract string GetDescription(Object value);

        public abstract bool IsMatched(Object fieldValue, Object filterValue);

        protected IEnumerable<T> ConvertFieldValueToList<T>(object fieldValue)
        {
            var beValues = fieldValue as BusinessEntityValues;
            if (beValues != null)
            {
                var values = beValues.GetValues();
                return (values != null) ? values.Select(itm => (T)itm) : new List<T>();
            }

            var staticValues = fieldValue as StaticValues;
            if (staticValues != null)
                return staticValues.Values.Select(itm => (T)itm);

            var objList = fieldValue as List<object>;
            if (objList != null)
                return objList.Select(itm => (T)itm);

            return null;
        }

        protected Type GetNullableType(Type type)
        {
            return (type.IsValueType) ? typeof(Nullable<>).MakeGenericType(type) : type;
        }
    }   
}
