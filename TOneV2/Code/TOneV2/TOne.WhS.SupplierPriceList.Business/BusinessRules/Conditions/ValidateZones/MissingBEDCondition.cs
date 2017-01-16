using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class MissingBEDCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedDataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedDataByZone zone = context.Target as ImportedDataByZone;

            foreach (var importedCode in zone.ImportedCodes)
            {
                if (importedCode.BED == DateTime.MinValue)
                {
                    context.Message = string.Format("Code {0} has a missing begin effective date", importedCode.Code);
                    return false;
                }
            }

            foreach (var importedNormalRate in zone.ImportedNormalRates)
            {
                if (importedNormalRate.BED == DateTime.MinValue)
                {
                    context.Message = string.Format("The Normal Rate of Zone {0} has a missing begin effective date", zone.ZoneName);
                    return false;
                }
            }

            foreach (var importedOtherRate in zone.ImportedOtherRates)
            {
                if (importedOtherRate.Value.Count > 0)
                {
                    foreach (var otherRate in importedOtherRate.Value)
                    {
                        if (otherRate.BED == DateTime.MinValue)
                        {
                            RateTypeManager rateTypeManager = new RateTypeManager();
                            string rateTypeName = rateTypeManager.GetRateTypeName(otherRate.RateTypeId.Value);
                            context.Message = string.Format("The {0} Rate of Zone {1} has a missing begin effective date", rateTypeName, zone.ZoneName);
                            return false;
                        }
                    }
                }
            }

            foreach (var importedZoneServiceGroup in zone.ImportedZoneServicesToValidate)
            {
                if (importedZoneServiceGroup.Value.Count > 0)
                {
                    foreach (var serviceGroup in importedZoneServiceGroup.Value)
                    {
                        if (serviceGroup.BED == DateTime.MinValue)
                        {
                            ZoneServiceConfigManager serviceConfigManager = new ZoneServiceConfigManager();
                            string serviceSymbol = serviceConfigManager.GetServiceSymbol(serviceGroup.ServiceId);
                            context.Message = string.Format("The {0} Service of Zone {1} has a missing begin effective date", serviceSymbol, zone.ZoneName);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a missing begin effective date", (target as ImportedDataByZone).ZoneName);
        }

    }
}
