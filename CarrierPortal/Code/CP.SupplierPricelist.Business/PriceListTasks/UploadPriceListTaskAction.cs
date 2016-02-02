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
                var initiateUploadContext = new InitiateUploadContext()
                {
                    UserId = pricelist.UserId,
                    PriceListType = pricelist.PriceListType.ToString(),
                    File = fileManager.GetFile(pricelist.FileId),
                    EffectiveOnDateTime = pricelist.EffectiveOnDate,
                    Url = uploadPriceListTaskActionArgument.SupplierPriceListConnector.Url,
                    UserName = uploadPriceListTaskActionArgument.SupplierPriceListConnector.Username,
                    Password = uploadPriceListTaskActionArgument.SupplierPriceListConnector.Password
                };
                InitiatePriceListOutput initiatePriceListOutput = new InitiatePriceListOutput();
                try
                {
                    initiatePriceListOutput =
                      uploadPriceListTaskActionArgument.SupplierPriceListConnector.InitiatePriceList(initiateUploadContext);
                }
                catch (Exception ex)
                {
                    initiatePriceListOutput.Result = InitiateSupplierResult.Failed;
                    initiatePriceListOutput.FailureMessage = ex.Message;
                }
                PriceListStatus priceListstatus;
                switch (initiatePriceListOutput.Result)
                {
                    case InitiateSupplierResult.Uploaded:
                        priceListstatus = PriceListStatus.Uploaded;
                        break;
                    case InitiateSupplierResult.Failed:
                        priceListstatus = PriceListStatus.GetStatusFailedWithNoRetry;
                        break;
                    default:
                        priceListstatus = PriceListStatus.GetStatusFailedWithRetry;
                        break;
                }
                manager.UpdateInitiatePriceList(pricelist.PriceListId, (int)priceListstatus, initiatePriceListOutput.QueueId);
            }
            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }
    }
}
