using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
	public class UpdateReceivePricelistStatusAsFailed : CodeActivity
	{
		[RequiredArgument]
		public InArgument<int?> ReceivedPricelistRecordId { get; set; }

		[RequiredArgument]
		public InArgument<ReceivedPricelistStatus> ReceivedPricelistStatus { get; set; }

		[RequiredArgument]
		public InArgument<List<BPValidationMessage>> ValidationErrorMessages { get; set; }

		protected override void Execute(CodeActivityContext context)
		{
			int? receivedPricelistRecordId = this.ReceivedPricelistRecordId.Get(context);
			ReceivedPricelistStatus receivedPricelistStatus = this.ReceivedPricelistStatus.Get(context);
			List<BPValidationMessage> validationErrorMessages = this.ValidationErrorMessages.Get(context);
			var validationMessageDataManager = new BPValidationMessageManager();
			long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;

			List<SPLImportErrorDetail> errorDetails = new List<SPLImportErrorDetail>();
			if (validationErrorMessages != null && validationErrorMessages.Any())
			{
				foreach (var message in validationErrorMessages)
				{
					errorDetails.Add(new SPLImportErrorDetail() { Message = message.Message });
				}
			}
			IReceivedPricelistManager manager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
			manager.UpdateReceivedPricelistStatus(receivedPricelistRecordId.Value, receivedPricelistStatus, errorDetails);

			var receivedSupplierPricelistManager = new ReceivedSupplierPricelistManager();
			receivedSupplierPricelistManager.SendMailToSupplier(receivedPricelistRecordId.Value, AutoImportEmailTypeEnum.Failed);
		}
	}
}