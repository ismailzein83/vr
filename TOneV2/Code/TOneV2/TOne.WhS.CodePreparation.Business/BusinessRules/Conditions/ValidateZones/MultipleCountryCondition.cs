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
            bool resultOfCodesToAdd = true;
            bool resultOfCodesToMove = true;
            bool resultOfCodesToClose = true;

            int? firstCodeToAddCountryId = null;
            int? firstCodeToMoveCountryId = null;
            int? firstCodeToCloseCountryId = null;

            var firstCodeToAdd = zone.CodesToAdd.FirstOrDefault();
            if (firstCodeToAdd != null)
            {
                firstCodeToAddCountryId = firstCodeToAdd.CodeGroup.CountryId;
                Func<CodeToAdd, bool> pred = new Func<CodeToAdd, bool>((code) => code.CodeGroup.CountryId != firstCodeToAddCountryId.Value);
                resultOfCodesToAdd = !zone.CodesToAdd.Any(pred);
            }

            var firstCodeToMove = zone.CodesToMove.FirstOrDefault();
            if (firstCodeToMove != null)
            {
                firstCodeToMoveCountryId = firstCodeToMove.CodeGroup.CountryId;
                Func<CodeToMove, bool> pred = new Func<CodeToMove, bool>((code) => code.CodeGroup.CountryId != firstCodeToMoveCountryId.Value);
                resultOfCodesToMove = !zone.CodesToMove.Any(pred);
            }

            var firstCodeToClose = zone.CodesToClose.FirstOrDefault();
            if (firstCodeToClose != null)
            {
                firstCodeToCloseCountryId = firstCodeToClose.CodeGroup.CountryId;
                Func<CodeToClose, bool> pred = new Func<CodeToClose, bool>((code) => code.CodeGroup.CountryId != firstCodeToCloseCountryId.Value);
                resultOfCodesToClose = !zone.CodesToClose.Any(pred);
            }

            if (firstCodeToAddCountryId.HasValue && firstCodeToMoveCountryId.HasValue && firstCodeToCloseCountryId.HasValue)
                result = (firstCodeToAddCountryId == firstCodeToCloseCountryId) && (firstCodeToAddCountryId == firstCodeToMoveCountryId);

            bool finalResult = resultOfCodesToAdd && resultOfCodesToMove && resultOfCodesToClose && result;

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
