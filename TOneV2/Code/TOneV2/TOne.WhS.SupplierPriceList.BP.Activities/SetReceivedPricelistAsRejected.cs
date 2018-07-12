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
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{
    public class SetReceivedPricelistAsRejected : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> ReceivedPricelistRecordId { get; set; }

        [RequiredArgument]
        public InArgument<List<BPValidationMessage>> ValidationWarningMessages { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int receivedPricelistRecordId = this.ReceivedPricelistRecordId.Get(context);
            List<BPValidationMessage> validationWarningMessages = this.ValidationWarningMessages.Get(context);

            List<SPLImportErrorDetail> warningDetails = new List<SPLImportErrorDetail>();
            if (validationWarningMessages != null && validationWarningMessages.Any())
            {
                foreach (var message in validationWarningMessages)
                {
                    warningDetails.Add(new SPLImportErrorDetail() { ErrorMessage = message.Message });
                }
            }

            IReceivedPricelistManager manager = SupPLDataManagerFactory.GetDataManager<IReceivedPricelistManager>();
            manager.UpdateReceivedPricelistStatus(receivedPricelistRecordId, ReceivedPricelistStatus.Rejected, warningDetails);

            var receivedSupplierPricelistManager = new ReceivedSupplierPricelistManager();
            receivedSupplierPricelistManager.SendMailToSupplier(receivedPricelistRecordId, AutoImportEmailTypeEnum.Rejected);
        }
    }
}
