using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

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
            if (value == null)
                return null;

            IEnumerable<string> textValues = ConvertFieldValueToList<string>(value);

            if (textValues == null)
                return value.ToString();
            
            var descriptions = new List<string>();
            foreach (var textValue in textValues)
                descriptions.Add(textValue.ToString());
            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            if (fieldValue != null && filterValue != null)
            {
                var fieldValueObjList = fieldValue as List<object>;
                fieldValueObjList = (fieldValueObjList == null) ? new List<object>() { fieldValue } : fieldValueObjList;

                var fieldValueStringList = fieldValueObjList.MapRecords(itm => Convert.ToString(itm).ToUpper());
                var filterValueString = filterValue.ToString().ToUpper();
                
                foreach (var fieldValueStringItem in fieldValueStringList)
                {
                    if (fieldValueStringItem.Contains(filterValueString))
                        return true;
                }
                return false;
            }
            return true;
        }
    }
}
