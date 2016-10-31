using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions
{
	public class SwapDealAnalysisOutboundRateSuppliers : SwapDealAnalysisOutboundRateCalcMethod
	{
		public override Guid ConfigId
		{
			get { return new Guid("2BBA4C50-CE06-4F61-9731-D31CE687CABF"); }
		}

		public override string ItemEditor
		{
			get { return "vr-whs-deal-swapdealanalysis-ratecalcmethod-outbound-suppliers-itemeditor"; }
		}
	}
}
