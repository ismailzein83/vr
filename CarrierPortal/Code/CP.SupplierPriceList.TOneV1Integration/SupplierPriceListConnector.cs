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
            ServiceActions serviceActions = new ServiceActions();
            RootObject tokenObject = serviceActions.GetAuthenticated();
            if (tokenObject != null)
            {
                queueResult = serviceActions.GetResults(context.QueueId, tokenObject.Token, tokenObject.TokenName);
            }
            switch (queueResult)
            {
                case 0: priceListProgressOutput.Result = PriceListProgressResult.Received;
                    break;
                case 1: priceListProgressOutput.Result = PriceListProgressResult.Processing;
                    break;
                case 2: priceListProgressOutput.Result = PriceListProgressResult.SuspendedDueToBusinessErrors;
                    break;
                case 3: priceListProgressOutput.Result = PriceListProgressResult.SuspendedToProcessingErrors;
                    break;
                case 4: priceListProgressOutput.Result = PriceListProgressResult.AwaitingWarningsConfirmation;
                    break;
                case 5: priceListProgressOutput.Result = PriceListProgressResult.AwaitingSaveConfirmation;
                    break;
                case 6: priceListProgressOutput.Result = PriceListProgressResult.WarningsConfirmed;
                    break;
                case 7: priceListProgressOutput.Result = PriceListProgressResult.SaveConfirmed;
                    break;
                case 8: priceListProgressOutput.Result = PriceListProgressResult.ProcessedSuccessfuly;
                    break;
                case 9: priceListProgressOutput.Result = PriceListProgressResult.FailedDuetoSheetError;
                    break;
                case 10: priceListProgressOutput.Result = PriceListProgressResult.Rejected;
                    break;
                case 11: priceListProgressOutput.Result = PriceListProgressResult.SuspendedDueToConfigurationErrors;
                    break;
                case 12: priceListProgressOutput.Result = PriceListProgressResult.ProcessedSuccessfulyByImport;
                    break;
                case 13: priceListProgressOutput.Result = PriceListProgressResult.AwaitingSaveConfirmationbySystemparam;
                    break;
                case 14: priceListProgressOutput.Result = PriceListProgressResult.Processedwithnochanges;
                    break;
                default: priceListProgressOutput.Result = PriceListProgressResult.Failed;
                    break;
            }
            return priceListProgressOutput;
        }
    }
}
