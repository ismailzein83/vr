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
            return (target as AllCountriesToProcess != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            CountryManager manager = new CountryManager();
            AllCountriesToProcess allCountriesToProcess = context.Target as AllCountriesToProcess;

            var invalidCodes = new HashSet<string>();
            var invalidZones = new HashSet<string>();

            foreach (var country in allCountriesToProcess.Countries)
            {
                if (country.CodesToAdd != null)
                {
                    foreach (CodeToAdd codeToAdd in country.CodesToAdd)
                    {
                        if (country.CodesToAdd.FindRecord(x => x.Code == codeToAdd.Code && !x.ZoneName.Equals(codeToAdd.ZoneName, StringComparison.InvariantCultureIgnoreCase)) != null)
                        {
                            invalidCodes.Add(codeToAdd.Code);
                            invalidZones.Add(codeToAdd.ZoneName);
                        }
                    }
                }

                if (country.CodesToMove != null)
                {
                    foreach (CodeToMove codeToMove in country.CodesToMove)
                    {
                        if (country.CodesToMove.FindRecord(x => x.Code == codeToMove.Code && !x.ZoneName.Equals(codeToMove.ZoneName, StringComparison.InvariantCultureIgnoreCase)) != null)
                        {
                            invalidCodes.Add(codeToMove.Code);
                            invalidZones.Add(codeToMove.ZoneName);
                        }
                    }
                }

                if (country.CodesToClose != null)
                {
                    foreach (CodeToClose codeToClose in country.CodesToClose)
                    {
                        if (country.CodesToClose.FindRecord(x => x.Code == codeToClose.Code && !x.ZoneName.Equals(codeToClose.ZoneName, StringComparison.InvariantCultureIgnoreCase)) != null)
                        {
                            invalidCodes.Add(codeToClose.Code);
                            invalidZones.Add(codeToClose.ZoneName);
                        }
                    }
                }

            }

            if (invalidCodes.Count > 0)
            {
                context.Message = string.Format("Performing same action more than one time on codes ({0}) in zones ({1}).", string.Join(", ", invalidCodes), string.Join(", ", invalidZones));
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
