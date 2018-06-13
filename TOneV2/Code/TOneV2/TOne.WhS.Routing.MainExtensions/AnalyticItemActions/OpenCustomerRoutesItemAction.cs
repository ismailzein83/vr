using System;
using Vanrise.Analytic.Entities;

namespace TOne.WhS.Routing.MainExtensions.AnalyticItemActions
{
	public class OpenCustomerRoutesItemAction : AnalyticItemAction
	{
		public override Guid ConfigId { get { return new Guid("2AEDD1D5-26D1-4C0E-A49C-CE5E8355C300"); } }

		public override void Execute(IAnalyticItemActionContext context)
		{
			throw new NotImplementedException();
		}
	}
}