﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RouteRuleManager : Vanrise.Rules.RuleManager<RouteRule, RouteRuleDetail>
    {
        #region Public Methods

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

        public RouteRule BuildLinkedRouteRule(int ruleId, int? customerId, string code, List<RouteOption> routeOptions)
        {
            RouteRule relatedRouteRule = base.GetRule(ruleId);
            if (relatedRouteRule == null)
                throw new NullReferenceException(string.Format("relatedRouteRule. RuleId: {0}", ruleId));

            if (relatedRouteRule.Settings == null)
                throw new NullReferenceException(string.Format("relatedRouteRule.Settings. RuleId: {0}", ruleId));

            LinkedRouteRuleContext context = new LinkedRouteRuleContext() { RouteOptions = routeOptions };
            DateTime now = DateTime.Now;
            RouteRule linkedRouteRule = new RouteRule()
            {
                BeginEffectiveTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second),
                Settings = relatedRouteRule.Settings.BuildLinkedRouteRuleSettings(context),
                Criteria = new RouteRuleCriteria()
            };

            if (customerId.HasValue && customerId.Value > 0)
                linkedRouteRule.Criteria.CustomerGroupSettings = new SelectiveCustomerGroup() { CustomerIds = new List<int>() { customerId.Value } };


            if (!string.IsNullOrEmpty(code))
            {
                CodeCriteria codeCriteria = new BusinessEntity.Entities.CodeCriteria() { Code = code };
                linkedRouteRule.Criteria.CodeCriteriaGroupSettings = new SelectiveCodeCriteriaGroup() { Codes = new List<CodeCriteria>() { codeCriteria } };
            }

            return linkedRouteRule;
        }

        public Vanrise.Entities.IDataRetrievalResult<RouteRuleDetail> GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<RouteRuleQuery> input)
        {
            var routeRules = base.GetAllRules();
            string ruleNameLower = !string.IsNullOrEmpty(input.Query.Name) ? input.Query.Name.ToLower() : null;
            Func<RouteRule, bool> filterExpression = (routeRule) =>
                {
                    if (!input.Query.RoutingProductId.HasValue && routeRule.Criteria.RoutingProductId.HasValue)
                        return false;

                    if (input.Query.RoutingProductId.HasValue && (!routeRule.Criteria.RoutingProductId.HasValue || routeRule.Criteria.RoutingProductId.Value != input.Query.RoutingProductId.Value))
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

                    return true;
                };

            RouteRuleExcelExportHandler routeRuleExcel = new RouteRuleExcelExportHandler(input.Query);
            ResultProcessingHandler<RouteRuleDetail> handler = new ResultProcessingHandler<RouteRuleDetail>()
            {
                ExportExcelHandler = routeRuleExcel
            };
            VRActionLogger.Current.LogGetFilteredAction(RouteRuleLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, routeRules.ToBigResult(input, filterExpression, MapToDetails), handler);
        }

        private class RouteRuleExcelExportHandler : ExcelExportHandler<RouteRuleDetail>
        {
            private RouteRuleQuery _query;
            public RouteRuleExcelExportHandler(RouteRuleQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RouteRuleDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet();
                sheet.Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Included Codes" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Customers" });
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
                                                         && (!routingProductId.HasValue || (itm.Criteria.RoutingProductId.HasValue && routingProductId.Value == itm.Criteria.RoutingProductId.Value)));
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

        private bool CheckIfCodeCriteriaSettingsContains(RouteRule routeRule, string code)
        {
            if (routeRule.Criteria.CodeCriteriaGroupSettings != null)
            {
                IRuleCodeCriteria ruleCode = routeRule as IRuleCodeCriteria;
                if (ruleCode.CodeCriterias != null && ruleCode.CodeCriterias.Any(x => x.Code.StartsWith(code)))
                    return true;
            }

            return false;
        }
        private bool CheckIfCustomerSettingsContains(RouteRule routeRule, IEnumerable<int> customerIds)
        {
            if (routeRule.Criteria.CustomerGroupSettings != null)
            {
                IRuleCustomerCriteria ruleCode = routeRule as IRuleCustomerCriteria;
                if (ruleCode.CustomerIds != null && ruleCode.CustomerIds.Intersect(customerIds).Count() > 0)
                    return true;
            }

            return false;
        }
        private bool CheckIfSaleZoneSettingsContains(RouteRule routeRule, IEnumerable<long> saleZoneIds)
        {
            if (routeRule.Criteria.SaleZoneGroupSettings != null)
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
                                RouteRuleCriteria criteria = routeRule.Value.Criteria;

                                string code = CheckAndReturnValidCode(criteria);
                                if (string.IsNullOrEmpty(code))
                                    continue;

                                int? customerId = CheckAndReturnValidCustomer(criteria);
                                if (!customerId.HasValue)
                                    continue;

                                RouteRuleIdentifier routeRuleIdentifier = new RouteRuleIdentifier() {Code = code, CustomerId =  customerId.Value };
                                List<RouteRule> relatedRouteRules = linkedRouteRules.GetOrCreateItem(routeRuleIdentifier);
                                relatedRouteRules.Add(routeRule.Value);
                            }
                        }
                    }

                    return linkedRouteRules;
                });
        }

        public int? CheckAndReturnValidCustomer(RouteRuleCriteria criteria)
        {
            if (criteria.CustomerGroupSettings == null)
                return null;

            SelectiveCustomerGroup selectiveCustomerGroup = criteria.CustomerGroupSettings as SelectiveCustomerGroup;
            if (selectiveCustomerGroup == null)
                return null;

            if (selectiveCustomerGroup.CustomerIds == null || selectiveCustomerGroup.CustomerIds.Count != 1)
                return null;

            return selectiveCustomerGroup.CustomerIds.First();
        }

        public string CheckAndReturnValidCode(RouteRuleCriteria criteria)
        {
            if (criteria.CodeCriteriaGroupSettings == null)
                return null;

            SelectiveCodeCriteriaGroup selectiveCodeCriteriaGroup = criteria.CodeCriteriaGroupSettings as SelectiveCodeCriteriaGroup;
            if (selectiveCodeCriteriaGroup == null)
                return null;

            if (selectiveCodeCriteriaGroup.Codes == null || selectiveCodeCriteriaGroup.Codes.Count != 1)
                return null;
            string code = selectiveCodeCriteriaGroup.Codes.First().Code;

            if (criteria.ExcludedCodes != null && criteria.ExcludedCodes.Contains(code))
                return null;

            return code;
        }
        #endregion

        #region Private Classes

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
                get {  return "Route Rules"; }
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
