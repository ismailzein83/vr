using System;
using System.Activities;
using TOne.WhS.Deal.Business;

namespace TOne.WhS.Deal.BP.Activities
{
	public sealed class DeleteDaysToReprocessByDate : CodeActivity
	{
		[RequiredArgument]
		public InArgument<DateTime> DateToDelete { get; set; }
		protected override void Execute(CodeActivityContext context)
		{
			var date = DateToDelete.Get(context);
			new DaysToReprocessManager().DeleteDaysToReprocessByDate(date);
		}
	}
}