using System;
using CP.SupplierPricelist.Entities;
using CP.SupplierPricelist.Business;

namespace CP.SupplierPriceList.TOneV1Integration
{
    public class SupplierPriceListConnector : SupplierPriceListConnectorBase
    {
        public override InitiatePriceListOutput InitiatePriceList(IInitiateUploadContext context)
        {
            InitiatePriceListOutput priceListOutput = new InitiatePriceListOutput();
            var cont = (InitiateUploadContext)context;
            ServiceActions serviceActions = new ServiceActions(cont.Url, cont.Password, cont.UserName);
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
                priceListOutput.QueueId = insertedId;
                priceListOutput.Result = insertedId != 0 ? InitiateSupplierResult.Uploaded : InitiateSupplierResult.Failed;
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
            var progressContext = (PriceListProgressContext)context;
            ServiceActions serviceActions = new ServiceActions(progressContext.Url, progressContext.Password, progressContext.UserName);
            RootObject tokenObject = serviceActions.GetAuthenticated();
            if (tokenObject != null)
            {
                queueResult = serviceActions.GetResults(progressContext.QueueId, tokenObject.Token, tokenObject.TokenName);
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
