using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.SupplierPriceList.BP.Arguments;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.SupplierPriceList.Web.Controllers
{
    public class SupplierPriceListController:BaseAPIController
    {
        [HttpGet]
        public CreateProcessOutput UploadSupplierPriceList(int supplierAccountId, int fileId, DateTime? effectiveDate)
        {
            BPClient bpClient = new BPClient();
            return bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new SupplierPriceListProcessInput
                {
                    EffectiveDate = effectiveDate,
                    FileId = fileId,
                    SupplierAccountId = supplierAccountId
                }

            });
        }

    }
}