using CP.SupplierPricelist.Business;
using CP.SupplierPricelist.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using Vanrise.Common.Business;

namespace CarrierPortal.DevRuntime
{
    public class UploadPriceList
    {
        private static bool _locked;
        private static MainForm _form1;
        private static readonly object SyncRoot = new object();

        private SupplierPriceListConnectorBase SupplierPriceListConnector =
            new CP.SupplierPriceList.TOneV1Integration.SupplierPriceListConnector();

        public void Start(MainForm f)
        {
            _locked = false;
            //UploadPriceList._form1 = f;
            //Thread thread = new Thread(new ThreadStart(UploadPriceListToAPI));
            //thread.IsBackground = true;
            //thread.Start();
            UploadPriceListToAPI();
        }
        private void UploadPriceListToAPI()
        {
            try
            {
                while (_locked != true)
                {
                    ImportPriceListManager manager = new ImportPriceListManager();
                    List<PriceListStatus> listPriceListStatuses = new List<PriceListStatus>()
                    {
                        PriceListStatus.Recieved
                    };
                    _locked = true;
                    lock (SyncRoot)
                    {
                        VRFileManager fileManager = new VRFileManager();
                        foreach (var pricelist in manager.GetPriceLists(listPriceListStatuses))
                        {
                            var initiateUploadContext = new InitiateUploadContext()
                            {
                                UserId = pricelist.UserId,
                                PriceListType = pricelist.PriceListType.ToString(),
                                File = fileManager.GetFile(pricelist.FileId),
                                EffectiveOnDateTime = pricelist.EffectiveOnDate
                            };
                            InitiatePriceListOutput initiatePriceListOutput = new InitiatePriceListOutput();
                            try
                            {
                                initiatePriceListOutput =
                                    SupplierPriceListConnector.InitiatePriceList(initiateUploadContext);
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
                                    priceListstatus = PriceListStatus.New;
                                    break;
                            }
                            manager.UpdateInitiatePriceList(pricelist.PriceListId, (int)pricelist.Status, (int)pricelist.Result);
                        }
                        _locked = false;
                        Thread.Sleep(1000);
                    }

                }
            }
            catch (Exception exc)
            {

                throw exc;
            }
        }

    }
}
