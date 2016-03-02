using System;
using CP.SupplierPricelist.Entities;
using CP.SupplierPricelist.Business;
using Vanrise.Entities;
using System.Collections.Generic;

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
        public override PriceListProgressOutput GetPriceListProgressOutput(IPriceListProgressContext context)
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
                case QueueItemStatus.PartiallyRejected:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                    priceListProgressOutput.PriceListResult = PriceListResult.PartiallyRejected;
                    priceListProgressOutput.AlertMessage = "Due To Retro-active";
                    priceListProgressOutput.AlertFile = uploadInformation.ContentBytes;
                    priceListProgressOutput.AlerFileName = uploadInformation.FileName;
                    break;
                case QueueItemStatus.Recieved:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                    break;
                case QueueItemStatus.Processing:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                    break;
                case QueueItemStatus.SuspendedDueToBusinessErrors:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                    priceListProgressOutput.PriceListResult = PriceListResult.Rejected;
                    priceListProgressOutput.AlertMessage = "Suspended due to business errors";
                    priceListProgressOutput.AlertFile = uploadInformation.ContentBytes;
                    priceListProgressOutput.AlerFileName = uploadInformation.FileName;
                    break;
                case QueueItemStatus.SuspendedToProcessingErrors:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                    priceListProgressOutput.PriceListResult = PriceListResult.Rejected;
                    priceListProgressOutput.AlertMessage = "Suspended due to processing errors";
                    priceListProgressOutput.AlertFile = uploadInformation.ContentBytes;
                    priceListProgressOutput.AlerFileName = uploadInformation.FileName;
                    break;
                case QueueItemStatus.AwaitingWarningsConfirmation:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.WaitingReview;
                    priceListProgressOutput.PriceListResult = PriceListResult.NotCompleted;
                    break;
                case QueueItemStatus.AwaitingSaveConfirmation:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.WaitingReview;
                    priceListProgressOutput.PriceListResult = PriceListResult.NotCompleted;
                    break;
                case QueueItemStatus.WarningsConfirmed:
                    priceListProgressOutput.PriceListStatus = PriceListStatus.WaitingReview;
                    break;
                case QueueItemStatus.SaveConfirmed:
                    {
                        priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                        priceListProgressOutput.PriceListResult = PriceListResult.Approved;
                        break;
                    }
                case QueueItemStatus.ProcessedSuccessfuly:
                    {
                        priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                        priceListProgressOutput.PriceListResult = PriceListResult.Approved;
                        break;
                    }
                case QueueItemStatus.FailedDuetoSheetError:
                    {
                        priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                        priceListProgressOutput.PriceListResult = PriceListResult.Rejected;
                        priceListProgressOutput.AlertMessage = "Failed due to sheet error";
                        priceListProgressOutput.AlertFile = uploadInformation.ContentBytes;
                        priceListProgressOutput.AlerFileName = uploadInformation.FileName;
                        break;
                    }
                case QueueItemStatus.Rejected:
                    {
                        priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                        priceListProgressOutput.PriceListResult = PriceListResult.Rejected;
                        priceListProgressOutput.AlertMessage = "Rejected";
                        priceListProgressOutput.AlertFile = uploadInformation.ContentBytes;
                        priceListProgressOutput.AlerFileName = uploadInformation.FileName;
                        break;
                    }
                case QueueItemStatus.SuspendedDueToConfigurationErrors:
                    {
                        priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                        priceListProgressOutput.PriceListResult = PriceListResult.Rejected;
                        priceListProgressOutput.AlertMessage = "Suspended due to configurtaion errors";
                        priceListProgressOutput.AlertFile = uploadInformation.ContentBytes;
                        priceListProgressOutput.AlerFileName = uploadInformation.FileName;
                        break;
                    }
                case QueueItemStatus.ProcessedSuccessfulyByImport:
                    {
                        priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                        priceListProgressOutput.PriceListResult = PriceListResult.Approved;
                        break;
                    }
                case QueueItemStatus.AwaitingSaveConfirmationbySystemparam:
                    {

                        priceListProgressOutput.PriceListStatus = PriceListStatus.WaitingReview;
                        priceListProgressOutput.PriceListResult = PriceListResult.NotCompleted;
                        break;
                    }
                case QueueItemStatus.Processedwithnochanges:
                    {
                        priceListProgressOutput.PriceListStatus = PriceListStatus.Completed;
                        priceListProgressOutput.PriceListResult = PriceListResult.Approved;
                        break;
                    }
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
