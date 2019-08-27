using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.SMSBusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Security.Business;
using Vanrise.Web.Base;

namespace TOne.WhS.SMSBusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierSMSRateChanges")]

    public class SupplierSMSRateChangesController : BaseAPIController
    {
        SupplierSMSRateDraftManager _supplierSMSRateChangesManager = new SupplierSMSRateDraftManager();

        [HttpPost]
        [Route("GetFilteredChanges")]
        public List<SupplierSMSRateChangesDetail> GetFilteredChanges(SupplierSMSRateChangesQuery input)
        {
            return _supplierSMSRateChangesManager.GetFilteredChanges(input);
        }

        [HttpPost]
        [Route("InsertOrUpdateChanges")]
        public DraftStateResult InsertOrUpdateChanges(SupplierSMSRateDraftToUpdate supplierDraftToUpdate)
        {
            return _supplierSMSRateChangesManager.InsertOrUpdateChanges(supplierDraftToUpdate);
        }

        [HttpPost]
        [Route("UpdateSMSRateChangesStatus")]
        public bool UpdateSMSRateChangesStatus(UpdateSupplierSMSDraftStatusInput input)
        {
            return _supplierSMSRateChangesManager.UpdateSMSRateChangesStatus(input, SecurityContext.Current.GetLoggedInUserId());
        }

        [HttpPost]
        [Route("GetDraftData")]
        public DraftData GetDraftData(SupplierDraftDataInput input)
        {
            return _supplierSMSRateChangesManager.GetDraftData(input);
        }

        [HttpGet]
        [Route("DownloadImportSupplierSMSRateTemplate")]
        public object DownloadImportSupplierSMSRateTemplate()
        {
            string templateRelativePath = "~/Client/Modules/WhS_SMSBusinessEntity/Templates/Import Supplier SMS Rates Template.xlsx";
            string templateAbsolutePath = HttpContext.Current.Server.MapPath(templateRelativePath);
            byte[] templateBytes = File.ReadAllBytes(templateAbsolutePath);
            MemoryStream memoryStream = new System.IO.MemoryStream();
            memoryStream.Write(templateBytes, 0, templateBytes.Length);
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(memoryStream, "Import Supplier SMS Rates Template.xlsx");
        }

        [HttpPost]
        [Route("UploadSMSRateChanges")]
        public UploadSupplierSMSRateChangesLog UploadSMSRateChanges(UploadSupplierSMSRateChangesInput input)
        {
            return _supplierSMSRateChangesManager.UploadSMSRateChanges(input);
        }

        [HttpGet]
        [Route("DownloadImportedSupplierSMSRateLog")]
        public object DownloadImportedSupplierSMSRateLog(long fileId)
        {
            byte[] bytes = _supplierSMSRateChangesManager.DownloadImportedSupplierSMSRateLog(fileId);
            return GetExcelResponse(bytes, "ImportedSMSRatesResults.xls");
        }
    }
}