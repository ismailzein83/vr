using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountManagerAssignment")]
    public class AccountManagerAssignmentController : BaseAPIController
    {
        //[HttpPost]
        //[Route("GetAccountManagerAssignmentByCarrierAccountId")]
        //public AccountManagerAssignment GetAccountManagerAssignmentByCarrierAccountId(int carrierAccountId)
        //{
        //    return new AccountManagerAssignmentManager().GetAccountManagerAssignmentByCarrierAccountId(carrierAccountId);
        //}
    }
}