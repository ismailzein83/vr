using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.CodePreparation.BP.Arguments;
using TOne.WhS.CodePreparation.Business;
using Vanrise.BusinessProcess.Client;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.CodePreparation.Web.Controllers
{
    public class CodePreparationController:BaseAPIController
    {
        [HttpGet]
        public CreateProcessOutput UploadSaleZonesList(int saleZonePackageId, int fileId, DateTime effectiveDate)
      {
            CodePreparationManager manager = new CodePreparationManager();
            BPClient bpClient = new BPClient();
         return bpClient.CreateNewProcess(new CreateProcessInput
            {
                InputArguments = new CodePreparationProcessInput
                {
                    EffectiveDate = effectiveDate,
                    FileId = fileId,
                    SaleZonePackageId = saleZonePackageId
                }

            });
         //   return manager.UploadSaleZonesList(saleZonePackageId, fileId, effectiveDate);
      }
    }
}