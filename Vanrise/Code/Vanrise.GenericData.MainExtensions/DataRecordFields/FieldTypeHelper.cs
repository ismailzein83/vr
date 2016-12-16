using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldTypeHelper
    {
        public static IEnumerable<T> ConvertFieldValueToList<T>(object fieldValue)
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

            if (fieldValue != null)
            {
                var fieldValueType = fieldValue.GetType();
                if ((fieldValueType.IsGenericType && fieldValueType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
                    || typeof(IEnumerable<T>).IsAssignableFrom(fieldValueType))
                {
                    var objList = fieldValue as System.Collections.IEnumerable;
                    if (objList != null)
                    {
                        List<T> values = new List<T>();
                        var enumerator = objList.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current != null)
                                values.Add((T)Convert.ChangeType(enumerator.Current, typeof(T)));
                        }
                        return values;
                    }
                }
            }
            return null;
        }
    }
}
