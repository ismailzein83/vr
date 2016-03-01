using CP.SupplierPricelist.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Runtime.Entities;

namespace CP.SupplierPricelist.Business.PriceListTasks
{
    public class UploadPriceListTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            UploadPriceListTaskActionArgument uploadPriceListTaskActionArgument = taskActionArgument as UploadPriceListTaskActionArgument;
            if (uploadPriceListTaskActionArgument == null)
                throw new Exception("taskActionArgument  is not of type UploadPriceListTaskActionArgument ");
            if (uploadPriceListTaskActionArgument.CustomerId == 0)
                throw new Exception("customerID is undefined");
            CustomerManager customerManager = new CustomerManager();
            Customer customer = customerManager.GetCustomer(uploadPriceListTaskActionArgument.CustomerId);

            ImportPriceListManager manager = new ImportPriceListManager();
            List<PriceListStatus> listPriceListStatuses = new List<PriceListStatus>
                    {
                        PriceListStatus.New,
                        PriceListStatus.UploadFailedWithRetry
                    };
            VRFileManager fileManager = new VRFileManager();
            foreach (var pricelist in manager.GetPriceLists(listPriceListStatuses, customer.CustomerId))
            {
                var priceListUploadContext = new PriceListUploadContext
                {
                    UserId = pricelist.UserId,
                    PriceListType = pricelist.PriceListType.ToString(),
                    File = fileManager.GetFile(pricelist.FileId),
                    EffectiveOnDateTime = pricelist.EffectiveOnDate,
                    CarrierAccountId = pricelist.CarrierAccountId
                };
                PriceListUploadOutput priceListUploadOutput = new PriceListUploadOutput();
                try
                {
                    priceListUploadOutput =
                      customer.Settings.PriceListConnector.PriceListUploadOutput(priceListUploadContext);
                }
                catch (Exception ex)
                {
                    priceListUploadOutput.Result = PriceListSupplierUploadResult.FailedWithRetry;
                    priceListUploadOutput.FailureMessage = ex.Message;
                }
                PriceListStatus priceListstatus;
                switch (priceListUploadOutput.Result)
                {
                    case PriceListSupplierUploadResult.Uploaded:
                        priceListstatus = PriceListStatus.SuccessfullyImported;
                        break;
                    case PriceListSupplierUploadResult.Failed:
                        priceListstatus = PriceListStatus.GetStatusFailedWithNoRetry;
                        break;
                    case PriceListSupplierUploadResult.FailedWithRetry:
                        {
                            priceListstatus = PriceListStatus.UploadFailedWithRetry;
                            if (pricelist.UploadMaxRetryCount < uploadPriceListTaskActionArgument.MaximumRetryCount)
                                pricelist.UploadMaxRetryCount = pricelist.UploadMaxRetryCount + 1;
                            else
                                priceListstatus = PriceListStatus.UploadFailedWithNoRetry;
                            break;
                        }
                    default:
                        priceListstatus = PriceListStatus.GetStatusFailedWithRetry;
                        break;
                }
                manager.UpdatePriceListUpload(pricelist.PriceListId, (int)priceListstatus, priceListUploadOutput.UploadPriceListInformation, (int)pricelist.UploadMaxRetryCount);
            }
            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }
    }
}
