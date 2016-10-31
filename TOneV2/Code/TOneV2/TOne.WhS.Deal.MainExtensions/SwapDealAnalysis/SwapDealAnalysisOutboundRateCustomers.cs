using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions
{
	public class SwapDealAnalysisOutboundRateCustomers : SwapDealAnalysisOutboundRateCalcMethod
	{
		public override Guid ConfigId
		{
			get { return new Guid("91EC1ACA-5D9B-418D-A327-8EE699CE192F"); }
		}

		public override string ItemEditor
		{
			get { return "vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-customers-itemeditor"; }
		}
	}
}
