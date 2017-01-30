using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class CodeWithDifferentCountryCodeGroupCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            bool result = true;
            ICPParametersContext cpContext = context.GetExtension<ICPParametersContext>();

            ExistingZoneInfoByZoneName existingZonesInfoByZoneName = cpContext.ExistingZonesInfoByZoneName;


            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            if (zoneToProcess.ChangeType != ZoneChangeType.New)
                return true;


            ExistingZoneInfo existingZoneInfoInContext;
            int zoneToProcessCountryId = zoneToProcess.AddedZones.First().CountryId;

            if (existingZonesInfoByZoneName.TryGetValue(zoneToProcess.ZoneName, out existingZoneInfoInContext))
            {
                if (existingZoneInfoInContext.CountryId != zoneToProcessCountryId)
                {
                    CountryManager manager = new CountryManager();
                    result = false;
                    string countryName = manager.GetCountryName(zoneToProcess.AddedZones.First().CountryId);
                    context.Message = string.Format("Zone {0} has a code that belongs to the code group of country {1}", zoneToProcess.ZoneName, countryName);
                }
            }
            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            ZoneToProcess zoneToProcess = target as ZoneToProcess;
            CountryManager manager = new CountryManager();
            string countryName = manager.GetCountryName(zoneToProcess.AddedZones.First().CountryId);
            return string.Format("Zone {0} has a code that belongs to the code group of country {1}", zoneToProcess.ZoneName, countryName);
        }

    }
}
