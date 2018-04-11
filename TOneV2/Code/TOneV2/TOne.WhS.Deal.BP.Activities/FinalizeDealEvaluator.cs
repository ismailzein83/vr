using System;
using System.Activities;
using TOne.WhS.Deal.Business;

namespace TOne.WhS.Deal.BP.Activities
{
	public sealed class FinalizeDealEvaluator : CodeActivity
	{
		protected override void Execute(CodeActivityContext context)
		{
			DealProgressManager dealProgressManager = new DealProgressManager();
			dealProgressManager.DeleteAffectedDealZoneGroups();
		}
	}
}