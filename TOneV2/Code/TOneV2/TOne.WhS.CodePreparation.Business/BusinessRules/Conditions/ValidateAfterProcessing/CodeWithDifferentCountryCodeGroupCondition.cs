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

            Dictionary<string, ExistingZone> existingZonesByZoneName = cpContext.ExistingZonesByZoneName;


            AddedZone addedZone = context.Target as AddedZone;

            ExistingZone existingZoneInContext;

            if (existingZonesByZoneName.TryGetValue(addedZone.Name, out existingZoneInContext) )
               return existingZoneInContext.CountryId == addedZone.CountryId;

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} already exist", (target as AddedZone).Name);
        }

    }
}
