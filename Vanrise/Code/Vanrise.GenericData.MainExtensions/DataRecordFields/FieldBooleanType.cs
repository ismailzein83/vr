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

            IEnumerable<bool> selectedBooleanValues = ConvertValueToSelectedBooleanValues(value);

            if (selectedBooleanValues == null)
                return value.ToString();
            
            var descriptions = new List<string>();

            foreach (bool selectedBooleanValue in selectedBooleanValues)
                descriptions.Add(selectedBooleanValue.ToString());

            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            return Convert.ToBoolean(fieldValue).CompareTo(Convert.ToBoolean(filterValue)) == 0;
        }

        #region Private Methods

        IEnumerable<bool> ConvertValueToSelectedBooleanValues(object value)
        {
            var staticValues = value as StaticValues;
            if (staticValues != null)
                return staticValues.Values.MapRecords(itm => Convert.ToBoolean(itm));

            var objList = value as List<object>;
            if (objList != null)
                return objList.MapRecords(itm => Convert.ToBoolean(itm));

            return null;
        }

        #endregion
    }
}
