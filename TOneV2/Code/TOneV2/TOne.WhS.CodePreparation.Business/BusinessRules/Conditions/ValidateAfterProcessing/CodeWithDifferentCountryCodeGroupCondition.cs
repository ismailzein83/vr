using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.CodePreparation.Business
{
    public class CodeWithDifferentCountryCodeGroupCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ICPParametersContext cpContext = context.GetExtension<ICPParametersContext>();

            ExistingZoneInfoByZoneName existingZonesInfoByZoneName = cpContext.ExistingZonesInfoByZoneName;


            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;

            if (zoneToProcess.ChangeType != ZoneChangeType.New)
                return true;


            ExistingZoneInfo existingZoneInfoInContext;
            int zoneToProcessCountryId = zoneToProcess.AddedZones.First().CountryId;

            if (existingZonesInfoByZoneName.TryGetValue(zoneToProcess.ZoneName, out existingZoneInfoInContext))
                return existingZoneInfoInContext.CountryId == zoneToProcessCountryId;

            return true;
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
