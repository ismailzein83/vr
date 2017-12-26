using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
namespace Vanrise.Invoice.Business
{
    public class InvoiceItemGroupingManager
    {

        #region Public Methods
        public IEnumerable<Entities.GroupingInvoiceItemDetail> GetFilteredGroupingInvoiceItems(GroupingInvoiceItemQuery query)
        {
            InvoiceTypeManager manager = new InvoiceTypeManager();
            var invoiceType = manager.GetInvoiceType(query.InvoiceTypeId);
            var groupItem = invoiceType.Settings.ItemGroupings.FirstOrDefault(x => x.ItemGroupingId == query.ItemGroupingId);
            InvoiceItemManager invoiceItemManager = new InvoiceItemManager();
            var results = invoiceItemManager.GetInvoiceItemsByItemSetNames(query.InvoiceId, new List<string> { groupItem.ItemSetName }, CompareOperator.Equal);

            var context = new GroupingInvoiceItemQueryContext(query);
           return ApplyFinalGroupingAndFiltering(context, results, query.DimensionIds, query.MeasureIds, query.Filters, groupItem);

           
        }
        #endregion

        #region Private Methods

        private IEnumerable<GroupingInvoiceItemDetail> GetOrderedByAllDimensions(IGroupingInvoiceItemQueryContext groupingInvoiceItemQueryContext, List<Guid> dimensionIds, IEnumerable<GroupingInvoiceItemDetail> allRecords)
        {

            List<string> orderByDimensions = new List<string>();
            if (dimensionIds != null)
            {
                foreach (var dimensionId in dimensionIds)
                {
                    var dimension = groupingInvoiceItemQueryContext.GetDimensionItemField(dimensionId);
                    if (dimension != null)
                        orderByDimensions.Add(dimension.FieldName);
                }
            }
            if (orderByDimensions == null || orderByDimensions.Count == 0)
                throw new NullReferenceException("orderByDimensions");
            IOrderedEnumerable<GroupingInvoiceItemDetail> orderedRecords;
            var firstDimensionConfig = groupingInvoiceItemQueryContext.GetDimensionItemField(dimensionIds[0]);
            if (firstDimensionConfig.FieldType.OrderType == DataRecordFieldOrderType.ByFieldValue)
                orderedRecords = allRecords.OrderBy(record => record.DimensionValues[0].Value);
            else
                orderedRecords = allRecords.OrderBy(record => record.DimensionValues[0].Name);
            if (orderByDimensions.Count > 1)
            {
                for (int i = 1; i < orderByDimensions.Count; i++)
                {
                    var dimensionIndex = i;
                    var dimensionConfig = groupingInvoiceItemQueryContext.GetDimensionItemField(dimensionIds[dimensionIndex]);
                    if (dimensionConfig.FieldType.OrderType == DataRecordFieldOrderType.ByFieldValue)
                        orderedRecords = orderedRecords.ThenBy(record => record.DimensionValues[dimensionIndex].Value);
                    else
                        orderedRecords = orderedRecords.ThenBy(record => record.DimensionValues[dimensionIndex].Name);
                }
            }
            return orderedRecords;
        }
        private IEnumerable<GroupingInvoiceItemDetail> GetOrderedByAllMeasures(IGroupingInvoiceItemQueryContext groupingInvoiceItemQueryContext,List<Guid> measureIds, IEnumerable<GroupingInvoiceItemDetail> allRecords)
        {

            List<string> orderByMeasures = new List<string>();
            if (measureIds != null)
            {
                foreach (var measureId in measureIds)
                {
                    var measure = groupingInvoiceItemQueryContext.GetAggregateItemField(measureId);
                    if (measure != null)
                        orderByMeasures.Add(measure.FieldName);
                }
            }

            if (orderByMeasures == null || orderByMeasures.Count() == 0)
                throw new NullReferenceException("orderByMeasures");
            string firstMeasureName = orderByMeasures[0];
            IOrderedEnumerable<GroupingInvoiceItemDetail> orderedRecords = allRecords.OrderByDescending(record => record.MeasureValues[firstMeasureName].Value);
            if (orderByMeasures.Count > 1)
            {
                for (int i = 1; i < orderByMeasures.Count; i++)
                {
                    string measureName = orderByMeasures[i];
                    orderedRecords = orderedRecords.ThenByDescending(itm => itm.MeasureValues[measureName].Value);
                }
            }
            return orderedRecords;
        }
        public IEnumerable<GroupingInvoiceItemDetail> ApplyFinalGroupingAndFiltering(IGroupingInvoiceItemQueryContext groupingInvoiceItemQueryContext, IEnumerable<InvoiceItem> invoiceItems, List<Guid> requestedDimensionIds, List<Guid> measureIds, List<InvoiceGroupingDimensionFilter> dimensionFilters, ItemGrouping itemGrouping)
        {
            List<GroupingInvoiceItemDetail> records = new List<GroupingInvoiceItemDetail>();

            Dictionary<string, InvoiceItemRecord> groupedRecordsByDimensionsKey = new Dictionary<string, InvoiceItemRecord>();
            foreach (var invoiceItem in invoiceItems)
            {

                #region ConvertTo InvoiceItemRecord
                InvoiceItemRecord invoiceItemRecord = new InvoiceItemRecord
                {
                    GroupingValuesByDimensionId = new Dictionary<Guid, InvoiceItemGroupingValue>(),
                    AggValuesByAggId = new Dictionary<Guid, InvoiceItemAggValue>()
                };
                foreach (var dimension in itemGrouping.DimensionItemFields)
                {
                    invoiceItemRecord.GroupingValuesByDimensionId.Add(dimension.DimensionItemFieldId, new InvoiceItemGroupingValue
                    {
                        Value = invoiceItem.Details.GetType().GetProperty(dimension.FieldName).GetValue(invoiceItem.Details, null)
                        ///Vanrise.Common.Utilities.GetPropValue(dimension.FieldName, invoiceItem.Details) 
                    //Vanrise.Common.Utilities.GetPropValueReader(dimension.FieldName).GetPropertyValue(invoiceItem.Details)
                    });
                }
                foreach (var measure in itemGrouping.AggregateItemFields)
                {
                    invoiceItemRecord.AggValuesByAggId.Add(measure.AggregateItemFieldId, new InvoiceItemAggValue
                    {

                        Value = invoiceItem.Details.GetType().GetProperty(measure.FieldName).GetValue(invoiceItem.Details, null)
                        //Vanrise.Common.Utilities.GetPropValue(measure.FieldName, invoiceItem.Details)
                        //Vanrise.Common.Utilities.GetPropValueReader(measure.FieldName).GetPropertyValue(invoiceItem.Details)
                    });
                }
                #endregion

                #region ApplyFilterAndGrouping
                if (!ApplyFilters(groupingInvoiceItemQueryContext, invoiceItemRecord, dimensionFilters))
                    continue;
                string groupingKey = GetDimensionGroupingKey(requestedDimensionIds, invoiceItemRecord);
                InvoiceItemRecord matchRecord;
                if (!groupedRecordsByDimensionsKey.TryGetValue(groupingKey, out matchRecord))
                {
                    matchRecord = invoiceItemRecord;
                    groupedRecordsByDimensionsKey.Add(groupingKey, matchRecord);
                }
                else
                {
                    UpdateAggregateValues(groupingInvoiceItemQueryContext, matchRecord, invoiceItemRecord);
                }
                foreach (var groupingValue in matchRecord.GroupingValuesByDimensionId)
                {
                    if (groupingValue.Value.AllValues == null)
                        groupingValue.Value.AllValues = new List<dynamic>();
                    groupingValue.Value.AllValues.Add(invoiceItemRecord.GroupingValuesByDimensionId[groupingValue.Key].Value);
                }

                #endregion

            }


            foreach (var item in groupedRecordsByDimensionsKey.Values)
            {
                GroupingInvoiceItemDetail analyticRecord = BuildGroupingInvoiceItemDetail(groupingInvoiceItemQueryContext, item, requestedDimensionIds, measureIds);
                records.Add(analyticRecord);
            }

            IEnumerable<GroupingInvoiceItemDetail> orderedRecords;
            if (itemGrouping.OrderType.HasValue)
            {
                switch (itemGrouping.OrderType.Value)
                {
                    case ItemGroupingOrderType.ByAllDimensions: orderedRecords = GetOrderedByAllDimensions(groupingInvoiceItemQueryContext, requestedDimensionIds, records); break;
                    case ItemGroupingOrderType.ByAllMeasures: orderedRecords = GetOrderedByAllMeasures(groupingInvoiceItemQueryContext, measureIds, records); break;
                    default: orderedRecords = null; break;
                }
            }
            else
                orderedRecords = records;

            return orderedRecords;
        }
        private bool ApplyFilters(IGroupingInvoiceItemQueryContext groupingInvoiceItemQueryContext, InvoiceItemRecord invoiceItemRecord, List<InvoiceGroupingDimensionFilter> dimensionFilters)
        {
            if (dimensionFilters != null)
            {
                foreach (var dimFilter in dimensionFilters)
                {
                    InvoiceItemGroupingValue dimensionValue;
                    if (!invoiceItemRecord.GroupingValuesByDimensionId.TryGetValue(dimFilter.DimensionId, out dimensionValue))
                        throw new NullReferenceException(String.Format("dimensionValue. dimId '{0}'", dimFilter.DimensionId));
                    if (dimensionValue.Value == null)
                        return dimFilter.FilterValue == null;
                    else
                    {
                       
                        var filterValueType = dimFilter.FilterValue.GetType();
                        var convertedDimensionValue = filterValueType == typeof(string) ? dimensionValue.Value.ToString() : Convert.ChangeType(dimensionValue.Value, filterValueType);
                        if (!dimFilter.FilterValue.Equals(convertedDimensionValue))
                            return false;
                    }
                }
            }
            return true;
        }

