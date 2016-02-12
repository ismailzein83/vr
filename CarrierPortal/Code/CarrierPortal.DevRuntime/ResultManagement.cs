using CP.SupplierPricelist.Business;
using CP.SupplierPricelist.Entities;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CarrierPortal.DevRuntime
{
    public class ResultManagement
    {
        private static MainForm _form;
        private static readonly object Syncroot = new object();
        private static bool _locked;

        public void Start(MainForm form)
        {
            //  ResultManagement._form = form;
            //  Thread thread = new Thread(GetResult) { IsBackground = true };
            //  thread.Start();
            GetResult();
        }
        private SupplierPriceListConnectorBase _supplierPriceListConnector =
           new CP.SupplierPriceList.TOneV1Integration.SupplierPriceListConnector();

        private void GetResult()
        {
            try
            {
                while (_locked != true)
                {
                    _locked = true;
                    lock (Syncroot)
                    {
                        ImportPriceListManager manager = new ImportPriceListManager();
                        List<PriceListStatus> listPriceListStatuses = new List<PriceListStatus>()
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
                                priceListProgressOutput = _supplierPriceListConnector.GetPriceListProgressOutput(priceListProgressContext);
                            }
                            catch (Exception)
                            {
                                priceListProgressOutput.PriceListProgress = PriceListProgressResult.FailedWithRetry;
                                priceListProgressOutput.PriceListResult = PriceListResult.NotCompleted;
                            }
                            manager.UpdatePriceListProgress(pricelist.PriceListId,
                                (int)priceListProgressOutput.PriceListProgress,
                                (int)priceListProgressOutput.PriceListResult,
                                (int)pricelist.ResultMaxRetryCount,
                                pricelist.AlertMessage);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
