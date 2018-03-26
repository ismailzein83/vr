using System;
using System.Activities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
	public sealed class FinalizeDealEvaluator : CodeActivity
	{
		protected override void Execute(CodeActivityContext context)
		{
			DealProgressManager dealProgressManager = new DealProgressManager();
			dealProgressManager.DeleteAffectedDealZoneGroups();
			var daysToReprocessManager = new DaysToReprocessManager();
			daysToReprocessManager.DeleteDaysToReprocess();
		}
	}
}
