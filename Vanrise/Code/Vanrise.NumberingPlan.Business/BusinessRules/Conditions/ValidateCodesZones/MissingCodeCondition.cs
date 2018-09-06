using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
	public class MissingCodeCondition : BusinessRuleCondition
	{
		public override bool ShouldValidate(IRuleTarget target)
		{
			return (target as AllImportedCodes != null);
		}

		public override bool Validate(IBusinessRuleConditionValidateContext context)
		{
			AllImportedCodes allImportedCodes = context.Target as AllImportedCodes;
			var invalidZones = new HashSet<string>();
			foreach (var importedCode in allImportedCodes.ImportedCodes)
			{
				if (string.IsNullOrEmpty(importedCode.Code))
					invalidZones.Add(importedCode.ZoneName);
			}
			if (invalidZones.Count > 0)
			{
				context.Message = string.Format("Codes are missing for the following zone(s): {0}.", string.Join(", ", invalidZones));
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
