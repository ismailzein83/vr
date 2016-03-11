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
    public class FieldBooleanType : DataRecordFieldType
    {
        public override Type GetRuntimeType()
        {
            return typeof(bool);
        }

        public override string GetDescription(Object value)
        {
            if (value == null)
                return null;

            IEnumerable<bool> selectedBooleanValues = ConvertFieldValueToList<bool>(value);

            if (selectedBooleanValues == null)
                return value.ToString();
            
            var descriptions = new List<string>();

            foreach (bool selectedBooleanValue in selectedBooleanValues)
                descriptions.Add(selectedBooleanValue.ToString());

            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            IEnumerable<bool> boolValues = ConvertFieldValueToList<bool>(fieldValue);
            return (boolValues != null) ? boolValues.Contains(Convert.ToBoolean(filterValue)) : Convert.ToBoolean(fieldValue).CompareTo(Convert.ToBoolean(filterValue)) == 0;
        }
    }
}
