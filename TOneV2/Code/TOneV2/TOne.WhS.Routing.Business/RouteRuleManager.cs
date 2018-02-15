using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RouteRuleManager : Vanrise.Rules.RuleManager<RouteRule, RouteRuleDetail>
    {
        #region Public Methods

        public IEnumerable<RouteRuleCriteriaConfig> GetRouteRuleCriteriaTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<RouteRuleCriteriaConfig>(RouteRuleCriteriaConfig.EXTENSION_TYPE);
        }

        public IEnumerable<RouteRule> GetEffectiveLinkedRouteRules(int customerId, string code, DateTime effectiveDate)
        {
            RouteRuleIdentifier routeRuleIdentifier = new RouteRuleIdentifier() { Code = code, CustomerId = customerId };
            var linkedRules = GetCachedLinkedRouteRules();
            if (linkedRules == null)
                return null;

            List<RouteRule> linkedRouteRules = linkedRules.GetRecord(routeRuleIdentifier);
            if (linkedRouteRules == null)
                return null;
            return linkedRouteRules.FindAllRecords(itm => itm.IsEffectiveOrFuture(effectiveDate));
        }

        public RouteRule GetRouteRuleHistoryDetailbyHistoryId(int routeRuleHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var routeRule = s_vrObjectTrackingManager.GetObjectDetailById(routeRuleHistoryId);
            return routeRule.CastWithValidate<RouteRule>("RouteRule : historyId ", routeRuleHistoryId);
        }

        public RouteRule BuildLinkedRouteRule(int ruleId, int? customerId, string code, long? saleZoneId, List<RouteOption> routeOptions)
        {
            RouteRule relatedRouteRule = base.GetRuleWithDeleted(ruleId);
            if (relatedRouteRule == null)
                throw new NullReferenceException(string.Format("relatedRouteRule. RuleId: {0}", ruleId));

            if (relatedRouteRule.Settings == null)
                throw new NullReferenceException(string.Format("relatedRouteRule.Settings. RuleId: {0}", ruleId));

            LinkedRouteRuleContext context = new LinkedRouteRuleContext() { RouteOptions = routeOptions };
            DateTime now = DateTime.Now;

            RouteRuleCriteria routeRuleCriteria = new RouteRuleCriteria();

            if (customerId.HasValue && customerId.Value > 0)
                routeRuleCriteria.CustomerGroupSettings = new SelectiveCustomerGroup() { CustomerIds = new List<int>() { customerId.Value } };

            if (string.IsNullOrEmpty(code))
                throw new NullReferenceException("code must have a value");

            if (saleZoneId.HasValue)
            {
                SaleZone saleZone = new SaleZoneManager().GetSaleZone(saleZoneId.Value);
                routeRuleCriteria.SaleZoneGroupSettings = new SelectiveSaleZoneGroup() { SellingNumberPlanId = saleZone.SellingNumberPlanId, ZoneIds = new List<long>() { saleZoneId.Value } };
            }
            else
            {
                CodeCriteria codeCriteria = new BusinessEntity.Entities.CodeCriteria() { Code = code };
                routeRuleCriteria.CodeCriteriaGroupSettings = new SelectiveCodeCriteriaGroup() { Codes = new List<CodeCriteria>() { codeCriteria } };
            }

            RouteRule linkedRouteRule = new RouteRule()
            {
                BeginEffectiveTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second),
                Settings = relatedRouteRule.Settings.BuildLinkedRouteRuleSettings(context),
                Criteria = routeRuleCriteria
            };

            return linkedRouteRule;
        }

        public List<RouteRule> GetEffectiveAndFutureRPRouteRules(DateTime effectiveDate)
        {
            var routeRules = base.GetAllRules();
            List<RouteRule> rpRouteRules = new List<RouteRule>();

            foreach (var routeRuleKvp in routeRules)
            {
                RouteRule routeRule = routeRuleKvp.Value;

                if (routeRule.IsEffective(effectiveDate) || (routeRule.BED >= effectiveDate && (!routeRule.EED.HasValue || routeRule.EED.Value != routeRule.BED)))
                {
                    int? routingProductId = routeRule.Criteria.GetRoutingProductId();
                    if (routingProductId.HasValue)
                        rpRouteRules.Add(routeRule);
                }
            }
            return rpRouteRules;
        }

        public List<RouteRule> GetEffectiveAndFutureCustomerRouteRules(DateTime effectiveDate)
        {
            List<RouteRule> customerRouteRules = new List<RouteRule>();
            var routeRules = base.GetAllRules();

            foreach (var routeRuleKvp in routeRules)
            {
                RouteRule routeRule = routeRuleKvp.Value;
                if (routeRule.IsEffective(effectiveDate) || (routeRule.BED >= effectiveDate && (!routeRule.EED.HasValue || routeRule.EED.Value != routeRule.BED)))
                    customerRouteRules.Add(routeRule);
            }

            return customerRouteRules;
        }

        public Vanrise.Entities.IDataRetrievalResult<RouteRuleDetail> GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<RouteRuleQuery> input)
        {
            var routeRules = base.GetAllRules();
            string ruleNameLower = !string.IsNullOrEmpty(input.Query.Name) ? input.Query.Name.ToLower() : null;
            Func<RouteRule, bool> filterExpression = (routeRule) =>
                {
                    if (input.Query.IsManagementScreen && !routeRule.Criteria.IsVisibleInManagementView())
                        return false;

                    int? routingProductId = routeRule.Criteria.GetRoutingProductId();
                    if (!input.Query.RoutingProductId.HasValue && routingProductId.HasValue)
                        return false;

                    if (input.Query.RoutingProductId.HasValue && (!routingProductId.HasValue || routingProductId.Value != input.Query.RoutingProductId.Value))
                        return false;

                    if (!string.IsNullOrEmpty(ruleNameLower) && (string.IsNullOrEmpty(routeRule.Name) || !routeRule.Name.ToLower().Contains(ruleNameLower)))
                        return false;

                    if (!string.IsNullOrEmpty(input.Query.Code) && !CheckIfCodeCriteriaSettingsContains(routeRule, input.Query.Code))
                        return false;

                    if (input.Query.CustomerIds != null && !CheckIfCustomerSettingsContains(routeRule, input.Query.CustomerIds))
                        return false;

                    if (input.Query.SaleZoneIds != null && !CheckIfSaleZoneSettingsContains(routeRule, input.Query.SaleZoneIds))
                        return false;

                    if (input.Query.RouteRuleSettingsConfigIds != null && !CheckIfSameRouteRuleSettingsConfigId(routeRule, input.Query.RouteRuleSettingsConfigIds))
                        return false;

                    if (input.Query.EffectiveOn.HasValue && (routeRule.BeginEffectiveTime > input.Query.EffectiveOn || (routeRule.EndEffectiveTime.HasValue && routeRule.EndEffectiveTime <= input.Query.EffectiveOn)))
                        return false;

                    if (input.Query.LinkedRouteRuleIds != null && !input.Query.LinkedRouteRuleIds.Contains(routeRule.RuleId))
                        return false;

                    if (input.Query.Filters != null)
                    {
                        RouteRuleFilterContext context = new RouteRuleFilterContext() { RouteRule = routeRule };
                        foreach (IRouteRuleFilter filter in input.Query.Filters)
                        {
                            if (!filter.IsMatched(context))
                                return false;
                        }
                    }

                    return true;
                };


            ResultProcessingHandler<RouteRuleDetail> handler = new ResultProcessingHandler<RouteRuleDetail>()
            {
                ExportExcelHandler = new RouteRuleExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(RouteRuleLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, routeRules.ToBigResult(input, filterExpression, MapToDetails), handler);
        }

        public RouteRule GetMatchRule(RouteRuleTarget target, int? routingProductId)
        {
            var ruleTrees = GetRuleTreesByPriority(routingProductId);
            if (ruleTrees != null)
            {
                foreach (var ruleTree in ruleTrees)
                {
                    var matchRule = ruleTree.GetMatchRule(target) as RouteRule;
                    if (matchRule != null)
                        return matchRule;
                }
            }
            return null;
        }

        public Vanrise.Rules.RuleTree[] GetRuleTreesByPriority(int? routingProductId)
        {
            return GetCachedOrCreate(string.Format("GetRuleTreesByPriority_{0}", routingProductId.HasValue ? routingProductId.Value : 0),
                () =>
                {
                    List<Vanrise.Rules.RuleTree> ruleTrees = new List<Vanrise.Rules.RuleTree>();
                    var structureBehaviors = GetRuleStructureBehaviors();
                    var routeRuleTypes = GetRouteRuleTypesTemplates();

                    int? currentPriority = null;
                    List<Vanrise.Rules.IVRRule> currentRules = null;
                    foreach (var ruleType in routeRuleTypes.OrderBy(itm => GetRuleTypePriority(itm)))
                    {
                        int priority = GetRuleTypePriority(ruleType);
                        if (currentPriority == null || currentPriority.Value != priority)
                        {
                            if (currentRules != null && currentRules.Count > 0)
                                ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                            currentPriority = priority;
                            currentRules = new List<Vanrise.Rules.IVRRule>();
                        }
                        var ruleTypeRules = GetFilteredRules(itm => itm.Settings.ConfigId == ruleType.ExtensionConfigurationId
                                                         && (!routingProductId.HasValue || (itm.Criteria.GetRoutingProductId().HasValue && routingProductId.Value == itm.Criteria.GetRoutingProductId().Value)));
                        if (ruleTypeRules != null)
                            currentRules.AddRange(ruleTypeRules);
                    }
                    if (currentRules != null && currentRules.Count > 0)
                        ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                    return ruleTrees.ToArray();
                });
        }

        public IEnumerable<RouteRuleSettingsConfig> GetRouteRuleTypesTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<RouteRuleSettingsConfig>(RouteRuleSettingsConfig.EXTENSION_TYPE);
        }

        public override RouteRuleDetail MapToDetails(RouteRule rule)
        {
            string _cssClass = null;
            string _routeRuleSettingsTypeName = null;
            Dictionary<Guid, RouteRuleSettingsConfig> routeRuleSettingsConfigDic = GetRouteRuleTypesTemplatesDict();

            if (rule != null && rule.Settings != null && rule.Settings.ConfigId != null)
            {
                RouteRuleSettingsConfig routeRuleSettingsConfig = routeRuleSettingsConfigDic.GetRecord(rule.Settings.ConfigId);
                _cssClass = routeRuleSettingsConfig.CssClass;
                _routeRuleSettingsTypeName = routeRuleSettingsConfig.Title;
            }

            return new RouteRuleDetail()
            {
                Entity = rule,
                CssClass = _cssClass,
                RouteRuleSettingsTypeName = _routeRuleSettingsTypeName
            };
        }

        public Dictionary<Guid, RouteRuleSettingsConfig> GetRouteRuleTypesTemplatesDict()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurationsByType<RouteRuleSettingsConfig>(RouteRuleSettingsConfig.EXTENSION_TYPE);
        }

        public override Vanrise.Rules.RuleLoggableEntity GetLoggableEntity(RouteRule rule)
        {
            return RouteRuleLoggableEntity.Instance;
        }


        public bool HasSelectiveCustomerCriteria(BaseRouteRuleCriteria criteria)
        {
            CustomerGroupSettings customerGroupSettings = criteria.GetCustomerGroupSettings();
            if (customerGroupSettings == null)
                return false;

            SelectiveCustomerGroup selectiveCustomerGroup = customerGroupSettings as SelectiveCustomerGroup;
            if (selectiveCustomerGroup == null)
                return false;

            return selectiveCustomerGroup.CustomerIds != null && selectiveCustomerGroup.CustomerIds.Count > 0;
        }

        public bool HasSelectiveCodeCriteria(BaseRouteRuleCriteria criteria)
        {
            CodeCriteriaGroupSettings codeCriteriaGroupSettings = criteria.GetCodeCriteriaGroupSettings();
            if (codeCriteriaGroupSettings == null)
                return false;

            SelectiveCodeCriteriaGroup selectiveCodeCriteriaGroup = codeCriteriaGroupSettings as SelectiveCodeCriteriaGroup;
            if (selectiveCodeCriteriaGroup == null)
                return false;

            return selectiveCodeCriteriaGroup.Codes != null && selectiveCodeCriteriaGroup.Codes.Count > 0;
        }

        public bool HasSelectiveSaleZoneCriteria(BaseRouteRuleCriteria criteria)
        {
            SaleZoneGroupSettings saleZoneGroupSettings = criteria.GetSaleZoneGroupSettings();
            if (saleZoneGroupSettings == null)
                return false;

            SelectiveSaleZoneGroup selectiveSaleZoneGroup = saleZoneGroupSettings as SelectiveSaleZoneGroup;
            if (selectiveSaleZoneGroup == null)
                return false;

            return selectiveSaleZoneGroup.ZoneIds != null && selectiveSaleZoneGroup.ZoneIds.Count > 0;
        }
        #endregion

        #region Private Methods

        private IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCode());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySaleZone());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCustomer());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByRoutingProduct());
            return ruleStructureBehaviors;
        }

        private int GetRuleTypePriority(RouteRuleSettingsConfig ruleTypeConfig)
        {
            return ruleTypeConfig.Priority.HasValue ? ruleTypeConfig.Priority.Value : int.MaxValue;
        }

        private Dictionary<RouteRuleIdentifier, List<RouteRule>> GetCachedLinkedRouteRules()
        {
            return GetCachedOrCreate(string.Format("GetCachedLinkedRouteRules"),
                () =>
                {
                    Dictionary<RouteRuleIdentifier, List<RouteRule>> linkedRouteRules = new Dictionary<RouteRuleIdentifier, List<RouteRule>>();
                    Dictionary<int, RouteRule> routeRules = GetAllRules();

                    if (routeRules != null)
                    {
                        foreach (var routeRule in routeRules)
                        {
                            if (routeRule.Value.Criteria != null)
                            {
                                RouteRuleCriteria criteria = routeRule.Value.Criteria as RouteRuleCriteria;

                                if (criteria == null)
                                    continue;

                                string code = CheckAndReturnValidCode(criteria, routeRule.Value.RuleId);
                                if (string.IsNullOrEmpty(code))
                                    continue;

                                int? customerId = CheckAndReturnValidCustomer(criteria);
                                if (!customerId.HasValue)
                                    continue;

                                RouteRuleIdentifier routeRuleIdentifier = new RouteRuleIdentifier() { Code = code, CustomerId = customerId.Value };
                                List<RouteRule> relatedRouteRules = linkedRouteRules.GetOrCreateItem(routeRuleIdentifier);
                                relatedRouteRules.Add(routeRule.Value);
                            }
                        }
                    }

                    return linkedRouteRules;
                });
        }

        int? CheckAndReturnValidCustomer(BaseRouteRuleCriteria criteria)
        {
            CustomerGroupSettings customerGroupSettings = criteria.GetCustomerGroupSettings();
            if (customerGroupSettings == null)
                return null;

            SelectiveCustomerGroup selectiveCustomerGroup = customerGroupSettings as SelectiveCustomerGroup;
            if (selectiveCustomerGroup == null)
                return null;

            if (selectiveCustomerGroup.CustomerIds == null || selectiveCustomerGroup.CustomerIds.Count != 1)
                return null;

            return selectiveCustomerGroup.CustomerIds.First();
        }

        string CheckAndReturnValidCode(BaseRouteRuleCriteria criteria, int ruleId)
        {
            CodeCriteriaGroupSettings codeCriteriaGroupSettings = criteria.GetCodeCriteriaGroupSettings();
            if (codeCriteriaGroupSettings == null)
                return null;

            SelectiveCodeCriteriaGroup selectiveCodeCriteriaGroup = codeCriteriaGroupSettings as SelectiveCodeCriteriaGroup;
            if (selectiveCodeCriteriaGroup == null)
                return null;

            if (selectiveCodeCriteriaGroup.Codes == null || selectiveCodeCriteriaGroup.Codes.Count != 1)
                return null;

            string code = selectiveCodeCriteriaGroup.Codes.First().Code;

            RoutingExcludedDestinations routingExcludedDestinations = criteria.GetExcludedDestinations();
            if (routingExcludedDestinations != null)
            {
                RoutingExcludedDestinationContext context = new RoutingExcludedDestinationContext(code, ruleId);
                if (routingExcludedDestinations.IsExcludedDestination(context))
                    return null;
            }

            return code;
        }

        private bool CheckIfCodeCriteriaSettingsContains(RouteRule routeRule, string code)
        {
            CodeCriteriaGroupSettings codeCriteriaGroupSettings = routeRule.Criteria.GetCodeCriteriaGroupSettings();
            if (codeCriteriaGroupSettings != null)
            {
                IRuleCodeCriteria ruleCode = routeRule as IRuleCodeCriteria;
                if (ruleCode.CodeCriterias != null && ruleCode.CodeCriterias.Any(x => x.Code.StartsWith(code)))
                    return true;
            }

            return false;
        }

        private bool CheckIfCustomerSettingsContains(RouteRule routeRule, IEnumerable<int> customerIds)
        {
            CustomerGroupSettings customerGroupSettings = routeRule.Criteria.GetCustomerGroupSettings();
            if (customerGroupSettings != null)
            {
                IRuleCustomerCriteria ruleCode = routeRule as IRuleCustomerCriteria;
                if (ruleCode.CustomerIds == null || ruleCode.CustomerIds.Intersect(customerIds).Count() > 0)
                    return true;
            }

            return false;
        }

        private bool CheckIfSaleZoneSettingsContains(RouteRule routeRule, IEnumerable<long> saleZoneIds)
        {
            SaleZoneGroupSettings saleZoneGroupSettings = routeRule.Criteria.GetSaleZoneGroupSettings();
            if (saleZoneGroupSettings != null)
            {
                IRuleSaleZoneCriteria ruleCode = routeRule as IRuleSaleZoneCriteria;
                if (ruleCode.SaleZoneIds != null && ruleCode.SaleZoneIds.Intersect(saleZoneIds).Count() > 0)
                    return true;
            }

            return false;
        }

        private bool CheckIfSameRouteRuleSettingsConfigId(RouteRule routeRule, List<Guid> RouteRuleSettingsConfigIds)
        {
            if (RouteRuleSettingsConfigIds.Contains(routeRule.Settings.ConfigId))
                return true;

            return false;
        }

        #endregion

        #region Private Classes

        private class RouteRuleExcelExportHandler : ExcelExportHandler<RouteRuleDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RouteRuleDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Routes",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Included Codes" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Customers", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Sale Zones" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rule Type" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "BED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "EED", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell { Value = record.IncludedCodes });
                            row.Cells.Add(new ExportExcelCell { Value = record.Customers });
                            row.Cells.Add(new ExportExcelCell { Value = record.SaleZones });
                            row.Cells.Add(new ExportExcelCell { Value = record.RouteRuleSettingsTypeName });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.BED });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.EED });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class RouteRuleLoggableEntity : Vanrise.Rules.RuleLoggableEntity
        {
            static RouteRuleLoggableEntity s_instance = new RouteRuleLoggableEntity();
            public static RouteRuleLoggableEntity Instance
            {
                get
                {
                    return s_instance;
                }
            }
            public override string EntityDisplayName
            {
                get { return "Route Rules"; }
            }

            public override string EntityUniqueName
            {
                get { return "WhS_Routing_RouteRules"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_Routing_RouteRules_ViewHistoryItem"; }
            }
        }


        #endregion
    }
}
