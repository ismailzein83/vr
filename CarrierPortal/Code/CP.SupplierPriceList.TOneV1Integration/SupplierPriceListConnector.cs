using System;
using CP.SupplierPricelist.Entities;

namespace CP.SupplierPriceList.TOneV1Integration
{
    public class SupplierPriceListConnector : SupplierPriceListConnectorBase
    {
        public override InitiatePriceListOutput InitiatePriceList(IInitiateUploadContext context)
        {
            InitiatePriceListOutput priceListOutput = new InitiatePriceListOutput();
            ServiceActions serviceActions = new ServiceActions();
            RootObject tokenObject = serviceActions.GetAuthenticated();
            if (tokenObject != null)
            {
                SupplierPriceListUserInput userInput = new SupplierPriceListUserInput()
                {
                    UserId = context.UserId,
                    PriceListType = context.PriceListType,
                    FileName = context.File.Name,
                    EffectiveOnDateTime = context.EffectiveOnDateTime,
                    ContentFile = context.File.Content
                };
                // Vanrise.Common.Compressor.Compress(context.File.Content)
                serviceActions.UploadOnline(tokenObject.Token, tokenObject.TokenName, userInput);
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
            throw new NotImplementedException();
        }
    }
}
