using System;
using System.Linq;
using Vanrise.Entities;
using System.Activities;
using Vanrise.Common.Business;
using Vanrise.BusinessProcess;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.BP.Activities
{
    public class SendSalePricelistEmailInput
    {
        public IEnumerable<SalePricelistVRFile> SalePricelistVRFiles { get; set; }
        public SalePriceList CustomerPricelist { get; set; }
    }
    public class SendSalePricelistEmailOutput
    {
    }
    public sealed class SendSalePricelistEmail : BaseAsyncActivity<SendSalePricelistEmailInput, SendSalePricelistEmailOutput>
    {
        [RequiredArgument]
        public InArgument<SalePriceList> CustomerPricelist { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SalePricelistVRFile>> SalePricelistVRFiles { get; set; }

        protected override SendSalePricelistEmailOutput DoWorkWithResult(SendSalePricelistEmailInput inputArgument, AsyncActivityHandle handle)
        {
            var carrierAccountManager = new CarrierAccountManager();
            var fileManager = new VRFileManager();
            var salePriceListManager = new SalePriceListManager();
            var vrMailManager = new VRMailManager();
            List<VRMailAttachement> vrMailAttachements = new List<VRMailAttachement>();
            var evaluatedObject = salePriceListManager.EvaluateEmail(inputArgument.CustomerPricelist.PriceListId, SalePriceListType.Full);

            foreach (var SalePricelistVRFile in inputArgument.SalePricelistVRFiles)
            {
                VRFile customerPriceListFile = fileManager.GetFile(SalePricelistVRFile.FileId);
                CarrierAccount customer = carrierAccountManager.GetCarrierAccount(inputArgument.CustomerPricelist.OwnerId);
                vrMailAttachements.Add(new VRMailAttachmentExcel
                {
                    Name = customerPriceListFile.Name,
                    Content = customerPriceListFile.Content
                });
            }
            vrMailManager.SendMail(evaluatedObject.From, evaluatedObject.To, evaluatedObject.CC, evaluatedObject.BCC, evaluatedObject.Subject, evaluatedObject.Body
                , vrMailAttachements, false);

            return new SendSalePricelistEmailOutput();
        }

        protected override SendSalePricelistEmailInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new SendSalePricelistEmailInput
            {
                SalePricelistVRFiles = this.SalePricelistVRFiles.Get(context),
                CustomerPricelist = this.CustomerPricelist.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, SendSalePricelistEmailOutput result)
        {
        }

    }
}
