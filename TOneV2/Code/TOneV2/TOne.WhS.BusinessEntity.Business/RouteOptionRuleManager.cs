using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RouteOptionRuleManager
    {
        public StructuredRouteOptionRules StructureRouteOptionRules(List<RouteOptionRule> rules)
        {
            TemplateConfigManager templateConfigManager = new TemplateConfigManager();
            Dictionary<int, SupplierRules> rulesBySuppliers = new Dictionary<int, SupplierRules>();
            foreach(var rule in rules)
            {
                RouteOptionRuleBehavior optionRuleBehavior = templateConfigManager.GetBehavior<RouteOptionRuleBehavior>(rule.Type);
                var ruleSuppliersWithZones = optionRuleBehavior.Execute(rule);
                if(ruleSuppliersWithZones != null && ruleSuppliersWithZones.SuppliersWithZones != null)
                {
                    foreach (var supplierWithZones in ruleSuppliersWithZones.SuppliersWithZones)
                    {
                        SupplierRules supplierRules;
                        if (!rulesBySuppliers.TryGetValue(supplierWithZones.SupplierId, out supplierRules))
                        {
                            supplierRules = new SupplierRules() { RulesBySupplierZones = new Dictionary<long, List<RouteOptionRule>>() };
                            rulesBySuppliers.Add(supplierWithZones.SupplierId, supplierRules);
                        }
                        if (supplierWithZones.SupplierZones != null && supplierWithZones.SupplierZones.Count > 0)
                        {
                            foreach(var supplierZoneId in supplierWithZones.SupplierZones)
                            {
                                List<RouteOptionRule> zoneRules;
                                if(!supplierRules.RulesBySupplierZones.TryGetValue(supplierZoneId, out zoneRules))
                                {
                                    zoneRules = new List<RouteOptionRule>();
                                    supplierRules.RulesBySupplierZones.Add(supplierZoneId, zoneRules);
                                }
                                zoneRules.Add(rule);
                            }
                        }
                        else
                            supplierRules.AllSupplierZonesRules.Add(rule);
                    }
                }
            }

            RouteRuleManager routeRuleManager = new RouteRuleManager();
            StructuredRouteOptionRules structuredRules = new StructuredRouteOptionRules { RulesBySupplier = new Dictionary<int, SupplierRouteOptionRules>() };
            foreach(var supplierRules in rulesBySuppliers)
            {
                SupplierRouteOptionRules structuredSupplierRules = new SupplierRouteOptionRules { RulesBySupplierZones = new Dictionary<long, StructuredRouteRules<RouteOptionRule>>() };
                if (supplierRules.Value.AllSupplierZonesRules != null)
                    structuredSupplierRules.AllSupplierZonesRules = routeRuleManager.StructureRules<RouteOptionRule>(supplierRules.Value.AllSupplierZonesRules);
                foreach(var supplierZoneRules in supplierRules.Value.RulesBySupplierZones)
                {
                    structuredSupplierRules.RulesBySupplierZones.Add(supplierZoneRules.Key, routeRuleManager.StructureRules<RouteOptionRule>(supplierZoneRules.Value));
                }
                structuredRules.RulesBySupplier.Add(supplierRules.Key, structuredSupplierRules);
            }
            return structuredRules; 
        }

        public RouteOptionRule GetMatchedRule(StructuredRouteOptionRules rules, int supplierId, long supplierZoneId, int? customerId, int? productId, string code, long saleZoneId)
        {
            SupplierRouteOptionRules supplierRules;
            if(rules.RulesBySupplier.TryGetValue(supplierId, out supplierRules))
            {
                 RouteRuleManager routeRuleManager = new RouteRuleManager();
                if(supplierRules.AllSupplierZonesRules != null)
                {
                    RouteOptionRule rule = routeRuleManager.GetMostMatchedRule<RouteOptionRule>(supplierRules.AllSupplierZonesRules, customerId, productId, code, saleZoneId);
                    if (rule != null)
                        return rule;
                }

                StructuredRouteRules<RouteOptionRule> supplierZoneRules;
                if(supplierRules.RulesBySupplierZones.TryGetValue(supplierZoneId, out supplierZoneRules))
                {
                    RouteOptionRule rule = routeRuleManager.GetMostMatchedRule<RouteOptionRule>(supplierZoneRules, customerId, productId, code, saleZoneId);
                    if (rule != null)
                        return rule;
                }
            }
            return null;
        }

        #region Private Classes

        private class SupplierRules
        {
            public List<RouteOptionRule> AllSupplierZonesRules { get; set; }
            public Dictionary<long, List<RouteOptionRule>> RulesBySupplierZones { get; set; }
        }

        #endregion
    }
}
