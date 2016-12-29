using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
    public class SameCodeInDifferentZoneCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as CountryToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            CountryToProcess country = context.Target as CountryToProcess;
            CountryManager manager = new CountryManager();
            if (country.CodesToAdd != null)
            {
                foreach (CodeToAdd codeToAdd in country.CodesToAdd)
                {
                    if (country.CodesToAdd.FindRecord(x => x.Code == codeToAdd.Code && !x.ZoneName.Equals(codeToAdd.ZoneName, StringComparison.InvariantCultureIgnoreCase)) != null)
                    {
                        context.Message = string.Format("Country {0} has duplicate code {1} with same status in different zones", manager.GetCountryName(country.CountryId), codeToAdd.Code);
                        return false;
                    }
                }
            }

            if (country.CodesToMove != null)
            {
                foreach (CodeToMove codeToMove in country.CodesToMove)
                {
                    if (country.CodesToMove.FindRecord(x => x.Code == codeToMove.Code && !x.ZoneName.Equals(codeToMove.ZoneName, StringComparison.InvariantCultureIgnoreCase)) != null)
                    {
                        context.Message = string.Format("Country {0} has duplicate code {1} with same status in different zones", manager.GetCountryName(country.CountryId), codeToMove.Code);
                        return false;
                    }
                }
            }

            if (country.CodesToClose != null)
            {
                foreach (CodeToClose codeToClose in country.CodesToClose)
                {
                    if (country.CodesToClose.FindRecord(x => x.Code == codeToClose.Code && !x.ZoneName.Equals(codeToClose.ZoneName, StringComparison.InvariantCultureIgnoreCase)) != null)
                    {
                        context.Message = string.Format("Country {0} has duplicate code {1} with same status in different zones", manager.GetCountryName(country.CountryId), codeToClose.Code);
                        return false;
                    }
                }
            }

            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            CountryManager manager = new CountryManager();
            return string.Format("Country {0} has duplicate code with same status in different zones", manager.GetCountryName((target as CountryToProcess).CountryId));
        }
    }
}
