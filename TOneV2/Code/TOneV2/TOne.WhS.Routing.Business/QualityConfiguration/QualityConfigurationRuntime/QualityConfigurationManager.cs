using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class QualityConfigurationManager
    {
        #region public methods

        public Dictionary<Guid, RouteRuleQualityConfiguration> GetRouteRuleQualityConfigurations()
        {
            ConfigManager configManager = new ConfigManager();
            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurations = configManager.GetQualityConfiguration().RouteRuleQualityConfigurationList;
            if (routeRuleQualityConfigurations == null)
                return null;

            return routeRuleQualityConfigurations.ToDictionary(itm => itm.QualityConfigurationId, itm => itm);
        }

        public RouteRuleQualityConfiguration GetDefaultQualityConfiguration()
        {
            Dictionary<Guid, RouteRuleQualityConfiguration> qualityConfigurations = GetRouteRuleQualityConfigurations();
            if (qualityConfigurations != null)
            {
                foreach (var routeRuleQualityConfigurationKvp in qualityConfigurations)
                {
                    var routeRuleQualityConfiguration = routeRuleQualityConfigurationKvp.Value;
                    if (routeRuleQualityConfiguration.IsDefault)
                    {
                        if (!routeRuleQualityConfiguration.IsActive)
                            throw new VRBusinessException("Default quality configuration is not active");

                        return routeRuleQualityConfiguration;
                    }
                }
            }

            throw new VRBusinessException("No default quality configuration is selected");
        }

        public IEnumerable<QualityConfigurationInfo> GetQualityConfigurationInfo(QualityConfigurationInfoFilter filter)
        {
            Dictionary<Guid, RouteRuleQualityConfiguration> qualityConfigurations = GetRouteRuleQualityConfigurations();
            List<QualityConfigurationInfo> qualityConfigurationInfoList = new List<QualityConfigurationInfo>();

            if (qualityConfigurations != null)
            {
                foreach (var qualityConfigurationKvp in qualityConfigurations)
                {
                    RouteRuleQualityConfiguration qualityConfiguration = qualityConfigurationKvp.Value;

                    QualityConfigurationInfo qualityConfigurationInfo = new QualityConfigurationInfo()
                    {
                        Name = qualityConfiguration.Name,
                        QualityConfigurationId = qualityConfiguration.QualityConfigurationId
                    };

                    qualityConfigurationInfoList.Add(qualityConfigurationInfo);
                }
            }
            return qualityConfigurationInfoList;
        }

        public RouteRuleQualityConfiguration GetQualityConfigurationById(Guid qualityConfigurationId)
        {
            Dictionary<Guid, RouteRuleQualityConfiguration> qualityConfigurations = GetRouteRuleQualityConfigurations();
            if (qualityConfigurations == null)
                return null;

            return qualityConfigurations.GetRecord(qualityConfigurationId);
        }

        public List<RouteRuleQualityConfiguration> GetRouteRuleQualityConfigurationList(List<RouteRule> routeRules)
        {
            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurationList = null;
            List<Guid> qualityConfigurationIds = new List<Guid>();
            Guid defaultConfigurationId = GetDefaultQualityConfiguration().QualityConfigurationId;

            foreach (var routeRule in routeRules)
            {
                RouteRuleQualityContext qualityContext = new RouteRuleQualityContext();
                routeRule.Settings.GetQualityConfigurationIds(qualityContext);

                if (qualityContext.IsDefault)
                    qualityConfigurationIds.Add(defaultConfigurationId);

                if (qualityContext.QualityConfigurationIds != null)
                    qualityConfigurationIds.AddRange(qualityContext.QualityConfigurationIds);
            }

            if (qualityConfigurationIds.Count > 0)
            {
                routeRuleQualityConfigurationList = new List<RouteRuleQualityConfiguration>();
                HashSet<Guid> distinctQualityConfigurationIds = qualityConfigurationIds.ToHashSet();

                foreach (var qualityConfigurationId in distinctQualityConfigurationIds)
                {
                    var qualityConfiguration = GetQualityConfigurationById(qualityConfigurationId);
                    if (qualityConfiguration != null)
                        routeRuleQualityConfigurationList.Add(qualityConfiguration);
                }
            }

            return routeRuleQualityConfigurationList;
        }

        public Dictionary<Guid, List<string>> GetQualityConfigurationExpressionsMeasureFields(List<RouteRuleQualityConfiguration> routeRuleQualityConfigurations)
        {
            if (routeRuleQualityConfigurations == null || routeRuleQualityConfigurations.Count == 0)
                return null;

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QualityConfigurationDefinitionCacheManager>().GetOrCreateObject("GetQualityConfigurationExpressionsMeasureFields",
                () =>
                {
                    Dictionary<Guid, List<string>> measureFieldsByQualityConfigurationId = new Dictionary<Guid, List<string>>();

                    foreach (var routeRuleQualityConfiguration in routeRuleQualityConfigurations)
                    {
                        HashSet<string> currentMeasureFields = this.GetQualityConfigurationExpressionMeasureFields(routeRuleQualityConfiguration.Expression);
                        if (currentMeasureFields == null || currentMeasureFields.Count == 0)
                            throw new VRBusinessException("routeRuleQualityConfiguration.Expression doesn't contain any measure.");

                        measureFieldsByQualityConfigurationId.Add(routeRuleQualityConfiguration.QualityConfigurationId, currentMeasureFields.ToList());
                    }

                    return measureFieldsByQualityConfigurationId.Count > 0 ? measureFieldsByQualityConfigurationId : null;
                });
        }

        public List<RouteRuleQualityConfigurationData> GetRouteRuleQualityConfigurationDataList(List<RouteRuleQualityConfiguration> routeRuleQualityConfigurations)
        {
            if (routeRuleQualityConfigurations == null || routeRuleQualityConfigurations.Count == 0)
                return null;

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<QualityConfigurationDefinitionCacheManager>().GetOrCreateObject("GetCachedRouteRuleQualityConfigurationData",
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
                        if (String.IsNullOrEmpty(routeRuleQualityConfiguration.Expression))
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
                        classDefinitionBuilder.Replace("#EXECUTIONCODE#", routeRuleQualityConfiguration.Expression);

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

                    List<RouteRuleQualityConfigurationData> results = new List<RouteRuleQualityConfigurationData>();

                    foreach (var routeRuleQualityConfiguration in routeRuleQualityConfigurations)
                    {
                        var routeRuleQualityConfigurationFullType = routeRuleQualityConfigurationFullTypeById[routeRuleQualityConfiguration.QualityConfigurationId];
                        var runtimeType = compilationOutput.OutputAssembly.GetType(routeRuleQualityConfigurationFullType);
                        if (runtimeType == null)
                            throw new NullReferenceException("runtimeType");

                        IRouteRuleQualityConfigurationEvaluator evaluator = Activator.CreateInstance(runtimeType) as IRouteRuleQualityConfigurationEvaluator;
                        evaluator.ThrowIfNull(string.Format("routeRuleQualityConfigurationEvaluator '{0}'", routeRuleQualityConfiguration.Name));

                        results.Add(new RouteRuleQualityConfigurationData()
                        {
                            Entity = routeRuleQualityConfiguration,
                            Evaluator = evaluator
                        });
                    }

                    return results;
                });
        }

        public List<QualityAnalyticRecord> GetQualityAnalyticRecords(RouteRuleQualityConfigurationData routeRuleQualityConfigurationData, Guid analyticTableId,
            List<string> dimensionFields, List<string> measureFields, DateTime effectiveDate)
        {
            VRTimePeriodContext vrTimePeriodContext = new VRTimePeriodContext() { EffectiveDate = effectiveDate };
            routeRuleQualityConfigurationData.Entity.ThrowIfNull("routeRuleQualityConfigurationData.Entity");
            routeRuleQualityConfigurationData.Entity.TimePeriod.ThrowIfNull("routeRuleQualityConfigurationData.Entity.TimePeriod", routeRuleQualityConfigurationData.Entity.QualityConfigurationId);
            routeRuleQualityConfigurationData.Entity.TimePeriod.GetTimePeriod(vrTimePeriodContext);

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
                    Quality = routeRuleQualityConfigurationData.Evaluator.GetQualityValue(context)
                });
            }

            return qualityAnalyticRecordList;
        }

        public bool TryCompileQualityConfigurationExpression(string qualityConfigurationExpression)
        {
            HashSet<string> currentMeasureFields = GetQualityConfigurationExpressionMeasureFields(qualityConfigurationExpression);
            if (currentMeasureFields == null || currentMeasureFields.Count == 0)
            {
                LoggerFactory.GetLogger().WriteError("RouteRuleQualityConfiguration Expression doesn't contain any measure.");
                return false;
            }

            List<AnalyticMeasureInfo> analyticMeasureInfo = new ConfigManager().GetQualityConfigurationFields();
            List<string> undefinedMeasureFieldNames = new List<string>();

            foreach (var measureFieldName in currentMeasureFields)
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

            codeBuilder.Replace("#EXECUTIONCODE#", qualityConfigurationExpression);

            CSharpCompilationOutput compilationOutput;
            if (!CSharpCompiler.TryCompileClass(String.Format("RouteRuleQualityConfigurationEvaluators_{0}", Guid.NewGuid().ToString("N")), codeBuilder.ToString(), out compilationOutput))
            {
                StringBuilder errorsBuilder = new StringBuilder();
                if (compilationOutput.ErrorMessages != null)
                {
                    foreach (var errorMessage in compilationOutput.ErrorMessages)
                        errorsBuilder.AppendLine(errorMessage);
                }

                LoggerFactory.GetLogger().WriteError(String.Format("Compile Error when building Route Rule Quality Configuration Evaluator. Errors: {0}", errorsBuilder));
                return false;
            }

            return true;
        }

        public Dictionary<long, List<CustomerRouteQualityConfigurationData>> GetCachedCustomerRouteQualityConfigurationData(RoutingDatabase routingDatabase)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<QualityConfigurationDataCacheManager>();

            return cacheManager.GetOrCreateObject("GetCachedCustomerRouteQualityConfigurationData", routingDatabase.ID, QualityConfigurationDataCacheExpirationChecker.Instance,
                () =>
                {
                    ICustomerQualityConfigurationDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerQualityConfigurationDataManager>();
                    dataManager.RoutingDatabase = routingDatabase;

                    IEnumerable<CustomerRouteQualityConfigurationData> customerRouteQualityConfigurationsData = dataManager.GetCustomerRouteQualityConfigurationsData();
                    if (customerRouteQualityConfigurationsData == null)
                        return null;

                    Dictionary<long, List<CustomerRouteQualityConfigurationData>> results = new Dictionary<long, List<CustomerRouteQualityConfigurationData>>();

                    foreach (var itm in customerRouteQualityConfigurationsData)
                    {
                        List<CustomerRouteQualityConfigurationData> supplierZoneQualityConfigurationData = results.GetOrCreateItem(itm.SupplierZoneId);
                        supplierZoneQualityConfigurationData.Add(itm);
                    }

                    return results.Count > 0 ? results : null;
                });
        }

        public Dictionary<SaleZoneSupplier, List<RPQualityConfigurationData>> GetCachedRPQualityConfigurationData(RoutingDatabase routingDatabase)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<QualityConfigurationDataCacheManager>();

            return cacheManager.GetOrCreateObject("GetCachedRPQualityConfigurationData", routingDatabase.ID, QualityConfigurationDataCacheExpirationChecker.Instance,
                () =>
                {
                    IRPQualityConfigurationDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPQualityConfigurationDataManager>();
                    dataManager.RoutingDatabase = routingDatabase;
                    
                    IEnumerable<RPQualityConfigurationData> rpQualityConfigurationsData = dataManager.GetRPQualityConfigurationData();
                    if (rpQualityConfigurationsData == null)
                        return null;

                    Dictionary<SaleZoneSupplier, List<RPQualityConfigurationData>> results = new Dictionary<SaleZoneSupplier, List<RPQualityConfigurationData>>();

                    foreach (var itm in rpQualityConfigurationsData)
                    {
                        SaleZoneSupplier saleZoneSupplier = new SaleZoneSupplier() { SaleZoneId = itm.SaleZoneId, SupplierId = itm.SupplierId };
                        List<RPQualityConfigurationData> saleZoneSupplierQualityConfigurationData = results.GetOrCreateItem(saleZoneSupplier);
                        saleZoneSupplierQualityConfigurationData.Add(itm);
                    }

                    return results.Count > 0 ? results : null;
                });
        }

        #endregion

        #region Private Methods

        private HashSet<string> GetQualityConfigurationExpressionMeasureFields(string expression)
        {
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
                    throw new Exception("Quality configuration expression syntax is incorrect");

                string measureField = expression.Substring(0, quotationMarkIndex);
                measureFields.Add(measureField);
            }

            return measureFields.Count > 0 ? measureFields : null;
        }

        #endregion

        #region Private Classes

        private class QualityConfigurationDefinitionCacheManager : BaseCacheManager
        {
            RouteRuleManager _routeRuleManager = new RouteRuleManager();

            DateTime? _routeRuleCacheLastCheck;
            DateTime? _settingsCacheLastCheck;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _routeRuleManager.IsCacheExpired(ref _routeRuleCacheLastCheck)
                    | Vanrise.Caching.CacheManagerFactory.GetCacheManager<Vanrise.Common.Business.SettingManager.CacheManager>().IsCacheExpired(ref _settingsCacheLastCheck);
            }
        }

        private class QualityConfigurationDataCacheManager : BaseCacheManager<int>
        {

        }

        private class QualityConfigurationDataCacheExpirationChecker : Vanrise.Caching.CacheExpirationChecker
        {
            static QualityConfigurationDataCacheExpirationChecker s_instance = new QualityConfigurationDataCacheExpirationChecker();
            public static QualityConfigurationDataCacheExpirationChecker Instance { get { return s_instance; } }

            public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
            {
                TimeSpan entitiesTimeSpan = TimeSpan.FromMinutes(15);
                SlidingWindowCacheExpirationChecker slidingWindowCacheExpirationChecker = new SlidingWindowCacheExpirationChecker(entitiesTimeSpan);
                return slidingWindowCacheExpirationChecker.IsCacheExpired(context);
            }
        }

        #endregion
    }
}