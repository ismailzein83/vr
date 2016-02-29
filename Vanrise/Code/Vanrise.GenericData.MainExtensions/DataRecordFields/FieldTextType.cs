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
            var staticValues = value as StaticValues;

            if (staticValues == null)
                return value.ToString();
            
            var descriptions = new List<string>();
            foreach (var staticValue in staticValues.Values)
                descriptions.Add(staticValue.ToString());
            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object settingsValue, object filterValue)
        {
            return settingsValue.ToString().ToUpper().Contains(filterValue.ToString().ToUpper());
        }
    }
}
