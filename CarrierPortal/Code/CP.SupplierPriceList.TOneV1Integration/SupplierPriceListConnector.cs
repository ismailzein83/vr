using System;
using CP.SupplierPricelist.Entities;
using CP.SupplierPricelist.Business;
using Vanrise.Entities;
using System.Collections.Generic;

namespace CP.SupplierPriceList.TOneV1Integration
{
    public class SupplierPriceListConnector : SupplierPriceListConnectorBase
    {
        public override Guid ConfigId { get { return new Guid("fce278ec-410e-4c92-85e0-a9f2e4bb27a8"); } }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public override PriceListUploadOutput UploadPriceList(IPriceListUploadContext context)
        {
            PriceListUploadOutput priceListOutput = new PriceListUploadOutput();
            var cont = (PriceListUploadContext)context;
            ServiceActions serviceActions = new ServiceActions(Url, Password, UserName);
            SupplierPriceListUserInput userInput = new SupplierPriceListUserInput
            {
                UserId = cont.UserId,
                PriceListType = cont.PriceListType,
                FileName = cont.File.Name,
                EffectiveOnDateTime = cont.EffectiveOnDateTime,
                ContentFile = cont.File.Content,
                CarrierAccountId = cont.CarrierAccountId,
                UserEmail = cont.UserMail
            };
            // Vanrise.Common.Compressor.Compress(context.File.Content)
            int insertedId = serviceActions.UploadOnline(userInput);
            var uploadInformation = new UploadInformation
            {
                QueueId = insertedId
            };
            priceListOutput.UploadPriceListInformation = uploadInformation;
            priceListOutput.Result = insertedId != 0 ? PriceListSupplierUploadResult.Uploaded : PriceListSupplierUploadResult.Failed;

            return priceListOutput;
        }
        public class SupplierPriceListUserInput
        {
            public string CarrierAccountId { get; set; }
            public int UserId { get; set; }
            public string PriceListType { get; set; }
            public byte[] ContentFile { get; set; }
            public string FileName { get; set; }
            public DateTime EffectiveOnDateTime { get; set; }
            public string UserEmail { get; set; }
        }

        private void UpdateRejectedPricelistOutput(PriceListProgressOutput priceListProgressOutput, string message, UploadInfo uploadInformation)
        {
            priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
            priceListProgressOutput.PriceListResult = PriceListResult.Rejected;
            priceListProgressOutput.AlertMessage = message;
            priceListProgressOutput.AlertFile = uploadInformation.ContentBytes;
            priceListProgressOutput.AlerFileName = uploadInformation.FileName;
        }
        public override PriceListProgressOutput GetPriceListResult(IPriceListProgressContext context)
        {
            PriceListProgressOutput priceListProgressOutput = new PriceListProgressOutput();
            int queueId = ((UploadInformation)((PriceListProgressContext)context).UploadInformation).QueueId;
            ServiceActions serviceActions = new ServiceActions(Url, Password, UserName);
            var uploadInformation = serviceActions.GetUploadInfo(queueId);

            priceListProgressOutput.PriceListResult = PriceListResult.NotCompleted;
            var queueResult = (QueueItemStatus)uploadInformation.Status;
            priceListProgressOutput.UploadProgress = uploadInformation;

            switch (queueResult)
            {
                case QueueItemStatus.Recieved:
                case QueueItemStatus.WarningsConfirmed:
                case QueueItemStatus.SaveConfirmed:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.SuccessfullyUploaded;
                    priceListProgressOutput.PriceListResult = PriceListResult.NotCompleted;
                    break;

                case QueueItemStatus.PartiallyRejected:
                    priceListProgressOutput.AlertMessage = "Due To Retro-active";
                    priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                    priceListProgressOutput.PriceListResult = PriceListResult.PartiallyRejected;
                    priceListProgressOutput.AlertFile = uploadInformation.ContentBytes;
                    priceListProgressOutput.AlerFileName = uploadInformation.FileName;
                    break;

                case QueueItemStatus.Processing:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.UnderProcessing;
                    priceListProgressOutput.PriceListResult = PriceListResult.NotCompleted;
                    break;

                case QueueItemStatus.AwaitingWarningsConfirmation:
                case QueueItemStatus.AwaitingSaveConfirmation:
                case QueueItemStatus.AwaitingSaveConfirmationbySystemparam:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.WaitingReview;
                    priceListProgressOutput.PriceListResult = PriceListResult.NotCompleted;
                    break;

                case QueueItemStatus.ProcessedSuccessfuly:
                case QueueItemStatus.ProcessedSuccessfulyByImport:
                case QueueItemStatus.Processedwithnochanges:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                    priceListProgressOutput.PriceListResult = PriceListResult.Imported;
                    break;

                case QueueItemStatus.FailedDuetoSheetError:
                    UpdateRejectedPricelistOutput(priceListProgressOutput, "Failed due to sheet error", uploadInformation);
                    break;
                case QueueItemStatus.Rejected:
                    UpdateRejectedPricelistOutput(priceListProgressOutput, "Rejected", uploadInformation);
                    break;
                case QueueItemStatus.SuspendedDueToConfigurationErrors:
                    UpdateRejectedPricelistOutput(priceListProgressOutput, "Suspended due to configurtaion errors", uploadInformation);
                    break;

                case QueueItemStatus.SuspendedDueToBusinessErrors:
                    UpdateRejectedPricelistOutput(priceListProgressOutput, "Suspended due to business errors", uploadInformation);
                    break;
                case QueueItemStatus.SuspendedToProcessingErrors:
                    UpdateRejectedPricelistOutput(priceListProgressOutput, "Suspended due to processing errors", uploadInformation);
                    break;
                default:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                    priceListProgressOutput.PriceListResult = PriceListResult.Rejected;
                    break;
            }
            return priceListProgressOutput;
        }

        public override List<SupplierInfo> GetSuppliers(GetSuppliersContext context)
        {
            ServiceActions serviceActions = new ServiceActions(Url, Password, UserName);
            var list = serviceActions.GetCarriersInfos();
            List<SupplierInfo> supplierInfosList = new List<SupplierInfo>();
            foreach (var a in list)
            {
                supplierInfosList.Add(
                    new SupplierInfo()
                    {
                        SupplierId = a.CarrierAccountID,
                        SupplierName = a.Name
                    }
                );
            }

            return supplierInfosList;
        }

    }
}
