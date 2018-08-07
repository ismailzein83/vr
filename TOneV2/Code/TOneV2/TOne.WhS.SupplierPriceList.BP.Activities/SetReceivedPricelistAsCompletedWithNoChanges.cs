using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
	public class SetReceivedPricelistAsCompletedWithNoChanges : CodeActivity
	{
		[RequiredArgument]
		public InArgument<int?> ReceivedPricelistRecordId { get; set; }
		protected override void Execute(CodeActivityContext context)
		{
			int? receivedPricelistRecordId = this.ReceivedPricelistRecordId.Get(context);

			IReceivedPricelistManager manager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
			manager.UpdateReceivedPricelistStatus(receivedPricelistRecordId.Value, ReceivedPricelistStatus.CompletedWithNoChanges);

			var receivedSupplierPricelistManager = new ReceivedSupplierPricelistManager();
			receivedSupplierPricelistManager.SendMailToSupplier(receivedPricelistRecordId.Value, AutoImportEmailTypeEnum.Succeeded);
			receivedSupplierPricelistManager.SendMailToInternal(receivedPricelistRecordId.Value, AutoImportEmailTypeEnum.Succeeded);
		}
	}
}
