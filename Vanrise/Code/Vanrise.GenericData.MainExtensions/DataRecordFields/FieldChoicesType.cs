using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields.Filters;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldChoicesType : DataRecordFieldType
    {
        public List<Choice> Choices { get; set; }

        public override Type GetRuntimeType()
        {
            return typeof(int);
        }

        public override string GetDescription(Object value)
        {
            var descriptions = new List<string>();
            if (Choices != null && Choices.Count > 0)
            {
                var staticValues = value as StaticValues;
                var selectedValues = staticValues.Values.MapRecords(itm => Convert.ToInt32(itm));
                foreach (int selectedValue in selectedValues)
                    descriptions.Add(Choices.FindRecord(itm => itm.Value == selectedValue).Text);
            }
            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            var choicesFilter = filterValue as ChoicesFieldTypeFilter;
            if (fieldValue != null && choicesFilter.ChoiceIds != null)
            {
                var fieldValueObjList = fieldValue as List<object>;
                var fieldValueIds = fieldValueObjList.MapRecords(itm => Convert.ToInt32(itm));
                foreach (var filterId in choicesFilter.ChoiceIds)
                {
                    if (fieldValueIds.Contains(filterId))
                        return true;
                }
                return false;
            }
            return true;
        }
    }

    public class Choice
    {
        public int Value { get; set; }

        public string Text { get; set; }
    }
}
