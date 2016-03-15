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
                PriceListStatus.SuccessfullyUploaded,
                PriceListStatus.ResultFailedWithRetry,
                PriceListStatus.WaitingReview,
                PriceListStatus.UnderProcessing
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
                    priceListProgressOutput = customer.Settings.PriceListConnector.GetPriceListResult(priceListProgressContext);
                }
                catch (Exception)
                {
                    priceListProgressOutput.PriceListStatus = PriceListStatus.ResultFailedWithRetry;
                    priceListProgressOutput.PriceListResult = PriceListResult.NotCompleted;
                }
                switch (priceListProgressOutput.PriceListStatus)
                {
                    case PriceListStatus.ResultFailedWithRetry:
                        if (pricelist.ResultMaxRetryCount < resultTaskActionArgument.MaximumRetryCount)
                            pricelist.ResultMaxRetryCount = pricelist.ResultMaxRetryCount + 1;
                        else
                        {
                            priceListProgressOutput.PriceListStatus = PriceListStatus.ResultFailedWithNoRetry;
                            priceListProgressOutput.PriceListResult = PriceListResult.Rejected;
                        }
                        break;
                }
                if (pricelist.Status != priceListProgressOutput.PriceListStatus ||
                    pricelist.Result != priceListProgressOutput.PriceListResult)
                {
                    if (priceListProgressOutput.AlertFile != null)
                        pricelist.AlertFileId = SaveLog(priceListProgressOutput.AlertFile, priceListProgressOutput.AlerFileName);
                    manager.UpdatePriceListProgress(pricelist.PriceListId, (int)priceListProgressOutput.PriceListStatus,
                  (int)priceListProgressOutput.PriceListResult, pricelist.ResultMaxRetryCount, priceListProgressOutput.AlertMessage, pricelist.AlertFileId);
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
