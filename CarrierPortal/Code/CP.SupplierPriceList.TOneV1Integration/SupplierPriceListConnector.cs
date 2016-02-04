using System;
using CP.SupplierPricelist.Entities;
using CP.SupplierPricelist.Business;

namespace CP.SupplierPriceList.TOneV1Integration
{
    public class SupplierPriceListConnector : SupplierPriceListConnectorBase
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public override PriceListUploadOutput PriceListUploadOutput(IPriceListUploadContext context)
        {
            PriceListUploadOutput priceListOutput = new PriceListUploadOutput();
            var cont = (PriceListUploadContext)context;
            ServiceActions serviceActions = new ServiceActions(Url, Password, UserName);
            RootObject tokenObject = serviceActions.GetAuthenticated();
            if (tokenObject != null)
            {
                SupplierPriceListUserInput userInput = new SupplierPriceListUserInput()
                {
                    UserId = cont.UserId,
                    PriceListType = cont.PriceListType,
                    FileName = cont.File.Name,
                    EffectiveOnDateTime = cont.EffectiveOnDateTime,
                    ContentFile = cont.File.Content
                };
                // Vanrise.Common.Compressor.Compress(context.File.Content)
                int insertedId = serviceActions.UploadOnline(tokenObject.Token, tokenObject.TokenName, userInput);
                var uploadInformation = new UploadInformation
                {
                    QueueId = insertedId
                };
                priceListOutput.UploadPriceListInformation = uploadInformation;
                priceListOutput.Result = insertedId != 0 ? PriceListSupplierUploadResult.Uploaded : PriceListSupplierUploadResult.Failed;
            }
            return priceListOutput;
        }
        public class SupplierPriceListUserInput
        {
            public int UserId { get; set; }
            public string PriceListType { get; set; }
            public byte[] ContentFile { get; set; }
            public string FileName { get; set; }
            public DateTime EffectiveOnDateTime { get; set; }
        }
        public override PriceListProgressOutput GetPriceListProgressOutput(IPriceListProgressContext context)
        {
            int queueResult = 0;
            PriceListProgressOutput priceListProgressOutput = new PriceListProgressOutput();
            int queueId = ((UploadInformation)((PriceListProgressContext)context).UploadInformation).QueueId;
            ServiceActions serviceActions = new ServiceActions(Url, Password, UserName);
            RootObject tokenObject = serviceActions.GetAuthenticated();

            if (tokenObject != null)
            {
                queueResult = serviceActions.GetResults(queueId, tokenObject.Token, tokenObject.TokenName);
            }
            switch (queueResult)
            {
                case 0: priceListProgressOutput.Result = PriceListProgressResult.Approved;
                    break;
                case 1: priceListProgressOutput.Result = PriceListProgressResult.Approved;
                    break;
                case 2: priceListProgressOutput.Result = PriceListProgressResult.Rejected;
                    break;
                case 3: priceListProgressOutput.Result = PriceListProgressResult.Rejected;
                    break;
                case 4: priceListProgressOutput.Result = PriceListProgressResult.PartiallyApproved;
                    break;
                case 5: priceListProgressOutput.Result = PriceListProgressResult.PartiallyApproved;
                    break;
                case 6: priceListProgressOutput.Result = PriceListProgressResult.PartiallyApproved;
                    break;
                case 7: priceListProgressOutput.Result = PriceListProgressResult.Approved;
                    break;
                case 8: priceListProgressOutput.Result = PriceListProgressResult.Approved;
                    break;
                case 9: priceListProgressOutput.Result = PriceListProgressResult.Rejected;
                    break;
                case 10: priceListProgressOutput.Result = PriceListProgressResult.Rejected;
                    break;
                case 11: priceListProgressOutput.Result = PriceListProgressResult.Rejected;
                    break;
                case 12: priceListProgressOutput.Result = PriceListProgressResult.Approved;
                    break;
                case 13: priceListProgressOutput.Result = PriceListProgressResult.PartiallyApproved;
                    break;
                case 14: priceListProgressOutput.Result = PriceListProgressResult.Approved;
                    break;
                default: priceListProgressOutput.Result = PriceListProgressResult.Rejected;
                    break;
            }
            return priceListProgressOutput;
        }
    }
}
