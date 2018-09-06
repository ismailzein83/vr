using System;
using System.Linq;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
	public class WrongMoveCondition : BusinessRuleCondition
	{
		public override bool ShouldValidate(IRuleTarget target)
		{
			return (target as CodeToAdd != null && target as CodeToMove == null);
		}

		public override bool Validate(IBusinessRuleConditionValidateContext context)
		{
			CodeToAdd codeToAdd = context.Target as CodeToAdd;
			if (codeToAdd.ChangedExistingCodes != null)
			{
				if (codeToAdd.ChangedExistingCodes.Any())
				{
					context.Message = string.Format("Can not add codes ({0}) because they already exist in zone '{1}'.", codeToAdd.Code, codeToAdd.ChangedExistingCodes.First().ParentZone.Name);
					return false;
				}
			}

			return true;
		}

		public override string GetMessage(IRuleTarget target)
		{
			throw new NotImplementedException();
		}
	}
}