using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business.CarrierAccounts;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields.Filters;
using Vanrise.GenericData.Transformation;
using Vanrise.GenericData.Transformation.Entities;

namespace TOne.WhS.BusinessEntity.Business.EventHandler
{
    public class ReevaluateRuleStatusChangedHandler : CarrierAccountStatusChangedEventHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("53C12517-0507-440A-BF48-E2E816F71E7F"); }
        }

        public override void Execute(Vanrise.Entities.IVREventHandlerContext context)
        {
            var eventPayload = context.EventPayload as CarrierAccountStatusChangedEventPayload;
            eventPayload.ThrowIfNull("context.EventPayload", eventPayload);

            DateTime now = DateTime.Now;
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var carrierAccount = carrierAccountManager.GetCarrierAccount(eventPayload.CarrierAccountId);

            if (carrierAccount.CarrierAccountSettings.ActivationStatus == ActivationStatus.Inactive)
            {
                MappingRuleManager mappingRuleManager = new MappingRuleManager();
                ConfigManager configManager = new ConfigManager();

                HashSet<Guid> mappingRuleDefinitionIds = new HashSet<Guid>();

                switch (carrierAccount.AccountType)
                {
                    case CarrierAccountType.Customer:
                        mappingRuleDefinitionIds.Add(configManager.GetCustomerRuleDefinitionId());
                        break;

                    case CarrierAccountType.Supplier:
                        mappingRuleDefinitionIds.Add(configManager.GetSupplierRuleDefinitionId());
                        break;

                    case CarrierAccountType.Exchange:
                        mappingRuleDefinitionIds.Add(configManager.GetCustomerRuleDefinitionId());
                        mappingRuleDefinitionIds.Add(configManager.GetSupplierRuleDefinitionId());
                        break;
                }

                //BusinessEntityFieldTypeFilter settingFilter = new BusinessEntityFieldTypeFilter() { BusinessEntityIds = new List<object>() { carrierAccount.CarrierAccountId } };

                foreach (Guid mappingRuleDefinitionId in mappingRuleDefinitionIds)
                {
                    GenericRuleQuery query = new GenericRuleQuery() { RuleDefinitionId = mappingRuleDefinitionId };//, SettingsFilterValue = settingFilter };
                    List<MappingRule> mappingRules = mappingRuleManager.GetApplicableFilteredRules(mappingRuleDefinitionId, query, true);

                    if (mappingRules != null)
                    {
                        foreach (MappingRule mappingRule in mappingRules)
                        {
                            if (Convert.ToInt32(mappingRule.Settings.Value) == carrierAccount.CarrierAccountId)
                            {
                                if (!mappingRule.EndEffectiveTime.HasValue || mappingRule.EndEffectiveTime.Value > now)
                                {
                                    if (mappingRule.BeginEffectiveTime < now)
                                        mappingRule.EndEffectiveTime = now;
                                    else
                                        mappingRule.EndEffectiveTime = mappingRule.BeginEffectiveTime;

                                    mappingRuleManager.UpdateRule(mappingRule);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
