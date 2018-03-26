using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
	public class InsertDaysToReprocessInput
	{
		public bool IsSale { get; set; }
		public Dictionary<int, HashSet<DateTime>> DayToReprocessByDealId { get; set; }
	}

	public sealed class InsertDaysToReprocess : BaseAsyncActivity<InsertDaysToReprocessInput>
	{
		[RequiredArgument]
		public InArgument<bool> IsSale { get; set; }
		
		[RequiredArgument]
		public InArgument<Dictionary<int, HashSet<DateTime>>> DayToReprocessByDealId { get; set; }

		protected override void DoWork(InsertDaysToReprocessInput inputArgument, AsyncActivityHandle handle)
		{
			var dayToReprocessByDealId = inputArgument.DayToReprocessByDealId;
			if (dayToReprocessByDealId == null)
				return;

			bool isSale = inputArgument.IsSale;
			var dealDefinitionManager = new DealDefinitionManager();
			var daysToReprocessManager = new DaysToReprocessManager();

			foreach (var dealDaysToReprocess in dayToReprocessByDealId)
			{
				var deal = dealDefinitionManager.GetDeal(dealDaysToReprocess.Key);
				if (deal == null)
					throw new DataIntegrityValidationException(String.Format("Deal '{0}' does not exist", dealDaysToReprocess.Key));

				int carrierAccountId = deal.Settings.GetCarrierAccountId();

				foreach (var dayToReprocess in dealDaysToReprocess.Value)
				{
					int dayToReprocessId;
					daysToReprocessManager.Insert(dayToReprocess, isSale, carrierAccountId, out dayToReprocessId);
				}
			}

			handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Inserting days to reprocess is done.");
		}

		protected override InsertDaysToReprocessInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return new InsertDaysToReprocessInput
			{
				IsSale = this.IsSale.Get(context),
				DayToReprocessByDealId = this.DayToReprocessByDealId.Get(context)
			};
		}
	}
}
