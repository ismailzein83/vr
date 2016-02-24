using InterConnect.BusinessEntity.Business;
using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace InterConnect.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "OperatorAccount")]
    public class OperatorAccountController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredOperatorAccounts")]
        public object GetFilteredOperatorAccounts(Vanrise.Entities.DataRetrievalInput<OperatorAccountQuery> input)
        {
            OperatorAccountManager manager = new OperatorAccountManager();
            return GetWebResponse(input, manager.GetFilteredOperatorAccounts(input));
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
        public InsertOperationOutput<OperatorAccountDetail> AddOperatorAccount(OperatorAccount operatorAccount)
        {
            OperatorAccountManager manager = new OperatorAccountManager();
            return manager.AddOperatorAccount(operatorAccount);
        }

        [HttpPost]
        [Route("UpdateOperatorAccount")]
        public UpdateOperationOutput<OperatorAccountDetail> UpdateOperatorAccount(OperatorAccount operatorAccount)
        {
            OperatorAccountManager manager = new OperatorAccountManager();
            return manager.UpdateOperatorAccount(operatorAccount);
        }
    }
}