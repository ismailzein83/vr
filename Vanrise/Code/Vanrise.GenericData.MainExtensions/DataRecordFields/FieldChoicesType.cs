using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields.Filters;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;

namespace Vanrise.GenericData.MainExtensions.DataRecordFields
{
    public class FieldChoicesType : DataRecordFieldType
    {
        
        public override Guid ConfigId { get { return new Guid("eabc41a9-e332-4120-ac85-f0b7e53c0d0d"); } }
        
        public override string RuntimeEditor { get { return "vr-genericdata-fieldtype-choices-runtimeeditor"; } }

        List<Choice> _choices;
        public List<Choice> Choices
        {
            get
            {
                if (this.ChoiceDefinitionId.HasValue)
                {
                    return GetChoices(this.ChoiceDefinitionId.Value);
                }
                else
                {
                    return _choices;
                }
            }
            set
            {
                _choices = value;
            }
        }

        public Guid? ChoiceDefinitionId { get; set; }

        public bool IsNullable { get; set; }

        public override DataRecordFieldOrderType OrderType
        {
            get
            {
                return DataRecordFieldOrderType.ByFieldDescription;
            }
        }

        #region Public Methods

        Type _runtimeType;
        public override Type GetRuntimeType()
        {
            if (_runtimeType == null)
            {
                var type = GetNonNullableRuntimeType();
                lock (this)
                {
                    if (_runtimeType == null)
                        _runtimeType = (IsNullable) ? GetNullableType(type) : type;
                }
            }
            return _runtimeType;
        }

        Type _nonNullableRuntimeType = typeof(int);
        public override Type GetNonNullableRuntimeType()
        {
            return _nonNullableRuntimeType;
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

        public override bool RenderDescriptionByDefault()
        {
            return true;
        }

        public override string GetRuntimeTypeDescription()
        {
            return "Choices";
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
            return new Vanrise.Entities.GridColumnAttribute() { Type = "Text", NumberPrecision = "NoDecimal", Field = context != null ? context.DescriptionFieldPath : null };
        }

        public override RecordFilter ConvertToRecordFilter(string fieldName, List<Object> filterValues)
        {
            if (filterValues == null || filterValues.Count == 0)
                return null;

            return new NumberListRecordFilter
            {
                CompareOperator = ListRecordFilterOperator.In,
                Values = filterValues.Select(value => Convert.ToDecimal(value)).ToList(),
                FieldName = fieldName
            };
        }
        public override void GetValueByDescription(IGetValueByDescriptionContext context)
        {

            if (context.FieldDescription == null)
                return;
            var choice = Choices.FindRecord(x => x.Text.Equals(context.FieldDescription.ToString().Trim(), StringComparison.InvariantCultureIgnoreCase));
            if (choice != null)
                context.FieldValue = (long)choice.Value;
            else
                context.ErrorMessage = string.Format("The description {0} does not exist.",context.FieldDescription.ToString());
        }

        List<Choice> GetChoices(Guid choiceDefinitionId)
        {
            List<Choice> choices = GetCachedChoicesByDefinitionId().GetRecord(choiceDefinitionId);
            choices.ThrowIfNull("choices", choiceDefinitionId);
            return choices;
        }

        Dictionary<Guid, List<Choice>> GetCachedChoicesByDefinitionId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataRecordFieldChoiceManager.CacheManager>().GetOrCreateObject("GetCachedChoicesByDefinitionId",
                () =>
                {
                    DataRecordFieldChoiceManager choiceDefinitionManager = new DataRecordFieldChoiceManager();
                    var allChoiceDefinitions = choiceDefinitionManager.GetCachedDataRecordFieldChoices();
                    allChoiceDefinitions.ThrowIfNull("allChoiceDefinitions");
                    Dictionary<Guid, List<Choice>> choices = new Dictionary<Guid, List<Choice>>();
                    foreach(var choiceDefinition in allChoiceDefinitions.Values)
                    {
                        choiceDefinition.Settings.ThrowIfNull("choiceDefinition.Settings", choiceDefinition.DataRecordFieldChoiceId);
                        choiceDefinition.Settings.Choices.ThrowIfNull("choiceDefinition.Settings.Choices", choiceDefinition.DataRecordFieldChoiceId);
                        choices.Add(choiceDefinition.DataRecordFieldChoiceId, choiceDefinition.Settings.Choices.Select(itm => new Choice
                            {
                                Text = itm.Text,
                                Value = itm.Value
                            }).ToList());
                    }
                    return choices;
                });
        }
    }

    public class Choice
    {
        public int Value { get; set; }

        public string Text { get; set; }
    }
}
