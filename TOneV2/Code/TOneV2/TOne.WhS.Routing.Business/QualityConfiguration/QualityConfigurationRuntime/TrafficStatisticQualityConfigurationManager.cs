using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class TrafficStatisticQualityConfigurationManager
    {
        public Dictionary<Guid, InitializedQualityConfiguration> InitializeTrafficStatisticQualityConfigurations(List<RouteRuleQualityConfiguration> routeRuleQualityConfigurations)
        {
            if (routeRuleQualityConfigurations == null || routeRuleQualityConfigurations.Count == 0)
                return null;

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOne.WhS.Routing.Business.QualityConfigurationCacheManager>().GetOrCreateObject("InitializeTrafficStatisticQualityConfigurations",
                () =>
                {
                    Dictionary<Guid, string> routeRuleQualityConfigurationFullTypeById = new Dictionary<Guid, string>();

                    StringBuilder codeBuilder = new StringBuilder(@"
                                using System;
                                using System.Linq;
        
                                namespace #NAMESPACE#
                                {
                                    #CLASSESCODE#
                                }");

                    string classesNamespace = CSharpCompiler.GenerateUniqueNamespace("TOne.WhS.Routing.Business");
                    codeBuilder.Replace("#NAMESPACE#", classesNamespace);

                    StringBuilder classesCodeBuilder = new StringBuilder();

                    foreach (var routeRuleQualityConfiguration in routeRuleQualityConfigurations)
                    {
                        routeRuleQualityConfiguration.ThrowIfNull("routeRuleQualityConfiguration");
                        var trafficStatisticQualityConfigurationSettings = routeRuleQualityConfiguration.Settings.CastWithValidate<TrafficStatisticQualityConfigurationSettings>("routeRuleQualityConfiguration.Settings");

                        if (String.IsNullOrEmpty(trafficStatisticQualityConfigurationSettings.Expression))
                            throw new NullReferenceException(String.Format("routeRuleQualityConfigurationExpression '{0}'", routeRuleQualityConfiguration.Name));

                        StringBuilder classDefinitionBuilder = new StringBuilder(@"                 
                                    public class #CLASSNAME# : TOne.WhS.Routing.Entities.IRouteRuleQualityConfigurationEvaluator
                                    {
                                        public decimal GetQualityValue(TOne.WhS.Routing.Entities.IRouteRuleGetQualityValueContext context)
                                        {
                                            return #EXECUTIONCODE#;
                                        }
                                    }");

                        string className = string.Format("RouteRuleQualityConfigurationEvaluator_{0}", Guid.NewGuid().ToString("N"));
                        classDefinitionBuilder.Replace("#CLASSNAME#", className);
                        classDefinitionBuilder.Replace("#EXECUTIONCODE#", trafficStatisticQualityConfigurationSettings.Expression);

                        classesCodeBuilder.AppendLine(classDefinitionBuilder.ToString());

                        string fullTypeName = String.Format("{0}.{1}", classesNamespace, className);
                        routeRuleQualityConfigurationFullTypeById.Add(routeRuleQualityConfiguration.QualityConfigurationId, fullTypeName);
                    }

                    codeBuilder.Replace("#CLASSESCODE#", classesCodeBuilder.ToString());

                    CSharpCompilationOutput compilationOutput;
                    if (!CSharpCompiler.TryCompileClass(String.Format("RouteRuleQualityConfigurationEvaluators_{0}", Guid.NewGuid().ToString("N")), codeBuilder.ToString(), out compilationOutput))
                    {
                        StringBuilder errorsBuilder = new StringBuilder();
                        if (compilationOutput.ErrorMessages != null)
                        {
                            foreach (var errorMessage in compilationOutput.ErrorMessages)
                                errorsBuilder.AppendLine(errorMessage);
                        }

                        throw new Exception(String.Format("Compile Error when building Route Rule Quality Configuration Evaluators. Errors: {0}", errorsBuilder));
                    }

                    Dictionary<Guid, InitializedQualityConfiguration> results = new Dictionary<Guid, InitializedQualityConfiguration>();

                    foreach (var routeRuleQualityConfiguration in routeRuleQualityConfigurations)
                    {
                        var routeRuleQualityConfigurationFullType = routeRuleQualityConfigurationFullTypeById[routeRuleQualityConfiguration.QualityConfigurationId];
                        var runtimeType = compilationOutput.OutputAssembly.GetType(routeRuleQualityConfigurationFullType);
                        if (runtimeType == null)
                            throw new NullReferenceException("runtimeType");

                        IRouteRuleQualityConfigurationEvaluator evaluator = Activator.CreateInstance(runtimeType) as IRouteRuleQualityConfigurationEvaluator;
                        evaluator.ThrowIfNull(string.Format("routeRuleQualityConfigurationEvaluator '{0}'", routeRuleQualityConfiguration.Name));

                        if (!results.ContainsKey(routeRuleQualityConfiguration.QualityConfigurationId))
                            results.Add(routeRuleQualityConfiguration.QualityConfigurationId, new InitializedTrafficStatisticQualityConfiguration() { Evaluator = evaluator });
                    }

                    return results;
                });
        }

        public List<string> GetExpressionMeasureFields(string expression, string qualityConfigurationName, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrEmpty(expression))
                return null;

            const int getMeasureValueLength = 25; //representing length of context.getMeasureValue("

            HashSet<string> measureFields = new HashSet<string>();

            while (expression.Length > 0)
            {
                int getMeasureValueIndex = expression.IndexOf("context.GetMeasureValue");
                if (getMeasureValueIndex == -1)
                    break;

                expression = expression.Remove(0, getMeasureValueIndex + getMeasureValueLength);
                int quotationMarkIndex = expression.IndexOf("\"");
                if (quotationMarkIndex == -1)
                {
                    errorMessage = string.Format("Quality Configuration '{0}': Expression syntax is incorrect", qualityConfigurationName);
                    return null;
                }

                string measureField = expression.Substring(0, quotationMarkIndex);
                measureFields.Add(measureField);
            }

            return measureFields.Count > 0 ? measureFields.ToList() : null;
        }

        public List<QualityAnalyticRecord> GetQualityAnalyticRecords(IRouteRuleQualityConfigurationEvaluator evalutor, VRTimePeriod vrTimePeriod, Guid analyticTableId,
            List<string> dimensionFields, List<string> measureFields, DateTime effectiveDate)
        {
            vrTimePeriod.ThrowIfNull("routeRuleQualityConfigurationData.Entity");

            VRTimePeriodContext vrTimePeriodContext = new VRTimePeriodContext() { EffectiveDate = effectiveDate };
            vrTimePeriod.GetTimePeriod(vrTimePeriodContext);

            AnalyticQuery analyticQuery = new AnalyticQuery();
            analyticQuery.TableId = analyticTableId;
            analyticQuery.DimensionFields = dimensionFields;
            analyticQuery.MeasureFields = measureFields;
            analyticQuery.FromTime = vrTimePeriodContext.FromTime;
            analyticQuery.ToTime = vrTimePeriodContext.ToTime;

            AnalyticRecord analyticRecordSummary;
            List<AnalyticRecord> analyticRecords = new AnalyticManager().GetAllFilteredRecords(analyticQuery, out analyticRecordSummary);
            if (analyticRecords == null || analyticRecords.Count == 0)
                return null;

            List<QualityAnalyticRecord> qualityAnalyticRecordList = new List<QualityAnalyticRecord>();

            foreach (AnalyticRecord analyticRecord in analyticRecords)
            {
                RouteRuleGetQualityValueContext context = new RouteRuleGetQualityValueContext(analyticRecord);

                qualityAnalyticRecordList.Add(new QualityAnalyticRecord()
                {
                    AnalyticRecord = analyticRecord,
                    Quality = evalutor.GetQualityValue(context)
                });
            }

            return qualityAnalyticRecordList;
        }

        public List<AnalyticMeasureInfo> GetTrafficStatisticQualityConfigurationMeasures(Guid qualityConfigurationDefinitionId)
        {
            QualityConfigurationDefinitionManager qualityConfigurationDefinitionManager = new QualityConfigurationDefinitionManager();
            QualityConfigurationDefinitionExtendedSettings extendedSettings = qualityConfigurationDefinitionManager.GetQualityConfigurationDefinitionExtendedSettings(qualityConfigurationDefinitionId);
            var trafficStatisticQCDefinitionSettings = extendedSettings.CastWithValidate<TrafficStatisticQCDefinitionSettings>("qualityConfigurationDefinitionExtendedSettings", qualityConfigurationDefinitionId);
            List<string> includedMeasures = trafficStatisticQCDefinitionSettings.IncludedMeasures;
            if (includedMeasures == null || includedMeasures.Count == 0)
                throw new Exception("trafficStatisticQCDefinitionSettings.IncludedMeasures should contains at least one measure.");

            Dictionary<string, AnalyticMeasure> analyticItemConfigs = new AnalyticItemConfigManager().GetMeasures(trafficStatisticQCDefinitionSettings.AnalyticTableId);
            List<AnalyticMeasureInfo> analyticMeasureInfos = new List<AnalyticMeasureInfo>();

            if (analyticItemConfigs != null)
            {
                AnalyticMeasure analyticMeasure;
                foreach (var measureName in includedMeasures)
                {
                    if (analyticItemConfigs.TryGetValue(measureName, out analyticMeasure))
                        analyticMeasureInfos.Add(new AnalyticMeasureInfo { Name = measureName, Title = analyticMeasure.Title });

                }
            }

            return analyticMeasureInfos;
        }
    }
}