        private string GetDimensionGroupingKey(List<Guid> requestedDimensionIds, InvoiceItemRecord invoiceItemRecord)
        {
            StringBuilder builder = new StringBuilder();
            if (requestedDimensionIds != null)
            {
                foreach (var dimId in requestedDimensionIds)
                {
                    InvoiceItemGroupingValue groupingValue;
                    if (invoiceItemRecord.GroupingValuesByDimensionId.TryGetValue(dimId, out groupingValue))
                    {
                        builder.AppendFormat("^*^{0}", groupingValue.Value != null ? groupingValue.Value : "");
                    }
                    else
                        throw new NullReferenceException(String.Format("groupingValue. dimId '{0}'", dimId));
                }
            }
            return builder.ToString();
        }

        private void UpdateAggregateValues(IGroupingInvoiceItemQueryContext groupingInvoiceItemQueryContext, InvoiceItemRecord existingRecord, InvoiceItemRecord invoiceItemRecord)
        {
            foreach (var aggEntry in invoiceItemRecord.AggValuesByAggId)
            {
                var existingAgg = existingRecord.AggValuesByAggId[aggEntry.Key];
                var agg = aggEntry.Value;
                if (existingAgg.Value == null)
                    existingAgg.Value = agg.Value;
                else if (agg.Value != null)
                {
                    switch (groupingInvoiceItemQueryContext.GetAggregateItemField(aggEntry.Key).AggregateType)
                    {
                        case AggregateType.Count:
                        case AggregateType.Sum:
                            existingAgg.Value = existingAgg.Value + agg.Value;
                            break;
                        case AggregateType.Max:
                            if (existingAgg.Value < agg.Value)
                                existingAgg.Value = agg.Value;
                            break;
                        case AggregateType.Min:
                            if (existingAgg.Value > agg.Value)
                                existingAgg.Value = agg.Value;
                            break;
                    }
                }
            }
        }

