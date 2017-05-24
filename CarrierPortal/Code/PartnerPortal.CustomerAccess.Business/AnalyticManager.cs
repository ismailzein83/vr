using PartnerPortal.CustomerAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace PartnerPortal.CustomerAccess.Business
{
    public class AnalyticManager
    {
        public AnalyticTileInfo GetAnalyticTileInfo(AnalyticDefinitionSettings analyticDefinitionSettings)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            var accountId = manager.GetRetailAccountId(userId);
           
            Dictionary<Guid, AnalyticTileField> fieldsDic = new Dictionary<Guid, AnalyticTileField>();
            foreach(var query in analyticDefinitionSettings.Queries)
            {
                VRTimePeriodContext context = new VRTimePeriodContext();
                query.TimePeriod.GetTimePeriod(context);
                List<string> measures = query.Measures.Select(x => x.MeasureName).ToList();
                var analyticData = GetFilteredRecords(query.VRConnectionId, query.TableId, measures, query.UserDimensionName, accountId, context.FromTime, context.ToTime);
                AddAnalyticTileFields(analyticData.Data, fieldsDic, query.Measures);
            }
            AnalyticTileInfo analyticTileInfo = new AnalyticTileInfo
            {
                Fields = new List<AnalyticTileField>()
            };
            foreach (var measureId in analyticDefinitionSettings.OrderedMeasureIds)
            {
                AnalyticTileField analyticTileField;
                if (fieldsDic.TryGetValue(measureId, out analyticTileField))
                {
                   analyticTileInfo.Fields.Add(analyticTileField);
                }
            }

            if(analyticDefinitionSettings.ViewId.HasValue)
            {
                 ViewManager viewManager = new ViewManager();
                 var view = viewManager.GetView(analyticDefinitionSettings.ViewId.Value);
                analyticTileInfo.ViewURL = view.Url;
            }
            return analyticTileInfo;
        }
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(Guid connectionId, int tableId, List<string> listMeasures, string dimensionFilterName, object dimensionFilterValue, DateTime fromDate, DateTime toDate)
        {
            Vanrise.Entities.DataRetrievalInput<Vanrise.Analytic.Entities.AnalyticQuery> analyticQuery = new DataRetrievalInput<Vanrise.Analytic.Entities.AnalyticQuery>()
            {
                Query = new Vanrise.Analytic.Entities.AnalyticQuery()
                {
                    MeasureFields = listMeasures,
                    TableId = tableId,
                    FromTime = fromDate,
                    ToTime = toDate,
                    Filters = new List<DimensionFilter>(),
                    OrderType = AnalyticQueryOrderType.ByAllMeasures
                },
            };
            DimensionFilter dimensionFilter = new DimensionFilter()
            {
                Dimension = dimensionFilterName,
                FilterValues = new List<object> { dimensionFilterValue }
            };
            analyticQuery.Query.Filters.Add(dimensionFilter);

            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Post<Vanrise.Entities.DataRetrievalInput<Vanrise.Analytic.Entities.AnalyticQuery>, AnalyticSummaryBigResult<AnalyticRecord>>(string.Format("/api/VR_Analytic/Analytic/GetFilteredRecords"), analyticQuery);
        }

        private void AddAnalyticTileFields(IEnumerable<AnalyticRecord> analyticRecords, Dictionary<Guid,AnalyticTileField> fieldsDic, List<MeasureItem> listMeasures)
        {
            if (analyticRecords != null)
            {
                foreach (var analyticRecord in analyticRecords)
                {
                    foreach (var listMeasure in listMeasures)
                    {
                        MeasureValue measureValue = GetMeasureValue(analyticRecord, listMeasure.MeasureName);
                        if (measureValue != null)
                        {
                            fieldsDic.Add(listMeasure.MeasureItemId,new AnalyticTileField
                            {
                                Description = listMeasure.MeasureTitle,
                                Value = measureValue.Value
                            });
                        }
                    }
                }
            }
        }
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }
    }
}
