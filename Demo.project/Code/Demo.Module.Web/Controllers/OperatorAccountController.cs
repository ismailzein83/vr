using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "OperatorAccount")]
    public class Demo_OperatorAccountController : BaseAPIController
    {

        [HttpPost]
        [Route("GetFilteredOperatorAccounts")]
        public object GetFilteredOperatorAccounts(Vanrise.Entities.DataRetrievalInput<OperatorAccountQuery> input)
        {
            OperatorAccountManager manager = new OperatorAccountManager();
            return GetWebResponse(input, manager.GetFilteredOperatorAccounts(input));
        }
        [HttpGet]
        [Route("GetOperatorAccountsInfo")]
        public IEnumerable<OperatorAccountInfo> GetOperatorAccountsInfo()
        {
            OperatorAccountManager manager = new OperatorAccountManager();
            return manager.GetOperatorAccountsInfo();
        }
        [HttpGet]
        [Route("GetOperatorAccount")]
        public OperatorAccount GetOperatorAccount(int operatorAccountId)
        {
            OperatorAccountManager manager = new OperatorAccountManager();
            return manager.GetOperatorAccount(operatorAccountId);
        }
        
        [HttpPost]
        [Route("AddOperatorAccount")]
        public Vanrise.Entities.InsertOperationOutput<OperatorAccountDetail> AddOperatorAccount(OperatorAccount operatorAccount)
        {
            OperatorAccountManager manager = new OperatorAccountManager();
            return manager.AddOperatorAccount(operatorAccount);
        }

        [HttpPost]
        [Route("UpdateOperatorAccount")]
        public Vanrise.Entities.UpdateOperationOutput<OperatorAccountDetail> UpdateOperatorAccount(OperatorAccount operatorAccount)
        {
            OperatorAccountManager manager = new OperatorAccountManager();
            return manager.UpdateOperatorAccount(operatorAccount);
        }
       
    }
}