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
                if (IsDestinationZoneEvaluator)
                {
                    if (trafficDirection == TrafficDirectionInputValue)
                        return context.GetFieldValue(SubscriberZoneFieldName);

                    if (trafficDirection == TrafficDirectionOutputValue)
                        return context.GetFieldValue(OtherPartyZoneFieldName);
                }
                else
                {
                    if (trafficDirection == TrafficDirectionInputValue)
                        return context.GetFieldValue(OtherPartyZoneFieldName);

                    if (trafficDirection == TrafficDirectionOutputValue)
                        return context.GetFieldValue(SubscriberZoneFieldName);
                }
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

            ObjectListRecordFilter objectListFilter = context.InitialFilter as ObjectListRecordFilter;
            if (objectListFilter != null)
            {
                FieldBusinessEntityType subscriberZoneBEFieldType;
                FieldBusinessEntityType otherPartyZoneBEFieldType;
                GetFieldTypes(context, out subscriberZoneBEFieldType, out otherPartyZoneBEFieldType);

                var subscriberZoneFilter = ConvertToRecordFilter(this.SubscriberZoneFieldName, subscriberZoneBEFieldType, objectListFilter);
                var otherPartyZoneFilter = ConvertToRecordFilter(this.OtherPartyZoneFieldName, otherPartyZoneBEFieldType, objectListFilter);

                if (IsDestinationZoneEvaluator)
                {
                    trafficDirectionInputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilter, subscriberZoneFilter };
                    trafficDirectionOutputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionOutputRecordFilter, otherPartyZoneFilter };
                }
                else
                {
                    trafficDirectionInputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilter, otherPartyZoneFilter };
                    trafficDirectionOutputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionOutputRecordFilter, subscriberZoneFilter };
                }

                childRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilterGroup, trafficDirectionOutputRecordFilterGroup };
                return childRecordFilterGroup;
            }

            EmptyRecordFilter emptyFilter = context.InitialFilter as EmptyRecordFilter;
            if (emptyFilter != null)
            {
                trafficDirectionInputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilter, new EmptyRecordFilter { FieldName = this.SubscriberZoneFieldName } };
                trafficDirectionOutputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionOutputRecordFilter, new EmptyRecordFilter { FieldName = this.OtherPartyZoneFieldName } };
                childRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilterGroup, trafficDirectionOutputRecordFilterGroup };
                return childRecordFilterGroup;
            }

            NonEmptyRecordFilter nonEmptyFilter = context.InitialFilter as NonEmptyRecordFilter;
            if (nonEmptyFilter != null)
            {
                trafficDirectionInputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilter, new NonEmptyRecordFilter { FieldName = this.SubscriberZoneFieldName } };
                trafficDirectionOutputRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionOutputRecordFilter, new NonEmptyRecordFilter { FieldName = this.OtherPartyZoneFieldName } };
                childRecordFilterGroup.Filters = new List<RecordFilter>() { trafficDirectionInputRecordFilterGroup, trafficDirectionOutputRecordFilterGroup };
                return childRecordFilterGroup;
            }

            throw new Exception(String.Format("Invalid Record Filter '{0}'", context.InitialFilter.GetType()));
        }


        private void GetFieldTypes(IDataRecordFieldFormulaContext context, out FieldBusinessEntityType subscriberZoneBEFieldType, out FieldBusinessEntityType otherPartyZoneBEFieldType)
        {
            subscriberZoneBEFieldType = context.GetFieldType(this.SubscriberZoneFieldName) as FieldBusinessEntityType;
            if (subscriberZoneBEFieldType == null)
                throw new NullReferenceException(String.Format("subscriberZoneBEFieldType '{0}'", this.SubscriberZoneFieldName));

            otherPartyZoneBEFieldType = context.GetFieldType(this.OtherPartyZoneFieldName) as FieldBusinessEntityType;
            if (otherPartyZoneBEFieldType == null)
                throw new NullReferenceException(String.Format("otherPartyZoneBEFieldType '{0}'", this.OtherPartyZoneFieldName));
        }

        private RecordFilter ConvertToRecordFilter(string fieldName, FieldBusinessEntityType fieldBusinessEntityType, ObjectListRecordFilter objectListRecordFilter)
        {
            var zoneFilter = fieldBusinessEntityType.ConvertToRecordFilter(fieldName, objectListRecordFilter.Values);
            if (zoneFilter is ObjectListRecordFilter)
                ((ObjectListRecordFilter)zoneFilter).CompareOperator = objectListRecordFilter.CompareOperator;
            return zoneFilter;
        }
    }
}
