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
            if (uploadPriceListTaskActionArgument.SupplierPriceListConnector == null)
                throw new Exception("SupplierPriceListConnector is null");
            ImportPriceListManager manager = new ImportPriceListManager();
            List<PriceListStatus> listPriceListStatuses = new List<PriceListStatus>()
                    {
                        PriceListStatus.New
                    };
            VRFileManager fileManager = new VRFileManager();
            foreach (var pricelist in manager.GetPriceLists(listPriceListStatuses))
            {
                var priceListUploadContext = new PriceListUploadContext
                {
                    UserId = pricelist.UserId,
                    PriceListType = pricelist.PriceListType.ToString(),
                    File = fileManager.GetFile(pricelist.FileId),
                    EffectiveOnDateTime = pricelist.EffectiveOnDate
                };
                PriceListUploadOutput priceListUploadOutput = new PriceListUploadOutput();
                try
                {
                    priceListUploadOutput =
                      uploadPriceListTaskActionArgument.SupplierPriceListConnector.PriceListUploadOutput(priceListUploadContext);
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
                        priceListstatus = PriceListStatus.Uploaded;
                        break;
                    case PriceListSupplierUploadResult.Failed:
                        priceListstatus = PriceListStatus.GetStatusFailedWithNoRetry;
                        break;
                    case PriceListSupplierUploadResult.FailedWithRetry:
                        priceListstatus = PriceListStatus.GetStatusFailedWithRetry;

                        break;
                    default:
                        priceListstatus = PriceListStatus.GetStatusFailedWithRetry;
                        break;
                }
                manager.UpdateInitiatePriceList(pricelist.PriceListId, (int)priceListstatus, priceListUploadOutput.UploadPriceListInformation);
            }
            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }
    }
}
