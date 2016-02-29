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

        #region Public Methods

        public override Type GetRuntimeType()
        {
            return typeof(int);
        }

        public override string GetDescription(object value)
        {
            if (value == null)
                return null;

            if (Choices == null)
                throw new NullReferenceException("Choices");
            
            IEnumerable<int> selectedChoiceValues = ConvertValueToSelectedChoiceValues(value);
            
            if (selectedChoiceValues == null)
                return GetChoiceText(Convert.ToInt32(value));

            var descriptions = new List<string>();

            foreach (int selectedChoiceValue in selectedChoiceValues)
                descriptions.Add(GetChoiceText(selectedChoiceValue));
            
            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object settingsValue, object filterValue)
        {
            var choicesFilter = filterValue as ChoicesFieldTypeFilter;
            return choicesFilter.ChoiceIds.Contains(Convert.ToInt32(settingsValue));
        }

        #endregion

        #region Private Methods

        IEnumerable<int> ConvertValueToSelectedChoiceValues(object value)
        {
            var staticValues = value as StaticValues;
            if (staticValues != null)
                return staticValues.Values.MapRecords(itm => Convert.ToInt32(itm));

            var objList = value as List<object>;
            if (objList != null)
                return objList.MapRecords(itm => Convert.ToInt32(itm));

            return null;
        }

        string GetChoiceText(int choiceValue)
        {
            Choice choice = Choices.FindRecord(itm => itm.Value == choiceValue);
            if (choice == null) throw new NullReferenceException("choice");
            return choice.Text;
        }

        #endregion
    }

    public class Choice
    {
        public int Value { get; set; }

        public string Text { get; set; }
    }
}
