using PartnerPortal.CustomerAccess.Business;
using PartnerPortal.CustomerAccess.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace PartnerPortal.CustomerAccess.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "RetailAccountUser")]
    [JSONWithTypeAttribute]
    public class RetailAccountUserController : BaseAPIController
    {
        [HttpPost]
        [Route("AddRetailAccountUser")]
        public Vanrise.Entities.InsertOperationOutput<UserDetail> AddRetailAccountUser(RetailAccount retailAccount)
        {
            RetailAccountUserManager _manager = new RetailAccountUserManager();
            return _manager.AddRetailAccountUser(retailAccount);
        }
    }
}