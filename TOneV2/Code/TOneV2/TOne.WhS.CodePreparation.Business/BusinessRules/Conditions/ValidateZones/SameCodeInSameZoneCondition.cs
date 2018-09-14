using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.CodePreparation.Business
{
	public class SameCodeInSameZoneCondition : BusinessRuleCondition
	{
		public override bool ShouldValidate(IRuleTarget target)
		{
			return (target as ZoneToProcess != null);
		}

		public override bool Validate(IBusinessRuleConditionValidateContext context)
		{
			/* 
             * This rule is to check action status on codes:
             * 1- if we have same action more than once 
             * 2- or we have different action on same code
            */

			ZoneToProcess zoneToProcess = context.Target as ZoneToProcess;
			var invalidCodes = new HashSet<string>();
			var sameCodeDifferentStatus = new HashSet<string>();

			foreach (CodeToAdd codeToAdd in zoneToProcess.CodesToAdd)
			{
				if (zoneToProcess.CodesToAdd.FindAllRecords(item => item.Code == codeToAdd.Code).Count() > 1)
					invalidCodes.Add(codeToAdd.Code);
				foreach (var codeToClose in zoneToProcess.CodesToClose)
				{
					if (codeToClose.Code == codeToAdd.Code && codeToClose.ZoneName.Equals(codeToAdd.ZoneName, StringComparison.InvariantCultureIgnoreCase))
					{
						sameCodeDifferentStatus.Add(codeToAdd.Code);
					}
				}
				foreach (var codeToMove in zoneToProcess.CodesToMove)
				{
					if (codeToMove.Code == codeToAdd.Code && codeToMove.ZoneName.Equals(codeToAdd.ZoneName, StringComparison.InvariantCultureIgnoreCase))
					{
						sameCodeDifferentStatus.Add(codeToAdd.Code);
					}
				}
			}

			foreach (CodeToClose codeToClose in zoneToProcess.CodesToClose)
			{
				if (zoneToProcess.CodesToClose.FindAllRecords(item => item.Code == codeToClose.Code).Count() > 1)
					invalidCodes.Add(codeToClose.Code);
			}

			foreach (CodeToMove codeToMove in zoneToProcess.CodesToMove)
			{
				if (codeToMove.ZoneName.Equals(codeToMove.OldZoneName, StringComparison.InvariantCultureIgnoreCase))
					invalidCodes.Add(codeToMove.Code);

			}
			if (invalidCodes.Count > 0)
				context.Message += string.Format("Performing same action more than one time on code(s) ({0}) of zone '{1}'.", string.Join(", ", invalidCodes), zoneToProcess.ZoneName);

			if (sameCodeDifferentStatus.Count > 0)
				context.Message += string.Format("Performing different action on same code(s) ({0}) of zone '{1}'.", string.Join(", ", sameCodeDifferentStatus), zoneToProcess.ZoneName);

			if (invalidCodes.Count > 0 || sameCodeDifferentStatus.Count > 0)
				return false;

			return true;
		}

		public override string GetMessage(IRuleTarget target)
		{
			throw new NotImplementedException();
		}
	}
}
