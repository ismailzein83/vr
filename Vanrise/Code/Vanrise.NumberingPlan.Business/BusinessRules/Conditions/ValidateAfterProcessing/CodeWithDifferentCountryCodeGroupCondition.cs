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
            ICPParametersContext cpContext = context.GetExtension<ICPParametersContext>();
            ExistingZoneInfoByZoneName existingZonesInfoByZoneName = cpContext.ExistingZonesInfoByZoneName;
            ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;
            if (zoneToProcess.ChangeType != ZoneChangeType.New)
                return true;

            var invalidCodes = new HashSet<string>();
            ExistingZoneInfo existingZoneInfoInContext;

            if (existingZonesInfoByZoneName.TryGetValue(zoneToProcess.ZoneName, out existingZoneInfoInContext))
            {
                foreach (var code in zoneToProcess.CodesToAdd)
                {
                    if (existingZoneInfoInContext.CountryId != code.CodeGroup.CountryId)
                        invalidCodes.Add(code.Code);
                }
                foreach (var code in zoneToProcess.CodesToMove)
                {
                    if (existingZoneInfoInContext.CountryId != code.CodeGroup.CountryId)
                        invalidCodes.Add(code.Code);
                }
                if (invalidCodes.Count > 0)
                {
                    CountryManager countryManager = new CountryManager();
                    var countryName = countryManager.GetCountryName(existingZoneInfoInContext.CountryId);
                    context.Message = string.Format("Can not add or move codes ({0}) to the zone '{1}' because codes do not belong to country {2}.", string.Join(", ", invalidCodes), zoneToProcess.ZoneName, countryName);
                    return false;
                }
            }
            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }

    }
}
