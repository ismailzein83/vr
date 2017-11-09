using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;

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

            if(countries.Count>1)
            {
                context.Message = string.Format("Can not add zone '{0}' with codes belong to different countries: {1}.", zone.ZoneName, string.Join(", ",countries));
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
