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
        public override Guid ConfigId { get { return new Guid("eabc41a9-e332-4120-ac85-f0b7e53c0d0d"); } }
        public List<Choice> Choices { get; set; }
        public bool IsNullable { get; set; }

        public override DataRecordFieldOrderType OrderType
        {
            get
            {
                return DataRecordFieldOrderType.ByFieldDescription;
            }
        }

        #region Public Methods

        public override Type GetRuntimeType()
        {
            var type = GetNonNullableRuntimeType();
            return (IsNullable) ? GetNullableType(type) : type;
        }

        public override Type GetNonNullableRuntimeType()
        {
            return typeof(int);
        }

        public override string GetDescription(object value)
        {
            if (value == null)
                return null;

            if (Choices == null)
                throw new NullReferenceException("Choices");

            IEnumerable<long> selectedChoiceValues = FieldTypeHelper.ConvertFieldValueToList<long>(value);
            
            if (selectedChoiceValues == null)
                return GetChoiceText(Convert.ToInt32(value));

            var descriptions = new List<string>();

            foreach (int selectedChoiceValue in selectedChoiceValues)
                descriptions.Add(GetChoiceText(selectedChoiceValue));
            
            return String.Join(",", descriptions);
        }

        public override bool IsMatched(object fieldValue, object filterValue)
        {
            var choicesFilter = filterValue as ChoicesFieldTypeFilter;
            if (fieldValue != null && choicesFilter.ChoiceIds != null)
            {
                var fieldValueObjList = fieldValue as List<object>;
                fieldValueObjList = (fieldValueObjList == null) ? new List<object>() { fieldValue } : fieldValueObjList;

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

        public override bool IsMatched(object fieldValue, RecordFilter recordFilter)
        {
            NumberListRecordFilter numberListRecordFilter = recordFilter as NumberListRecordFilter;
            if (numberListRecordFilter == null)
                throw new NullReferenceException("numberListRecordFilter");
            if (fieldValue == null)
                return numberListRecordFilter.CompareOperator == ListRecordFilterOperator.NotIn;
            bool isValueInFilter = numberListRecordFilter.Values.Contains(Convert.ToDecimal(fieldValue));

            return numberListRecordFilter.CompareOperator == ListRecordFilterOperator.In ? isValueInFilter : !isValueInFilter;
        }

        #endregion

        #region Private Methods

        string GetChoiceText(int choiceValue)
        {
            Choice choice = Choices.FindRecord(itm => itm.Value == choiceValue);
            if (choice == null) throw new NullReferenceException("choice");
            return choice.Text;
        }

        #endregion

        public override Vanrise.Entities.GridColumnAttribute GetGridColumnAttribute(FieldTypeGetGridColumnAttributeContext context)
        {
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal" };
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            return new NumberListRecordFilter
            {
                CompareOperator = ListRecordFilterOperator.In,
                Values = filterValues.Select(value => Convert.ToDecimal(value)).ToList(),
                FieldName = fieldName
            };
        }

        public override string GetFilterDescription(RecordFilter filter)
        {
            NumberListRecordFilter numberListRecordFilter = filter as NumberListRecordFilter;
            return string.Format(" {0} {1} ( {2} ) ", numberListRecordFilter.FieldName, Utilities.GetEnumDescription(numberListRecordFilter.CompareOperator), GetDescription(numberListRecordFilter.Values));
        }
    }

    public class Choice
    {
        public int Value { get; set; }

        public string Text { get; set; }
    }
}
