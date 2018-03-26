using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Data;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.WhS.Deal.BP.Activities
{
	public class PrepareDealZoneRatesForDBApplyInput
	{
		public BaseQueue<DealZoneRateBatch> InputQueue { get; set; }
		public BaseQueue<Object> OutputQueue { get; set; }

	}

	public sealed class PrepareDealZoneRatesForApply: DependentAsyncActivity<PrepareDealZoneRatesForDBApplyInput>
	{
		[RequiredArgument]
		public InArgument<BaseQueue<DealZoneRateBatch>> InputQueue { get; set; }

		[RequiredArgument]
		public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

		protected override void DoWork(PrepareDealZoneRatesForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
		{
			IDealZoneRateDataManager dataManager = DealDataManagerFactory.GetDataManager<IDealZoneRateDataManager>();

			PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, DealZoneRateBatch => DealZoneRateBatch.DealZoneRates);

			handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Preparing deal zone rates for applying to database is done.");
		}

		protected override PrepareDealZoneRatesForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
		{
			return new PrepareDealZoneRatesForDBApplyInput()
			{
				InputQueue = this.InputQueue.Get(context),
				OutputQueue = this.OutputQueue.Get(context),
			};
		}

		protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
		{
			if (this.OutputQueue.Get(context) == null)
				this.OutputQueue.Set(context, new MemoryQueue<Object>());
			base.OnBeforeExecute(context, handle);
		}
	}
}
