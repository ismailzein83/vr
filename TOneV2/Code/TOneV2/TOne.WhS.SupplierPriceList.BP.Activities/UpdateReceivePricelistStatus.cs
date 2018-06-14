using System;
using System.Activities;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
	public class UpdateReceivePricelistStatus : CodeActivity
	{
		[RequiredArgument]
		public InArgument<int> ReceivedPricelistRecordId { get; set; }

		[RequiredArgument]
		public InArgument<ReceivedPricelistStatus> ReceivedPricelistStatus { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			int receivedPricelistRecordId = this.ReceivedPricelistRecordId.Get(context);
			ReceivedPricelistStatus receivedPricelistStatus = this.ReceivedPricelistStatus.Get(context);
			IReceivedPricelistManagerTemp manager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManagerTemp>();
			manager.UpdateReceivePricelistStatus(receivedPricelistRecordId, receivedPricelistStatus);
		}
	}
}