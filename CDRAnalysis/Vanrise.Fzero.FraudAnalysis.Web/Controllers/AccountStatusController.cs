using System;
using System.IO;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Fzero.FraudAnalysis.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountStatus")]
    public class AccountStatusController : BaseAPIController
    {

        [HttpPost]
        [Route("AddAccountStatus")]
        public Vanrise.Entities.InsertOperationOutput<AccountStatusDetail> AddAccountStatus(AccountStatus accountStatus)
        {
            AccountStatusManager manager = new AccountStatusManager();
            return manager.AddAccountStatus(accountStatus);
        }
        [HttpPost]
        [Route("UpdateAccountStatus")]
        public Vanrise.Entities.UpdateOperationOutput<AccountStatusDetail> UpdateAccountStatus(AccountStatus accountStatus)
        {
            AccountStatusManager manager = new AccountStatusManager();
            return manager.UpdateAccountStatus(accountStatus);
        }

        [HttpGet]
        [Route("DownloadAccountStatusesTemplate")]
        public object DownloadAccountStatusesTemplate()
        {
            var template = "~/Client/Modules/FraudAnalysis/Template/AccountStatusList.xls";
            string physicalPath = HttpContext.Current.Server.MapPath(template);
            byte[] bytes = File.ReadAllBytes(physicalPath);

            MemoryStream memStreamRate = new System.IO.MemoryStream();
            memStreamRate.Write(bytes, 0, bytes.Length);
            memStreamRate.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(memStreamRate, "AccountStatusList.xls");
        }

        [HttpGet]
        [Route("UploadAccountStatuses")]
        public string UploadAccountStatuses(int fileID, DateTime validTill, string reason)
        {
            AccountStatusManager manager = new AccountStatusManager();
            return manager.AddAccountStatuses(fileID, validTill, reason);
        }


        [HttpPost]
        [Route("GetAccountStatusesData")]
        public object GetAccountStatusesData(Vanrise.Entities.DataRetrievalInput<AccountStatusQuery> input)
        {
            AccountStatusManager manager = new AccountStatusManager();
            return GetWebResponse(input, manager.GetAccountStatusesData(input));

        }


        [HttpGet]
        [Route("GetAccountStatus")]
        public AccountStatus GetAccountStatus(string accountNumber)
        {
            AccountStatusManager manager = new AccountStatusManager();
            return manager.GetAccountStatus(accountNumber);
        }


        [HttpGet]
        [Route("DeleteAccountStatus")]
        public object DeleteAccountStatus(string accountNumber)
        {
            AccountStatusManager manager = new AccountStatusManager();
            return manager.DeleteAccountStatus(accountNumber);
        }


    }
}