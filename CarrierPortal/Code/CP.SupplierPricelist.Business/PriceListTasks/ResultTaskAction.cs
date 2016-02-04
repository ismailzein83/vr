using CP.SupplierPricelist.Entities;
using System;
using System.Collections.Generic;
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
            if (resultTaskActionArgument.SupplierPriceListConnector == null)
                throw new Exception("SupplierPriceListConnector is null");

            ImportPriceListManager manager = new ImportPriceListManager();
            List<PriceListStatus> listPriceListStatuses = new List<PriceListStatus>
            {
                PriceListStatus.Uploaded
            };
            foreach (var pricelist in manager.GetPriceLists(listPriceListStatuses))
            {
                var priceListProgressContext = new PriceListProgressContext
                {
                    UploadInformation = pricelist.UploadedInformation
                };
                PriceListProgressOutput priceListProgressOutput = new PriceListProgressOutput();
                try
                {
                    priceListProgressOutput = resultTaskActionArgument.SupplierPriceListConnector.GetPriceListProgressOutput(priceListProgressContext);
                }
                catch (Exception)
                {
                    priceListProgressOutput.Result = PriceListProgressResult.Rejected;
                }
                manager.UpdatePriceListProgress(pricelist.PriceListId, (int)priceListProgressOutput.Result);
            }
            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }
    }
}
