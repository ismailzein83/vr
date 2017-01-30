using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
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
                        context.Message = string.Format("Can not add Code {0} because Country {1} contains this code with same status in different zones", codeToAdd.Code, manager.GetCountryName(country.CountryId));
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
                        context.Message = string.Format("Can not move Code {0} because Country {1} contains this code with same status in different zones", codeToMove.Code, manager.GetCountryName(country.CountryId));
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
                        context.Message = string.Format("Can not close Code {0} because Country {1} contains this code with same status in different zones", codeToClose.Code, manager.GetCountryName(country.CountryId));
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
