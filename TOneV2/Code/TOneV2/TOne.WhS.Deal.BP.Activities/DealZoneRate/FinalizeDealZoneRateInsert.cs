using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Deal.BP.Activities
{
	public class FinalizeDealZoneRateInsertInput
	{
	}

	public sealed class FinalizeDealZoneRateInsert : BaseAsyncActivity<FinalizeDealZoneRateInsertInput>
	{
		protected override void DoWork(FinalizeDealZoneRateInsertInput inputArgument, AsyncActivityHandle handle)
		{
			var dealZoneRateManager = new DealZoneRateManager();
			dealZoneRateManager.FinalizeDealZoneRateInsert();

			handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finalizing deal zone rates insert is done.");
		}

		protected override FinalizeDealZoneRateInsertInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return null;
		}
	}
}
