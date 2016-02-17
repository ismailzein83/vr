using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldTextType : DataRecordFieldType
    {
        public override Type GetRuntimeType()
        {
            return typeof(string);
        }

        public override string GetDescription(Object value)
        {
            return value.ToString();
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            if (filterValue != null)
            {
                var fieldValueList = ConvertObjectToStringList(fieldValue);
                var filterValueString = filterValue.ToString().ToUpper();
                
                foreach (var fieldValueListItem in fieldValueList)
                {
                    if (fieldValueListItem.ToUpper().Contains(filterValueString))
                        return true;
                }
            }
            return false;
        }

        IEnumerable<string> ConvertObjectToStringList(object target)
        {
            if (target == null) return null;

            var objectList = target as List<object>;
            var stringList = new List<string>();

            foreach (var objectListItem in objectList)
            {
                stringList.Add(objectListItem.ToString());
            }

            return stringList;
        }
    }
}
