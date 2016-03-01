using CP.SupplierPricelist.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace CP.SupplierPricelist.Business.PriceListTasks
{
    public class ResultTaskAction : SchedulerTaskAction
    {
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            ResultTaskActionArgument resultTaskActionArgument = taskActionArgument as ResultTaskActionArgument;
            if (resultTaskActionArgument == null)
                throw new Exception("taskActionArgument  is not of type ResultTaskActionArgument ");
            if (resultTaskActionArgument.CustomerId == 0)
                throw new Exception("CustomerId is undefined");
            CustomerManager customerManager = new CustomerManager();
            Customer customer = customerManager.GetCustomer(resultTaskActionArgument.CustomerId);

            ImportPriceListManager manager = new ImportPriceListManager();
            List<PriceListStatus> listPriceListStatuses = new List<PriceListStatus>
            {
                PriceListStatus.Uploaded,
                PriceListStatus.GetStatusFailedWithRetry
            };
            foreach (var pricelist in manager.GetPriceLists(listPriceListStatuses, customer.CustomerId))
            {
                var priceListProgressContext = new PriceListProgressContext
                {
                    UploadInformation = pricelist.UploadedInformation
                };
                PriceListProgressOutput priceListProgressOutput = new PriceListProgressOutput();
                try
                {
                    priceListProgressOutput = customer.Settings.PriceListConnector.GetPriceListProgressOutput(priceListProgressContext);
                }
                catch (Exception)
                {
                    priceListProgressOutput.PriceListProgress = PriceListProgressResult.FailedWithRetry;
                    priceListProgressOutput.PriceListResult = PriceListResult.NotCompleted;
                }

                if (pricelist.Status != priceListProgressOutput.PriceListStatus &&
                    pricelist.Result != priceListProgressOutput.PriceListResult)
                {
                    switch (priceListProgressOutput.PriceListResult)
                    {
                        case PriceListResult.Approved:
                            break;
                        case PriceListResult.Completed:
                            break;
                        case PriceListResult.NotCompleted:
                            break;
                        case PriceListResult.PartiallyApproved:
                            break;
                        case PriceListResult.Rejected:
                            break;
                        case PriceListResult.WaitingReview:
                            break;
                    }
                    if (priceListProgressOutput.AlertFile != null)
                        pricelist.FileId = SaveLog(priceListProgressOutput.AlertFile, priceListProgressOutput.AlerFileName);
                    manager.UpdatePriceListProgress(pricelist.PriceListId, (int)priceListProgressOutput.PriceListStatus,
                  (int)priceListProgressOutput.PriceListResult, pricelist.ResultMaxRetryCount, pricelist.AlertMessage);
                }
                else
                {
                    priceListProgressOutput.PriceListProgress = PriceListProgressResult.ProgressNotChanged;
                }

                PriceListResult priceListResult = PriceListResult.NotCompleted;
                PriceListStatus priceListStatus = PriceListStatus.Uploaded;
                switch (priceListProgressOutput.PriceListProgress)
                {
                    case PriceListProgressResult.Completed:
                        break;
                    case PriceListProgressResult.FailedWithNoRetry:
                        priceListStatus = PriceListStatus.GetStatusFailedWithNoRetry;
                        priceListResult = PriceListResult.Rejected;
                        break;
                    case PriceListProgressResult.ProgressChanged:
                        break;
                    case PriceListProgressResult.ProgressNotChanged:
                        break;
                    case PriceListProgressResult.FailedWithRetry:
                        priceListStatus = PriceListStatus.Uploaded;
                        priceListResult = PriceListResult.Completed;
                        if (pricelist.ResultMaxRetryCount < resultTaskActionArgument.MaximumRetryCount)
                            pricelist.ResultMaxRetryCount = pricelist.UploadMaxRetryCount + 1;
                        else
                        {
                            priceListStatus = PriceListStatus.GetStatusFailedWithNoRetry;
                            priceListResult = PriceListResult.NotCompleted;
                        }
                        break;
                    default:
                        priceListStatus = PriceListStatus.GetStatusFailedWithNoRetry;
                        priceListResult = PriceListResult.NotCompleted;
                        break;
                }


            }
            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }

        private long SaveLog(byte[] contentBytes, string fileName)
        {
            string[] nameastab = fileName.Split('.');
            VRFile file = new VRFile
            {
                Content = contentBytes,
                Name = fileName,
                Extension = nameastab[nameastab.Length - 1],
                CreatedTime = DateTime.Now

            };
            VRFileManager manager = new VRFileManager();
            return manager.AddFile(file);
        }
    }
}
