using System;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
	public class SetReceivedPricelistAsStarted: CodeActivity
	{
		[RequiredArgument]
		public InArgument<int?> ReceivedPricelistRecordId { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			int? receivedPricelistRecordId = this.ReceivedPricelistRecordId.Get(context);
			long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
			IReceivedPricelistManager manager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();

			manager.SetReceivedPricelistAsStarted(receivedPricelistRecordId.Value, ReceivedPricelistStatus.Processing, processInstanceId, DateTime.Now);
		}
	}
}
