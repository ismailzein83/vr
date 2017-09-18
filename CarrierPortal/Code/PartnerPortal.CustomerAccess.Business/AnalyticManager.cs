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
using Vanrise.Common;
namespace PartnerPortal.CustomerAccess.Business
{
    public class AnalyticManager
    {
        public AnalyticTileInfo GetAnalyticTileInfo(AnalyticDefinitionSettings analyticDefinitionSettings)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            RetailAccountUserManager manager = new RetailAccountUserManager();
            var accountInfo = manager.GetRetailAccountInfo(userId);
            accountInfo.ThrowIfNull("accountInfo", userId);
            var accountId = accountInfo.AccountId;
           
            Dictionary<Guid, AnalyticTileField> fieldsDic = new Dictionary<Guid, AnalyticTileField>();
            VRTimePeriodManager vrTimeManager = new VRTimePeriodManager();

            foreach(var query in analyticDefinitionSettings.Queries)
            {
                DateTimeRange range = vrTimeManager.GetTimePeriod(query.TimePeriod);
                List<string> measures = query.Measures.Select(x => x.MeasureName).ToList();
                var analyticData = GetFilteredRecords(query.VRConnectionId, query.TableId, measures, query.UserDimensionName, accountId, range.From, range.To);
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
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(Guid connectionId, Guid tableId, List<string> listMeasures, string dimensionFilterName, object dimensionFilterValue, DateTime fromDate, DateTime toDate)
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
            if (analyticRecords != null && analyticRecords.Count() > 0)
            {
                foreach (var analyticRecord in analyticRecords)
                {
                    AddMeasureToDic(fieldsDic, listMeasures, analyticRecord);
                }
            }else
            {
                AddMeasureToDic(fieldsDic, listMeasures, null);
            }
        }
        private void AddMeasureToDic(Dictionary<Guid, AnalyticTileField> fieldsDic, List<MeasureItem> listMeasures, AnalyticRecord analyticRecord)
        {
            foreach (var listMeasure in listMeasures)
            {
                MeasureValue measureValue = null;
                if (analyticRecord != null)
                    measureValue = GetMeasureValue(analyticRecord, listMeasure.MeasureName);
                fieldsDic.Add(listMeasure.MeasureItemId, new AnalyticTileField
                {
                    Description = listMeasure.MeasureTitle,
                    Value = measureValue != null ? measureValue.Value : 0
                });
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