        private GroupingInvoiceItemDetail BuildGroupingInvoiceItemDetail(IGroupingInvoiceItemQueryContext groupingInvoiceItemQueryContext, InvoiceItemRecord invoiceItemRecord, List<Guid> dimensionIds, List<Guid> measureIds)
        {
            GroupingInvoiceItemDetail analyticRecord = new GroupingInvoiceItemDetail() { MeasureValues = new InvoiceGroupingMeasureValues() };

            if (dimensionIds != null)
            {
                analyticRecord.DimensionValues = new InvoiceGroupingDimensionValue[dimensionIds.Count];
                int dimIndex = 0;
                foreach (Guid dimId in dimensionIds)
                {
                    var dimensionValue = new InvoiceGroupingDimensionValue();
                    dimensionValue.Value = invoiceItemRecord.GroupingValuesByDimensionId[dimId].Value;
                    dimensionValue.Name = groupingInvoiceItemQueryContext.GetDimensionItemField(dimId).FieldType.GetDescription(dimensionValue.Value);
                    analyticRecord.DimensionValues[dimIndex] = dimensionValue;
                    dimIndex++;
                }
            }
            foreach (var measureId in measureIds)
            {
                
                var measureValue = new GetMeasureValueContext(groupingInvoiceItemQueryContext, invoiceItemRecord).GetAggregateValue(measureId);
                var measureConfig = groupingInvoiceItemQueryContext.GetAggregateItemField(measureId);
                analyticRecord.MeasureValues.Add(measureConfig.FieldName, new InvoiceGroupingMeasureValue { Value = measureValue });
            }
            return analyticRecord;
        }

        #endregion
        
    }
    public class InvoiceItemRecord
    {
        public Dictionary<Guid, InvoiceItemGroupingValue> GroupingValuesByDimensionId { get; set; }

        public Dictionary<Guid, InvoiceItemAggValue> AggValuesByAggId { get; set; }
    }
    public class InvoiceItemGroupingValue : ICloneable
    {
        public dynamic Value { get; set; }

        public List<dynamic> AllValues { get; set; }

        public object Clone()
        {
            return new InvoiceItemGroupingValue
            {
                Value = this.Value
            };
        }
    }
    public class InvoiceItemAggValue : ICloneable
    {
        public dynamic Value { get; set; }

        public object Clone()
        {
            return new InvoiceItemAggValue
            {
                Value = this.Value
            };
        }
    }
}
