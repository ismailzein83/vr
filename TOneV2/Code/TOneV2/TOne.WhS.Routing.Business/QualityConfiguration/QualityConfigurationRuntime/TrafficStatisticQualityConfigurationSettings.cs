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
    public class TrafficStatisticQualityConfigurationSettings : RouteRuleQualityConfigurationSettings
    {
        public override Guid ConfigId { get { return new Guid("11E33FEA-1553-46BA-A09C-D5F8AA50F7E1"); } }

        public VRTimePeriod TimePeriod { get; set; }

        public string Expression { get; set; }

        public override List<RouteRuleQualityConfigurationData> GetRouteRuleQualityConfigurationData(IGetRouteRuleQualityConfigurationDataContext context)
        {
            QualityConfigurationDefinitionManager qualityConfigurationDefinitionManager = new QualityConfigurationDefinitionManager();
            var qualityConfigurationDefinitionExtendedSettings = qualityConfigurationDefinitionManager.GetQualityConfigurationDefinitionExtendedSettings(context.QualityConfigurationDefinitionId);
            var trafficStatisticQCDefinitionSettings = qualityConfigurationDefinitionExtendedSettings.CastWithValidate<TrafficStatisticQCDefinitionSettings>("qualityConfigurationDefinitionExtendedSettings");

            Guid analyticTableId = trafficStatisticQCDefinitionSettings.AnalyticTableId;
            List<string> dimensionFields = null;

            switch (context.RoutingProcessType)
            {
                case RoutingProcessType.CustomerRoute:
                    dimensionFields = new List<string>() { trafficStatisticQCDefinitionSettings.SupplierZoneFieldName }; break;

                case RoutingProcessType.RoutingProductRoute:
                    dimensionFields = new List<string>() { trafficStatisticQCDefinitionSettings.SaleZoneFieldName, trafficStatisticQCDefinitionSettings.SupplierFieldName }; break;

                default: throw new Exception(string.Format("Unsupported RoutingProcessType: {0}", context.RoutingProcessType));
            }
            
            TrafficStatisticQualityConfigurationManager trafficStatisticQualityConfigurationManager = new TrafficStatisticQualityConfigurationManager();

            string errorMessage;
            List<string> measureFields = trafficStatisticQualityConfigurationManager.GetExpressionMeasureFields(this.Expression, context.QualityConfigurationName, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                throw new VRBusinessException(errorMessage);

            if (measureFields == null || measureFields.Count == 0)
                throw new VRBusinessException(string.Format("Quality Configuration '{0}': Expression doesn't contain any measure.", context.QualityConfigurationName));

            var initializedTrafficStatisticQualityConfiguration = context.InitializedQualityConfiguration.CastWithValidate<InitializedTrafficStatisticQualityConfiguration>("initializedQualityConfiguration");
            IRouteRuleQualityConfigurationEvaluator evalutor = initializedTrafficStatisticQualityConfiguration.Evaluator;

            var qualityAnalyticRecordList = trafficStatisticQualityConfigurationManager.GetQualityAnalyticRecords(evalutor, this.TimePeriod, analyticTableId, dimensionFields, measureFields, context.EffectiveDate);
            if (qualityAnalyticRecordList == null)
                return null;

            List<RouteRuleQualityConfigurationData> routeRuleQualityConfigurationDataList = new List<RouteRuleQualityConfigurationData>();

            switch (context.RoutingProcessType)
            {
                case RoutingProcessType.CustomerRoute:
                    routeRuleQualityConfigurationDataList.AddRange(this.GetCustomerRouteQualityConfigurationData(qualityAnalyticRecordList, context.QualityConfigurationId));
                    break;

                case RoutingProcessType.RoutingProductRoute:
                    routeRuleQualityConfigurationDataList.AddRange(this.GetProductCostQualityConfigurationData(qualityAnalyticRecordList, context.QualityConfigurationId));
                    break;

                default: throw new Exception(string.Format("Unsupported RoutingProcessType: {0}", context.RoutingProcessType));
            }

            return routeRuleQualityConfigurationDataList.Count > 0 ? routeRuleQualityConfigurationDataList : null;
        }

        public override bool ValidateRouteRuleQualityConfigurationSettings(IValidateQualityConfigurationDataContext context)
        {
            TrafficStatisticQualityConfigurationManager trafficStatisticQualityConfigurationManager = new TrafficStatisticQualityConfigurationManager();

            string errorMessage;
            List<string> measureFields = trafficStatisticQualityConfigurationManager.GetExpressionMeasureFields(this.Expression, context.QualityConfigurationName, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                LoggerFactory.GetLogger().WriteError(errorMessage);
                return false;
            }

            if (measureFields == null || measureFields.Count == 0)
            {
                LoggerFactory.GetLogger().WriteError(string.Format("'{0}' Traffic Statistic Quality Configuration Expression doesn't contain any measure.", context.QualityConfigurationName));
                return false;
            }

            List<AnalyticMeasureInfo> analyticMeasureInfo = trafficStatisticQualityConfigurationManager.GetTrafficStatisticQualityConfigurationMeasures(context.QualityConfigurationDefinitionId);
            List<string> undefinedMeasureFieldNames = new List<string>();

            foreach (var measureFieldName in measureFields)
            {
                if (!analyticMeasureInfo.Any(itm => itm.Name == measureFieldName))
                    undefinedMeasureFieldNames.Add(measureFieldName);
            }

            if (undefinedMeasureFieldNames.Count > 0)
            {
                LoggerFactory.GetLogger().WriteError(string.Format("Traffic Statistic table does not contain Measure(s): {0}", string.Join(", ", undefinedMeasureFieldNames)));
                return false;
            }

            StringBuilder codeBuilder = new StringBuilder(@"
                        using System;
                        using System.Linq;

                        namespace #NAMESPACE#
                        {
                            public class #CLASSNAME# : TOne.WhS.Routing.Entities.IRouteRuleQualityConfigurationEvaluator
                            {
                                public decimal GetQualityValue(TOne.WhS.Routing.Entities.IRouteRuleGetQualityValueContext context)
                                {
                                    return #EXECUTIONCODE#;
                                }
                            }
                        }");

            string classesNamespace = CSharpCompiler.GenerateUniqueNamespace("TOne.WhS.Routing.Business");
            codeBuilder.Replace("#NAMESPACE#", classesNamespace);

            string className = string.Format("RouteRuleQualityConfigurationEvaluator_{0}", Guid.NewGuid().ToString("N"));
            codeBuilder.Replace("#CLASSNAME#", className);

            codeBuilder.Replace("#EXECUTIONCODE#", this.Expression);

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(String.Format("RouteRuleQualityConfigurationEvaluators_{0}", Guid.NewGuid().ToString("N")), codeBuilder.ToString(), out compilationOutput))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var currentErrorMessage in compilationOutput.ErrorMessages)
                        errorsBuilder.AppendLine(currentErrorMessage);
                }

                LoggerFactory.GetLogger().WriteError(String.Format("Compile Error when building Traffic Statistic Quality Configuration Evaluator. Errors: {0}", errorsBuilder));
                return false;
            }

            return true;
        }

        private List<RouteRuleQualityConfigurationData> GetCustomerRouteQualityConfigurationData(List<QualityAnalyticRecord> qualityAnalyticRecordList, Guid qualityConfigurationId)
        {
            List<RouteRuleQualityConfigurationData> results = new List<RouteRuleQualityConfigurationData>();

            foreach (var qualityAnalyticRecord in qualityAnalyticRecordList)
            {
                var dimensionValue = qualityAnalyticRecord.AnalyticRecord.DimensionValues.First();
                if (dimensionValue.Value == null)
                    continue;

                long supplierZoneId = (long)dimensionValue.Value;

                results.Add(new CustomerRouteQualityConfigurationData()
                {
                    QualityConfigurationId = qualityConfigurationId,
                    SupplierZoneId = supplierZoneId,
                    QualityData = qualityAnalyticRecord.Quality
                });
            }

            return results;
        }

        private List<RouteRuleQualityConfigurationData> GetProductCostQualityConfigurationData(List<QualityAnalyticRecord> qualityAnalyticRecordList, Guid qualityConfigurationId)
        {
            List<RouteRuleQualityConfigurationData> results = new List<RouteRuleQualityConfigurationData>();

            foreach (var qualityAnalyticRecord in qualityAnalyticRecordList)
            {
                DimensionValue currentSaleZone = qualityAnalyticRecord.AnalyticRecord.DimensionValues[0];
                DimensionValue currentSupplier = qualityAnalyticRecord.AnalyticRecord.DimensionValues[1];

                if (currentSaleZone.Value == null || currentSupplier.Value == null)
                    continue;

                long saleZoneId = (long)currentSaleZone.Value;
                int supplierId = (int)currentSupplier.Value;

                results.Add(new RPQualityConfigurationData()
                {
                    QualityConfigurationId = qualityConfigurationId,
                    SaleZoneId = saleZoneId,
                    SupplierId = supplierId,
                    QualityData = qualityAnalyticRecord.Quality
                });
            }

            return results;
        }
    }
}