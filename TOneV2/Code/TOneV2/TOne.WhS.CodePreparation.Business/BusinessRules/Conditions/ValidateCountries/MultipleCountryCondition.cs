using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class MultipleCountryCondition : BusinessRuleCondition
    {

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            
            ZoneToProcess zone = context.Target as ZoneToProcess;

            bool result = true;

            if (zone.CodesToAdd != null)
            {
                var firstCode = zone.CodesToAdd.FirstOrDefault();
                if (firstCode != null)
                {
                    int? firstCodeCountryId = firstCode.CodeGroup != null ? firstCode.CodeGroup.CountryId : (int?)null;
                    Func<CodeToAdd, bool> pred = new Func<CodeToAdd, bool>((code) => code.CodeGroup != null && firstCodeCountryId.HasValue && code.CodeGroup.CountryId != firstCodeCountryId.Value);
                    result = !zone.CodesToAdd.Any(pred);
                }
            }


            if (zone.CodesToMove != null)
            {
                var firstCode = zone.CodesToMove.FirstOrDefault();
                if (firstCode != null)
                {
                    int? firstCodeCountryId = firstCode.CodeGroup != null ? firstCode.CodeGroup.CountryId : (int?)null;
                    Func<CodeToMove, bool> pred = new Func<CodeToMove, bool>((code) => code.CodeGroup != null && firstCodeCountryId.HasValue && code.CodeGroup.CountryId != firstCodeCountryId.Value);
                    result = !zone.CodesToMove.Any(pred);
                }
            }

            return result;


        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has multiple codes that belong to different countries", (target as ZoneToProcess).ZoneName);
        }

    }
}
