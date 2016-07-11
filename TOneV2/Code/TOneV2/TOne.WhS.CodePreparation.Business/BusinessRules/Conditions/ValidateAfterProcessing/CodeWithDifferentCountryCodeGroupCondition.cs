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
            return (target as AddedZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ICPParametersContext cpContext = context.GetExtension<ICPParametersContext>();

            ExistingZoneInfoByZoneName existingZonesInfoByZoneName = cpContext.ExistingZonesInfoByZoneName;


            AddedZone addedZone = context.Target as AddedZone;

            ExistingZoneInfo existingZoneInfoInContext;

            if (existingZonesInfoByZoneName.TryGetValue(addedZone.Name, out existingZoneInfoInContext))
                return existingZoneInfoInContext.CountryId == addedZone.CountryId;

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            CountryManager manager = new CountryManager();
            AddedZone addedZone = target as AddedZone;
            return string.Format("Zone {0} has a code that belongs to the code group of country {1}", addedZone.Name , manager.GetCountryName(addedZone.CountryId));
        }

    }
}
