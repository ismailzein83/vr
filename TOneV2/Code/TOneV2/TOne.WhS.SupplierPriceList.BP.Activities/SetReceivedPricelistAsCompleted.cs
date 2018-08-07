using System;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
	public class SetReceivedPricelistAsCompleted : CodeActivity
	{
		[RequiredArgument]
		public InArgument<int?> ReceivedPricelistRecordId { get; set; }

		[RequiredArgument]
		public InArgument<int> PricelistId { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			int? receivedPricelistRecordId = this.ReceivedPricelistRecordId.Get(context);
			int pricelistId = this.PricelistId.Get(context);
			IReceivedPricelistManager manager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();

			manager.SetReceivedPricelistAsCompleted(receivedPricelistRecordId.Value, ReceivedPricelistStatus.Succeeded, pricelistId);

			var receivedSupplierPricelistManager = new ReceivedSupplierPricelistManager();
            receivedSupplierPricelistManager.SendMailToSupplier(receivedPricelistRecordId.Value, AutoImportEmailTypeEnum.Succeeded);
            receivedSupplierPricelistManager.SendMailToInternal(receivedPricelistRecordId.Value, AutoImportEmailTypeEnum.Succeeded);
		}
	}
}
