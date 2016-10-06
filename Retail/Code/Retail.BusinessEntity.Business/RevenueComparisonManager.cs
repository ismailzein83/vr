using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Common;
namespace Retail.BusinessEntity.Business
{
    public class RevenueComparisonManager
    {
        public Vanrise.Entities.IDataRetrievalResult<RevenueComparisonDetail> GetFilteredRevenueComparisons(Vanrise.Entities.DataRetrievalInput<RevenueComparisonQuery> input)
        {
            OperatorDeclaredInfoManager operatorDeclaredInfoManager = new Business.OperatorDeclaredInfoManager();

            var operatorDeclaredInfo = operatorDeclaredInfoManager.GetOperatorDeclaredInfoForSpecificPeriod(input.Query.FromDate,input.Query.ToDate);
            
            AnalyticManager analyticManager = new AnalyticManager();
            List<string> listDimensions = new List<string> { "EventDirection", "ServiceType" };
            List<string> listMeasures = new List<string> { "TotalAmount", "TotalVolume" };
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
                    MeasureFields = listMeasures,
                    TableId = 6,
                    FromTime = input.Query.FromDate,
                    ToTime = input.Query.ToDate,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                },
                SortByColumnName = "DimensionValues[0].Name"
            };

            var eventsInfo = analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;

            List<RevenueComparison> revenueComparisons = new List<RevenueComparison>();

            foreach (var record in operatorDeclaredInfo)
            {
                foreach (var item in record.Settings.Items)
                {
          

                        Func<AnalyticRecord, bool> filterExpression = (recordObj) =>
                          (recordObj.DimensionValues[0] != null  && recordObj.DimensionValues[0].Value != null  && (OperatorDeclaredInfoTrafficDirection)recordObj.DimensionValues[0].Value == item.TrafficDirection)
                          &&
                          (recordObj.DimensionValues[1] != null && recordObj.DimensionValues[1].Value != null && Guid.Parse(recordObj.DimensionValues[1].Value.ToString()) == item.ServiceTypeId);

                        var eventData = eventsInfo.Data.FirstOrDefault(filterExpression);
                        if (eventData != null)
                        {

                            MeasureValue totalAmount;
                            eventData.MeasureValues.TryGetValue("TotalAmount", out totalAmount);
                            MeasureValue totalVolume;
                            eventData.MeasureValues.TryGetValue("TotalVolume", out totalVolume);


                            revenueComparisons.Add(new RevenueComparison
                            {
                                ServiceTypeID = item.ServiceTypeId,
                                EventDirection = item.TrafficDirection,
                                FromDate = record.Settings.FromDate,
                                ToDate = record.Settings.ToDate,
                                DeclaredAmount = item.Amount,
                                DeclaredVolume = item.Volume,
                                SystemAmount = Convert.ToDecimal(totalAmount.Value ?? 0.0),
                                SystemVolume = Convert.ToDecimal(totalVolume.Value ?? 0.0),
                                DifferenceAmount = Convert.ToDecimal(totalAmount.Value ?? 0.0) - item.Amount,
                                DifferenceVolume = Convert.ToDecimal(totalVolume.Value ?? 0.0) - item.Volume,
                            });
                        }
                }
            }
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, revenueComparisons.ToBigResult(input, null, RevenueComparisonDetailMapper));
        }

        public RevenueComparisonDetail RevenueComparisonDetailMapper(RevenueComparison revenueComparison)
        {
            ServiceTypeManager manager = new ServiceTypeManager();

            return new RevenueComparisonDetail
            {
                Entity = revenueComparison,
                ServiceTypeDescription = manager.GetServiceTypeName(revenueComparison.ServiceTypeID),
                EventDirectionDescription = revenueComparison.EventDirection.HasValue? Utilities.GetEnumDescription(revenueComparison.EventDirection.Value):null
            };
        }
    }
}
