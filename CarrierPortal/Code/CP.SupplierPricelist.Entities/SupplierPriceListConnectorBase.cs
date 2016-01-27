using System;
using System.Security.Cryptography.X509Certificates;
using Vanrise.Entities;

namespace CP.SupplierPricelist.Entities
{
    public abstract class SupplierPriceListConnectorBase
    {
        public int ConfigId { get; set; }
        public abstract InitiatePriceListOutput InitiatePriceList(IInitiateUploadContext context);
        public abstract PriceListProgressOutput GetPriceListProgressOutput(IPriceListProgressContext context);
    }
    public enum InitiateSupplierResult { Uploaded, Failed }
    public enum PriceListProgressResult { Received, Processing,AwaitingSaveConfirmationbySystemparam, SuspendedDueToBusinessErrors, SuspendedToProcessingErrors, AwaitingWarningsConfirmation, AwaitingSaveConfirmation, WarningsConfirmed, SaveConfirmed, ProcessedSuccessfuly, FailedDuetoSheetError,Rejected,SuspendedDueToConfigurationErrors,ProcessedSuccessfulyByImport,Processedwithnochanges, Failed }

    public class InitiatePriceListOutput
    {
        public int QueueId { get; set; }
        public InitiateSupplierResult Result { get; set; }

        public Object InitiateTestInformation { get; set; }

        public string FailureMessage { get; set; }
    }

    public interface IInitiateUploadContext
    {
        int UserId { get; }
        string PriceListType { get; }
        VRFile File { get; }
        DateTime EffectiveOnDateTime { get; }
    }
    public class PriceListProgressOutput
    {
        public PriceListProgressResult Result { get; set; }

        public Object TestProgress { get; set; }

        public PriceListResult PriceListResult { get; set; }
    }
    public interface IPriceListProgressContext
    {
        int QueueId { get; set; }
        Object InitiateTestInformation { get; }

        Object RecentTestProgress { get; }
    }

}
