using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace Retail.Interconnect.Business
{
    public class InterconnectInvoiceGenerationManager
    {
        static InterconnectInvoiceGenerationManager()
        {
            s_nullableInlineTypes.Add(typeof(int?), typeof(int));
            s_nullableInlineTypes.Add(typeof(long?), typeof(long));
            s_nullableInlineTypes.Add(typeof(short?), typeof(short));
            s_nullableInlineTypes.Add(typeof(decimal?), typeof(decimal));
            s_nullableInlineTypes.Add(typeof(float?), typeof(float));
            s_nullableInlineTypes.Add(typeof(double?), typeof(double));
        }

        Guid invoiceVoiceAnalyticTableId = Guid.Parse("6cd535c0-ac49-46bb-aecf-0eae33823b20");
        Guid invoiceSMSAnalyticTableId = Guid.Parse("c1bd3f2f-6213-44d1-9d58-99f81e169930");
        FinancialAccountManager _financialAccountManager = new FinancialAccountManager();
        public void AddGeneratedInvoiceItemSet<T>(string itemSetName, List<GeneratedInvoiceItemSet> generatedInvoiceItemSets, IEnumerable<T> items)
        {
            if (items != null && items.Count() > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = itemSetName;
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
                foreach (var item in items)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = item,
                        Name = " "
                    });
                }
                generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
            }
        }
        public List<T> GetInvoiceVoiceMappedRecords<T>(List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimensionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId,  Func<AnalyticRecord, T> mapper) where T : class
        {
            return GetInvoiceMappedRecords(invoiceVoiceAnalyticTableId, listDimensions, listMeasures, dimensionFilterName, dimensionFilterValue, fromDate, toDate, currencyId, mapper);
        }
        public List<T> GetInvoiceSMSMappedRecords<T>(List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimensionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId  ,Func<AnalyticRecord, T> mapper) where T : class
        {
            return GetInvoiceMappedRecords(invoiceSMSAnalyticTableId, listDimensions, listMeasures, dimensionFilterName, dimensionFilterValue, fromDate, toDate, currencyId,  mapper);
        }
        public T GetMeasureValue<T>(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            if (analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue))
            {
                if (measureValue.Value != null)
                {
                    if (measureValue.Value is T)
                        return (T)measureValue.Value;
                    else
                        return (T)Convert.ChangeType(measureValue.Value, GetInlineType(typeof(T)));
                }

            }
            return default(T);
        }
        public T GetDimensionValue<T>(AnalyticRecord analyticRecord, int dimIndex)
        {
            if (analyticRecord.DimensionValues != null)
            {
                DimensionValue dimension = analyticRecord.DimensionValues.ElementAtOrDefault(dimIndex);
                if (dimension.Value != null)
                {
                    if (dimension.Value is T)
                        return (T)dimension.Value;
                    else
                        return (T)Convert.ChangeType(dimension.Value, GetInlineType(typeof(T)));
                    //return (T)dimension.Value;
                    // return  (T)Convert.ChangeType(dimension.Value, typeof(T));
                }

            }
            return default(T);
        }
        static Dictionary<Type, Type> s_nullableInlineTypes = new Dictionary<Type, Type>();
        internal static Type GetInlineType(Type nullableType)
        {
            Type inlineType;
            if (s_nullableInlineTypes.TryGetValue(nullableType, out inlineType))
                return inlineType;
            else
                return nullableType;
        }
        private List<T> GetInvoiceMappedRecords<T>(Guid analyticTableId, List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimensionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId,  Func<AnalyticRecord, T> mapper) where T : class
        {
            var analyticRecords = GetInvoiceAnalyticRecords(analyticTableId, listDimensions, listMeasures, dimensionFilterName, dimensionFilterValue, fromDate, toDate, currencyId);
            if (analyticRecords != null && analyticRecords.Data != null && analyticRecords.Data.Count() > 0)
            {
                return PrepareItemSetNames<T>(analyticRecords.Data, mapper);
            }
            return default(List<T>);
        }
        private AnalyticSummaryBigResult<AnalyticRecord> GetInvoiceAnalyticRecords(Guid analyticTableId, List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimensionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId)
        {
            Dictionary<string, dynamic> queryParameters = null;
            AnalyticManager analyticManager = new AnalyticManager();
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
                    MeasureFields = listMeasures,
                    TableId = analyticTableId,
                    FromTime = fromDate,
                    ToTime = toDate,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                    CurrencyId = currencyId,
                    QueryParameters = queryParameters
                },
                SortByColumnName = "DimensionValues[0].Name"
            };
            DimensionFilter dimensionFilter = new DimensionFilter()
            {
                Dimension = dimensionFilterName,
                FilterValues = new List<object> { dimensionFilterValue }
            };
            analyticQuery.Query.Filters.Add(dimensionFilter);
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }
        public List<T> PrepareItemSetNames<T>(IEnumerable<AnalyticRecord> analyticRecords, Func<AnalyticRecord, T> mapper) where T : class
        {
            List<T> itemSetNames = null;
            if (analyticRecords != null)
            {
                itemSetNames = new List<T>();
                foreach (var analyticRecord in analyticRecords)
                {
                    var mappedItem = mapper(analyticRecord);
                    if (mappedItem != null)
                        itemSetNames.Add(mapper(analyticRecord));
                }
            }
            return itemSetNames;
        }
    }
}
