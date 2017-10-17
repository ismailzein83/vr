using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
    public class NewZoneWithNoRatesCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is CountryToProcess;
        }
        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var countryToProcess = context.Target as CountryToProcess;

            if (countryToProcess.ZonesToProcess == null || countryToProcess.ZonesToProcess.Count == 0)
                return true;

            var invalidZoneNames = new List<string>();

            foreach (ZoneToProcess zoneToProcess in countryToProcess.ZonesToProcess)
            {
                if (zoneToProcess.ChangeType == ZoneChangeType.New && zoneToProcess.RatesToAdd.Count == 0)
                    invalidZoneNames.Add(zoneToProcess.ZoneName);
            }

            if (invalidZoneNames.Count > 0)
            {
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryToProcess.CountryId);
                context.Message = string.Format("The following zones of country '{0}' have been created without rates: {1}", countryName, string.Join(", ", invalidZoneNames));
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
