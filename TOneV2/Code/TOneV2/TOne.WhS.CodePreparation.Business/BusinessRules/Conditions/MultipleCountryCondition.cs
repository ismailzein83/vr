﻿using System;
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
            if (context.Target == null)
                throw new ArgumentNullException("Target");

            IRuleTarget target = context.Target;

            ZoneToProcess zone = target as ZoneToProcess;

            if (zone == null)
                return false;

            bool result = true;
            bool resultOfCodeToAdd = true;
            int? firstCodeToAddCountryId = null;
            bool resultOfCodeToMove = true;
            int? firstCodeToMoveCountryId = null;


            if (zone != null)
            {
                CodeToAdd firstCodeToAdd=null;
                if(zone.CodesToAdd != null)
                    firstCodeToAdd = zone.CodesToAdd.FirstOrDefault();

                CodeToMove firstCodeToMove = null;
                if (zone.CodesToMove !=null )
                   firstCodeToMove  = zone.CodesToMove.FirstOrDefault();

                if (firstCodeToAdd != null)
                {
                    firstCodeToAddCountryId =  firstCodeToAdd.CodeGroup.CountryId;
                    Func<CodeToAdd, bool> pred = new Func<CodeToAdd, bool>((code) => code.CodeGroup != null && firstCodeToAddCountryId.HasValue && code.CodeGroup.CountryId != firstCodeToAddCountryId.Value);
                    resultOfCodeToAdd = !zone.CodesToAdd.Any(pred);
                }

                if (firstCodeToMove != null)
                {
                    firstCodeToMoveCountryId =  firstCodeToMove.CodeGroup.CountryId;
                    Func<CodeToMove, bool> pred = new Func<CodeToMove, bool>((code) => code.CodeGroup != null && firstCodeToAddCountryId.HasValue && code.CodeGroup.CountryId != firstCodeToMoveCountryId.Value);
                    resultOfCodeToMove = !zone.CodesToMove.Any(pred);
                }


            }
            if (firstCodeToMoveCountryId != null && firstCodeToAddCountryId != null)
                result = (firstCodeToMoveCountryId == firstCodeToAddCountryId);
            result = (resultOfCodeToMove == true && resultOfCodeToAdd == true && result == true);
            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has multiple codes that belong to different countries", (target as ZoneToProcess).ZoneName);
        }

    }
}
