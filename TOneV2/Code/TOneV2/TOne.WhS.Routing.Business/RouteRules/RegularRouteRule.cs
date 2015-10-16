using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business.RouteOptionRules;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules
{
    public class RegularRouteRule : RouteRuleSettings
    {
        RouteOptionRuleManager _routeOptionRuleManager = new RouteOptionRuleManager();

        public override void Execute(IRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            SetOptions(context, target);

            if (this.OptionOrderSettings != null)
                this.OptionOrderSettings.Execute(context, target);

            if (this.OptionPercentageSettings != null)
                this.OptionPercentageSettings.Execute(context, target);
        }

        private void SetOptions(IRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            target.Options = new List<RouteOptionRuleTarget>();
            var supplierCodeMatches = context.SupplierCodeMatches;
            if (supplierCodeMatches == null)
                return;
            
            if(this.OptionsSettingsGroup != null)
            {
                RouteOptionSettingsContext routeOptionSettingsContext = new RouteOptionSettingsContext();
                var optionsSettings = this.OptionsSettingsGroup.GetOptionSettings(routeOptionSettingsContext);
                if(optionsSettings != null)
                {
                    foreach(var optionSettings in optionsSettings)
                    {
                        foreach(var supplierCodeMatch in supplierCodeMatches)
                        {
                            if(supplierCodeMatch.SupplierId == optionSettings.SupplierId)
                            {
                                CreateAndAddOption(target, supplierCodeMatch, optionSettings.Percentage, optionSettings.Filter != null ? optionSettings.Filter : this.OptionFilterSettings);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var supplierCodeMatch in supplierCodeMatches)
                {
                    CreateAndAddOption(target, supplierCodeMatch, null, this.OptionFilterSettings);
                }
            }
        }

        private void CreateAndAddOption(RouteRuleTarget routeRuleTarget, SupplierCodeMatch supplierCodeMatch, Decimal? percentage, RouteRuleOptionFilterSettings optionFilter)
        {
            var option = new RouteOptionRuleTarget
            {
                RouteTarget = routeRuleTarget,
                SupplierId = supplierCodeMatch.SupplierId,
                SupplierCode = supplierCodeMatch.SupplierCode,
                SupplierZoneId = supplierCodeMatch.SupplierZoneId,
                SupplierRate = supplierCodeMatch.SupplierRate,
                EffectiveOn = routeRuleTarget.EffectiveOn
            };
            if (percentage.HasValue)
                option.Percentage = percentage.Value;
            RouteOptionRuleExecutionContext routeOptionRuleExecutionContext = new RouteOptionRuleExecutionContext();
            if(optionFilter != null)
                optionFilter.Execute(routeOptionRuleExecutionContext, option);
            if (!option.FilterOption)
            {
                var routeOptionRule = _routeOptionRuleManager.GetMatchRule(option);
                if (routeOptionRule != null)
                    routeOptionRule.Settings.Execute(routeOptionRuleExecutionContext, option);                
            }
            if (!option.BlockOption && !option.FilterOption)
                routeRuleTarget.Options.Add(option);
        }

        public RouteOptionSettingsGroup OptionsSettingsGroup { get; set; }

        public RouteRuleOptionOrderSettings OptionOrderSettings { get; set; }

        public RouteRuleOptionFilterSettings OptionFilterSettings { get; set; }

        public RouteRuleOptionPercentageSettings OptionPercentageSettings { get; set; }
    }
}
