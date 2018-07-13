using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class ZoneCountryIsEndedAndZoneIsChangedCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is ZoneDataByCountryIds;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var zoneDataByCountryId = context.Target as ZoneDataByCountryIds;

            var invalidCountryNames = new List<string>();
            var countryManager = new Vanrise.Common.Business.CountryManager();

            foreach (KeyValuePair<int, List<DataByZone>> kvp in zoneDataByCountryId)
            {
                foreach (DataByZone zoneData in kvp.Value)
                {
                    if (zoneData.IsCountryEnded)
                    {
                        IEnumerable<string> actionMessages = BusinessRuleUtilities.GetZoneActionMessages(zoneData);
                        if (actionMessages != null && actionMessages.Count() > 0)
                        {
                            string invalidCountryName = countryManager.GetCountryName(zoneData.CountryId);
                            if (invalidCountryName != null)
                                invalidCountryNames.Add(invalidCountryName);
                            break;
                        }
                    }
                }
            }

            if (invalidCountryNames.Count > 0)
            {
                context.Message = string.Format("Can not apply any changes on the following closed country(ies) : {0}", string.Join(",", invalidCountryNames));
                return false;
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
