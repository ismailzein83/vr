using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions
{
	public class SwapDealAnalysisInboundRateSuppliers : SwapDealAnalysisInboundRateCalcMethod
	{
		public override Guid ConfigId
		{
			get { return new Guid("96B9EB7E-D08B-4CCB-AC0D-7FE112EF41D8"); }
		}

		public override string ItemEditor
		{
			get { return "vr-whs-deal-swapdealanalysis-ratecalcmethod-inbound-suppliers-itemeditor"; }
		}
	}
}
