using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

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
            var fieldValueList = fieldValue as List<Choice>;
            int filterValueInt = Convert.ToInt32(filterValue);
            return (fieldValueList.FindRecord(itm => itm.Value == filterValueInt) != null);
        }
    }

    public class Choice
    {
        public int Value { get; set; }

        public string Text { get; set; }
    }
}
