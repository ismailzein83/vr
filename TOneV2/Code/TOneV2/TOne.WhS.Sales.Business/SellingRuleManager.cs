using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class SellingRuleManager : Vanrise.Rules.RuleManager<SellingRule, SellingRuleDetail>
    {

        public override SellingRuleDetail MapToDetails(SellingRule rule)
        {
            return new SellingRuleDetail()
            {
                Entity = rule
            };
        }

        public Vanrise.Entities.IDataRetrievalResult<SellingRuleDetail> GetFilteredSellingRules(Vanrise.Entities.DataRetrievalInput<SellingRuleQuery> input)
        {
            var sellingRules = base.GetAllRules();
            Func<SellingRule, bool> filterExpression = (sellingRule) =>
                (input.Query.SellingProductId == null || sellingRule.Criteria.SellingProductId == input.Query.SellingProductId)
                 && (input.Query.CustomerIds == null || this.CheckIfCustomerSettingsContains(sellingRule, input.Query.CustomerIds))
                 && (input.Query.SaleZoneIds == null || this.CheckIfSaleZoneSettingsContains(sellingRule, input.Query.SaleZoneIds));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, sellingRules.ToBigResult(input, filterExpression, MapToDetails));
        }

        private bool CheckIfCustomerSettingsContains(SellingRule sellingRule, IEnumerable<int> customerIds)
        {
            if (sellingRule.Criteria.CustomerGroupSettings != null)
            {
                IRuleCustomerCriteria ruleCode = sellingRule as IRuleCustomerCriteria;
                if (ruleCode.CustomerIds != null && ruleCode.CustomerIds.Intersect(customerIds).Count() > 0)
                    return true;
            }

            return false;
        }

        private bool CheckIfSaleZoneSettingsContains(SellingRule sellingRule, IEnumerable<long> saleZoneIds)
        {
            if (sellingRule.Criteria.SaleZoneGroupSettings != null)
            {
                IRuleSaleZoneCriteria ruleZone = sellingRule as IRuleSaleZoneCriteria;
                if (ruleZone.SaleZoneIds != null && ruleZone.SaleZoneIds.Intersect(saleZoneIds).Count() > 0)
                    return true;
            }

            return false;
        }

        public IEnumerable<SellingRuleSettingsConfig> GetSellingRuleTypesTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SellingRuleSettingsConfig>(SellingRuleSettingsConfig.EXTENSION_TYPE);
        }
    }
}
