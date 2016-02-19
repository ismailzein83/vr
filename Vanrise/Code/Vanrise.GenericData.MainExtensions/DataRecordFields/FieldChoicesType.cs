using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields.Filters;

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
            if (Choices != null && Choices.Count > 0)
            {
                int selectedChoiceValue = (int)value;
                Choice selectedChoice = Choices.Find(choice => choice.Value == selectedChoiceValue);
                return (selectedChoice != null) ? selectedChoice.Text : null;
            }
            return null;
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
