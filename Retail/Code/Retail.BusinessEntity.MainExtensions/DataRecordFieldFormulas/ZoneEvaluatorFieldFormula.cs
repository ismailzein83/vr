using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.Common;

namespace Retail.BusinessEntity.MainExtensions.DataRecordFieldFormulas
{
    public class ZoneEvaluatorFieldFormula : DataRecordFieldFormula
    {
        public override Guid ConfigId { get { return new Guid("3A88F4C6-5CCE-4A31-A74E-E83BF73A6892"); } }
        public bool IsDestinationZoneEvaluator { get; set; }
        public string SubscriberZoneFieldName { get; set; }
        public string OtherPartyZoneFieldName { get; set; }
        public string TrafficDirectionFieldName { get; set; }
        public int TrafficDirectionInputValue { get; set; }
        public int TrafficDirectionOutputValue { get; set; }

        public override List<string> GetDependentFields(IDataRecordFieldFormulaGetDependentFieldsContext context)
        {
            return new List<string>() { SubscriberZoneFieldName, OtherPartyZoneFieldName, TrafficDirectionFieldName };
        }

        public override dynamic CalculateValue(IDataRecordFieldFormulaCalculateValueContext context)
        {
            dynamic trafficDirection = context.GetFieldValue(TrafficDirectionFieldName);
            if (trafficDirection != null)
            {
                if (trafficDirection == TrafficDirectionInputValue)
                    return IsDestinationZoneEvaluator ? context.GetFieldValue(SubscriberZoneFieldName) : context.GetFieldValue(OtherPartyZoneFieldName);

                if (trafficDirection == TrafficDirectionOutputValue)
                    return IsDestinationZoneEvaluator ? context.GetFieldValue(OtherPartyZoneFieldName) : context.GetFieldValue(SubscriberZoneFieldName);
            }
            return null;
        }

        public override Vanrise.GenericData.Entities.RecordFilter ConvertFilter(IDataRecordFieldFormulaConvertFilterContext context)
        {
            context.InitialFilter.ThrowIfNull("context.InitialFilter");

            RecordFilterGroup childRecordFilterGroup = new RecordFilterGroup();
            childRecordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.Or;

            RecordFilterGroup trafficDirectionInputRecordFilterGroup = new RecordFilterGroup();
            trafficDirectionInputRecordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.And;

            RecordFilterGroup trafficDirectionOutputRecordFilterGroup = new RecordFilterGroup();
            trafficDirectionOutputRecordFilterGroup.LogicalOperator = RecordQueryLogicalOperator.And;

            NumberRecordFilter trafficDirectionInputRecordFilter = new NumberRecordFilter() { CompareOperator = NumberRecordFilterOperator.Equals, FieldName = TrafficDirectionFieldName, Value = TrafficDirectionInputValue };
            NumberRecordFilter trafficDirectionOutputRecordFilter = new NumberRecordFilter() { CompareOperator = NumberRecordFilterOperator.Equals, FieldName = TrafficDirectionFieldName, Value = TrafficDirectionOutputValue };

            string inputZoneFieldName = IsDestinationZoneEvaluator ? this.SubscriberZoneFieldName : this.OtherPartyZoneFieldName;
            string outputZoneFieldName = IsDestinationZoneEvaluator ? this.OtherPartyZoneFieldName : this.SubscriberZoneFieldName;

            ObjectListRecordFilter objectListFilter = context.InitialFilter as ObjectListRecordFilter;
            if (objectListFilter != null)
            {
                FieldBusinessEntityType inputZoneBEFieldType;
                FieldBusinessEntityType outputZoneBEFieldType;
                GetFieldTypes(context, inputZoneFieldName, outputZoneFieldName, out inputZoneBEFieldType, out outputZoneBEFieldType);

                var inputZoneFilter = ConvertToRecordFilter(inputZoneFieldName, inputZoneBEFieldType, objectListFilter);
                var outputZoneFilter = ConvertToRecordFilter(outputZoneFieldName, outputZoneBEFieldType, objectListFilter);

                trafficDirectionInputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilter, inputZoneFilter };
                trafficDirectionOutputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionOutputRecordFilter, outputZoneFilter };

                childRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilterGroup, trafficDirectionOutputRecordFilterGroup };
                return childRecordFilterGroup;
            }

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
            {
                trafficDirectionInputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilter, new EmptyRecordFilter { FieldName = inputZoneFieldName } };
                trafficDirectionOutputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionOutputRecordFilter, new EmptyRecordFilter { FieldName = outputZoneFieldName } };

                childRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilterGroup, trafficDirectionOutputRecordFilterGroup };
                return childRecordFilterGroup;
            }

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
            {
                trafficDirectionInputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilter, new NonEmptyRecordFilter { FieldName = inputZoneFieldName } };
                trafficDirectionOutputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionOutputRecordFilter, new NonEmptyRecordFilter { FieldName = outputZoneFieldName } };

                childRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilterGroup, trafficDirectionOutputRecordFilterGroup };
                return childRecordFilterGroup;
            }

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }

        private void GetFieldTypes(IDataRecordFieldFormulaConvertFilterContext context, string inputZoneFieldName, string outputZoneFieldName, out FieldBusinessEntityType inputZoneBEFieldType, out FieldBusinessEntityType outputZoneBEFieldType)
        {
            inputZoneBEFieldType = context.GetFieldType(inputZoneFieldName) as FieldBusinessEntityType;
            if (inputZoneBEFieldType == null)
                throw new NullReferenceException(String.Format("inputZoneBEFieldType '{0}'", inputZoneFieldName));

            outputZoneBEFieldType = context.GetFieldType(outputZoneFieldName) as FieldBusinessEntityType;
            if (outputZoneBEFieldType == null)
                throw new NullReferenceException(String.Format("outputZoneBEFieldType '{0}'", outputZoneFieldName));
        }

        private RecordFilter ConvertToRecordFilter(string fieldName, FieldBusinessEntityType fieldBusinessEntityType, ObjectListRecordFilter objectListRecordFilter)
        {
            var zoneFilter = fieldBusinessEntityType.ConvertToRecordFilter(new DataRecordFieldTypeConvertToRecordFilterContext { FieldName = fieldName, FilterValues = objectListRecordFilter.Values, StrictEqual = true });
            if (zoneFilter is ObjectListRecordFilter)
                ((ObjectListRecordFilter)zoneFilter).CompareOperator = objectListRecordFilter.CompareOperator;
            return zoneFilter;
        }
    }
}