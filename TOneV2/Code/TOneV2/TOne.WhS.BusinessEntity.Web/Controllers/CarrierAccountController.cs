using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CarrierAccount")]
    public class WhSBE_CarrierAccountController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCarrierAccounts")]
        public List<CarrierAccountInfo> GetCarrierAccounts(bool getCustomers, bool getSuppliers)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            return manager.GetCarrierAccounts(getCustomers, getSuppliers);
        }
    }
}