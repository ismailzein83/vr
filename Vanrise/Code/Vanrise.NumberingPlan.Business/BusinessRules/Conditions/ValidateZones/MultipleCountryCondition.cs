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
    public class MultipleCountryCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ZoneToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ZoneToProcess zone = context.Target as ZoneToProcess;
            CountryManager countryManager = new CountryManager();
            var countries = new HashSet<string>();

            foreach (var codeToAdd in zone.CodesToAdd)
            {
                if (codeToAdd.CodeGroup != null)
                    countries.Add(countryManager.GetCountryName(codeToAdd.CodeGroup.CountryId));
            }

            foreach (var codeToMove in zone.CodesToMove)
            {
                if (codeToMove.CodeGroup != null)
                    countries.Add(countryManager.GetCountryName(codeToMove.CodeGroup.CountryId));
            }

            foreach (var codeToClose in zone.CodesToClose)
            {
                if (codeToClose.CodeGroup != null)
                    countries.Add(countryManager.GetCountryName(codeToClose.CodeGroup.CountryId));
            }

            if (countries.Count > 1)
            {
                context.Message = string.Format("Can not add zone '{0}' with codes belong to different countries: {1}.", zone.ZoneName, string.Join(", ", countries));
                return false;
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
