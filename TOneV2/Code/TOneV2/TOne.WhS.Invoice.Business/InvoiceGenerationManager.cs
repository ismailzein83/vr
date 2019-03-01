using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business
{
    public class InvoiceGenerationManager
    {
        static InvoiceGenerationManager()
        {
            s_nullableInlineTypes.Add(typeof(int?), typeof(int));
            s_nullableInlineTypes.Add(typeof(long?), typeof(long));
            s_nullableInlineTypes.Add(typeof(short?), typeof(short));
            s_nullableInlineTypes.Add(typeof(decimal?), typeof(decimal));
            s_nullableInlineTypes.Add(typeof(float?), typeof(float));
            s_nullableInlineTypes.Add(typeof(double?), typeof(double));
        }
        Guid invoiceVoiceAnalyticTableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B");
        Guid invoiceSMSAnalyticTableId = Guid.Parse("53e9ebc8-c674-4aff-b6c0-9b3b18f95c1f");
        WHSFinancialAccountManager _financialAccountManager = new WHSFinancialAccountManager();
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
        public List<T> GetInvoiceVoiceMappedRecords<T>(List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId, TimeSpan? offsetValue, Func<AnalyticRecord, T> mapper)where T:class
        {
            return GetInvoiceMappedRecords(invoiceVoiceAnalyticTableId, listDimensions, listMeasures, dimensionFilterName, dimentionFilterValue, fromDate, toDate, currencyId, offsetValue,mapper);
        }
        public List<T> GetInvoiceSMSMappedRecords<T>( List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId, TimeSpan? offsetValue, Func<AnalyticRecord, T> mapper) where T : class
        {
            return GetInvoiceMappedRecords(invoiceSMSAnalyticTableId, listDimensions, listMeasures, dimensionFilterName, dimentionFilterValue, fromDate, toDate, currencyId, offsetValue,mapper);
        }
        public ResolvedInvoicePayloadObject GetDatesWithTimeZone<T>(T customSectionPayload, int financialAccountId, DateTime fromDate, DateTime toDate) where T : BaseGenerationCustomSectionPayload
        {
            ResolvedInvoicePayloadObject resolvedInvoicePayloadObject = new ResolvedInvoicePayloadObject();
            var generationCustomSectionPayload = customSectionPayload as BaseGenerationCustomSectionPayload;
            if (generationCustomSectionPayload != null)
            {
                resolvedInvoicePayloadObject.TimeZoneId = generationCustomSectionPayload.TimeZoneId;
                if (generationCustomSectionPayload.Commission.HasValue)
                {
                    resolvedInvoicePayloadObject.Commission = generationCustomSectionPayload.Commission.Value;
                    resolvedInvoicePayloadObject.CommissionType = generationCustomSectionPayload.CommissionType;
                }

            }
            if (!resolvedInvoicePayloadObject.TimeZoneId.HasValue)
            {
                resolvedInvoicePayloadObject.TimeZoneId = _financialAccountManager.GetCustomerTimeZoneId(financialAccountId);
            }

            resolvedInvoicePayloadObject.FromDate = fromDate;
            resolvedInvoicePayloadObject.ToDate = toDate;
            resolvedInvoicePayloadObject.ToDateForBillingTransaction = toDate.Date.AddDays(1);
            if (resolvedInvoicePayloadObject.TimeZoneId.HasValue)
            {
                VRTimeZone timeZone = new VRTimeZoneManager().GetVRTimeZone(resolvedInvoicePayloadObject.TimeZoneId.Value);
                if (timeZone != null)
                {
                    resolvedInvoicePayloadObject.OffsetValue = timeZone.Settings.Offset;
                    resolvedInvoicePayloadObject.Offset = timeZone.Settings.Offset.ToString();
                    resolvedInvoicePayloadObject.FromDate = resolvedInvoicePayloadObject.FromDate.Add(-timeZone.Settings.Offset);
                    resolvedInvoicePayloadObject.ToDate = resolvedInvoicePayloadObject.ToDate.Add(-timeZone.Settings.Offset);
                    resolvedInvoicePayloadObject.ToDateForBillingTransaction = resolvedInvoicePayloadObject.ToDateForBillingTransaction.Add(-timeZone.Settings.Offset);
                }
            }
            return resolvedInvoicePayloadObject;
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
        private List<T> GetInvoiceMappedRecords<T>(Guid analyticTableId, List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId, TimeSpan? offsetValue, Func<AnalyticRecord, T> mapper) where T : class
        {
            var analyticRecords = GetInvoiceAnalyticRecords(analyticTableId, listDimensions, listMeasures, dimensionFilterName, dimentionFilterValue, fromDate, toDate, currencyId, offsetValue);
            if (analyticRecords != null && analyticRecords.Data != null && analyticRecords.Data.Count() > 0)
            {
                return PrepareItemSetNames<T>(analyticRecords.Data, mapper);
            }
            return default(List<T>);
        }
        private AnalyticSummaryBigResult<AnalyticRecord> GetInvoiceAnalyticRecords(Guid analyticTableId, List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId, TimeSpan? offsetValue)
        {
            Dictionary<string, dynamic> queryParameters = null;
            if (offsetValue.HasValue)
            {
                queryParameters = new Dictionary<string, dynamic>();
                queryParameters.Add("BatchStart_TimeShift", offsetValue.Value);
            }
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
                FilterValues = new List<object> { dimentionFilterValue }
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
                    if(mappedItem != null)
                     itemSetNames.Add(mapper(analyticRecord));
                }
            }
            return itemSetNames;
        }
    }
}
