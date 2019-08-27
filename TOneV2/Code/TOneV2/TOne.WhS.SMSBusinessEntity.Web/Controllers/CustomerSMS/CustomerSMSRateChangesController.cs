using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using TOne.WhS.SMSBusinessEntity.Business;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Security.Business;
using Vanrise.Web.Base;

namespace TOne.WhS.SMSBusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerSMSRateChanges")]

    public class CustomerSMSRateChangesController : BaseAPIController
    {
        CustomerSMSRateDraftManager _customerSMSRateChangesManager = new CustomerSMSRateDraftManager();

        [HttpPost]
        [Route("GetFilteredChanges")]
        public List<CustomerSMSRateChangesDetail> GetFilteredChanges(CustomerSMSRateChangesQuery input)
        {
            return _customerSMSRateChangesManager.GetFilteredChanges(input);
        }

        [HttpPost]
        [Route("InsertOrUpdateChanges")]
        public DraftStateResult InsertOrUpdateChanges(CustomerSMSRateDraftToUpdate customerDraftToUpdate)
        {
            return _customerSMSRateChangesManager.InsertOrUpdateChanges(customerDraftToUpdate);
        }

        [HttpPost]
        [Route("UpdateSMSRateChangesStatus")]
        public bool UpdateSMSRateChangesStatus(UpdateCustomerSMSDraftStatusInput input)
        {
            return _customerSMSRateChangesManager.UpdateSMSRateChangesStatus(input, SecurityContext.Current.GetLoggedInUserId());
        }

        [HttpPost]
        [Route("GetDraftData")]
        public DraftData GetDraftData(CustomerDraftDataInput input)
        {
            return _customerSMSRateChangesManager.GetDraftData(input);
        }

        [HttpGet]
        [Route("DownloadImportCustomerSMSRateTemplate")]
        public object DownloadImportCustomerSMSRateTemplate()
        {
            string templateRelativePath = "~/Client/Modules/WhS_SMSBusinessEntity/Templates/Import Customer SMS Rates Template.xlsx";
            string templateAbsolutePath = HttpContext.Current.Server.MapPath(templateRelativePath);
            byte[] templateBytes = File.ReadAllBytes(templateAbsolutePath);
            MemoryStream memoryStream = new System.IO.MemoryStream();
            memoryStream.Write(templateBytes, 0, templateBytes.Length);
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(memoryStream, "Import Customer SMS Rates Template.xlsx");
        }

        [HttpPost]
        [Route("UploadSMSRateChanges")]
        public UploadCustomerSMSRateChangesLog UploadSMSRateChanges(UploadCustomerSMSRateChangesInput input)
        {
            return _customerSMSRateChangesManager.UploadSMSRateChanges(input);
        }

        [HttpGet]
        [Route("DownloadImportedCustomerSMSRateLog")]
        public object DownloadImportedCustomerSMSRateLog(long fileId)
        {
            byte[] bytes = _customerSMSRateChangesManager.DownloadImportedCustomerSMSRateLog(fileId);
            return GetExcelResponse(bytes, "ImportedSMSRatesResults.xls");
        }
    }
}