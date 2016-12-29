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
            bool resultOfCodeToAdd = true;
            bool resultOfCodeToMove = true;
            bool resultOfCodeToClose = true;

            int? firstCodeToAddCountryId = null;
            int? firstCodeToMoveCountryId = null;
            int? firstCodeToCloseCountryId = null;


            if (zone.CodesToAdd != null)
            {
                var firstCode = zone.CodesToAdd.FirstOrDefault();
                if (firstCode != null)
                {
                    firstCodeToAddCountryId = firstCode.CodeGroup.CountryId;
                    Func<CodeToAdd, bool> pred = new Func<CodeToAdd, bool>((code) => code.CodeGroup.CountryId != firstCodeToAddCountryId.Value);
                    resultOfCodeToAdd = !zone.CodesToAdd.Any(pred);
                }
            }


            if (zone.CodesToMove != null)
            {
                var firstCode = zone.CodesToMove.FirstOrDefault();
                if (firstCode != null)
                {
                    firstCodeToMoveCountryId = firstCode.CodeGroup.CountryId;
                    Func<CodeToMove, bool> pred = new Func<CodeToMove, bool>((code) => code.CodeGroup.CountryId != firstCodeToMoveCountryId.Value);
                    resultOfCodeToMove = !zone.CodesToMove.Any(pred);
                }
            }

            if (zone.CodesToClose != null)
            {
                var firstCode = zone.CodesToClose.FirstOrDefault();
                if (firstCode != null)
                {
                    firstCodeToCloseCountryId = firstCode.CodeGroup.CountryId;
                    Func<CodeToClose, bool> pred = new Func<CodeToClose, bool>((code) => code.CodeGroup.CountryId != firstCodeToCloseCountryId.Value);
                    resultOfCodeToClose = !zone.CodesToClose.Any(pred);
                }
            }


            if (firstCodeToAddCountryId != null && firstCodeToMoveCountryId != null && firstCodeToCloseCountryId != null)
                result = (firstCodeToAddCountryId == firstCodeToCloseCountryId) && (firstCodeToAddCountryId == firstCodeToMoveCountryId);
          
            bool finalResult = resultOfCodeToAdd && resultOfCodeToMove && resultOfCodeToClose && result;
            
            if (finalResult == false)
                context.Message = string.Format("Zone {0} has multiple codes that belong to different countries", zone.ZoneName);
           
            return finalResult;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has multiple codes that belong to different countries", (target as ZoneToProcess).ZoneName);
        }

    }
}
