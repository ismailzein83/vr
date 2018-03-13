using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Analytic.Entities;

namespace TOne.WhS.Routing.Business
{
    public class TrafficStatisticQualityConfigurationSettings //: RouteRuleQualityConfigurationSettings
    {
        //public override Guid ConfigId { get { throw new NotImplementedException(); } }

        //public VRTimePeriod TimePeriod { get; set; }

        //public string Expression { get; set; }

        //public override List<RouteRuleQualityConfigurationData> GetRouteRuleQualityConfigurationData(IGetRouteRuleQualityConfigurationDataContext context)
        //{
        //    QualityConfigurationDefinitionManager qualityConfigurationDefinitionManager = new QualityConfigurationDefinitionManager();
        //    var qualityConfigurationDefinitionExtendedSettings = qualityConfigurationDefinitionManager.GetQualityConfigurationDefinitionExtendedSettings(context.QualityConfigurationDefinitionId);
        //    var trafficStatisticQCDefinitionSettings = qualityConfigurationDefinitionExtendedSettings.CastWithValidate<TrafficStatisticQCDefinitionSettings>("qualityConfigurationDefinitionExtendedSettings");

        //    Guid analyticTableId = trafficStatisticQCDefinitionSettings.AnalyticTableId; //new ConfigManager().GetQualityAnalyticTableId();
        //    List<string> dimensionFields = null; //new List<string>() { "SupplierZone" };

        //    switch (context.RoutingProcessType)
        //    {
        //        case RoutingProcessType.CustomerRoute:
        //            dimensionFields = trafficStatisticQCDefinitionSettings.CustomerRouteDimensions; break;

        //        case RoutingProcessType.RoutingProductRoute:
        //            dimensionFields = trafficStatisticQCDefinitionSettings.ProductRouteDimensions; break;

        //        default: throw new Exception(string.Format("Unsupported RoutingProcessType: {0}", context.RoutingProcessType));
        //    }

        //    TrafficStatisticQualityConfigurationManager trafficStatisticQualityConfigurationManager = new TrafficStatisticQualityConfigurationManager();
        //    List<string> measureFields = trafficStatisticQualityConfigurationManager.GetExpressionMeasureFields(context.QualityConfigurationId, this.Expression);

        //    var initializedTrafficStatisticQualityConfiguration = context.InitializedQualityConfiguration.CastWithValidate<InitializedTrafficStatisticQualityConfiguration>("initializedQualityConfiguration");
        //    IRouteRuleQualityConfigurationEvaluator evalutor = initializedTrafficStatisticQualityConfiguration.Evaluator;

        //    var qualityAnalyticRecordList = trafficStatisticQualityConfigurationManager.GetQualityAnalyticRecords(evalutor, this.TimePeriod, analyticTableId, dimensionFields, measureFields, context.EffectiveDate);
        //    if (qualityAnalyticRecordList == null)
        //        return null;

        //    List<RouteRuleQualityConfigurationData> routeRuleQualityConfigurationDataList = new List<RouteRuleQualityConfigurationData>();

        //    switch (context.RoutingProcessType)
        //    {
        //        case RoutingProcessType.CustomerRoute:
        //            routeRuleQualityConfigurationDataList.AddRange(this.GetCustomerRouteQualityConfigurationData(qualityAnalyticRecordList, context.QualityConfigurationId));
        //            break;

        //        case RoutingProcessType.RoutingProductRoute:
        //            routeRuleQualityConfigurationDataList.AddRange(this.GetProductCostQualityConfigurationData(qualityAnalyticRecordList, context.QualityConfigurationId));
        //            break;

        //        default: throw new Exception(string.Format("Unsupported RoutingProcessType: {0}", context.RoutingProcessType));
        //    }

        //    return routeRuleQualityConfigurationDataList.Count > 0 ? routeRuleQualityConfigurationDataList : null;
        //}

        //private List<RouteRuleQualityConfigurationData> GetCustomerRouteQualityConfigurationData(List<QualityAnalyticRecord> qualityAnalyticRecordList, Guid qualityConfigurationId)
        //{
        //    List<RouteRuleQualityConfigurationData> results = new List<RouteRuleQualityConfigurationData>();

        //    foreach (var qualityAnalyticRecord in qualityAnalyticRecordList)
        //    {
        //        var dimensionValue = qualityAnalyticRecord.AnalyticRecord.DimensionValues.First();
        //        if (dimensionValue.Value == null)
        //            continue;

        //        long supplierZoneId = (long)dimensionValue.Value;

        //        results.Add(new CustomerRouteQualityConfigurationData()
        //        {
        //            QualityConfigurationId = qualityConfigurationId,
        //            SupplierZoneId = supplierZoneId,
        //            QualityData = qualityAnalyticRecord.Quality
        //        });
        //    }

        //    return results;
        //}

        //private List<RouteRuleQualityConfigurationData> GetProductCostQualityConfigurationData(List<QualityAnalyticRecord> qualityAnalyticRecordList, Guid qualityConfigurationId)
        //{
        //    List<RouteRuleQualityConfigurationData> results = new List<RouteRuleQualityConfigurationData>();

        //    foreach (var qualityAnalyticRecord in qualityAnalyticRecordList)
        //    {
        //        DimensionValue currentSaleZone = qualityAnalyticRecord.AnalyticRecord.DimensionValues[0];
        //        DimensionValue currentSupplier = qualityAnalyticRecord.AnalyticRecord.DimensionValues[1];

        //        if (currentSaleZone.Value == null || currentSupplier.Value == null)
        //            continue;

        //        long saleZoneId = (long)currentSaleZone.Value;
        //        int supplierId = (int)currentSupplier.Value;

        //        results.Add(new RPQualityConfigurationData()
        //        {
        //            QualityConfigurationId = qualityConfigurationId,
        //            SaleZoneId = saleZoneId,
        //            SupplierId = supplierId,
        //            QualityData = qualityAnalyticRecord.Quality
        //        });
        //    }

        //    return results;
        //}
    }
}