using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierZoneDetails")]
    public class SupplierZoneDetailsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetSupplierZoneDetailsByCode")]
        public IEnumerable<SupplierZoneDetail> GetSupplierZoneDetailsByCode(string code)
        {
            SupplierZoneDetailsManager manager = new SupplierZoneDetailsManager();
            return manager.GetSupplierZoneDetailsByCode(code);
        }
    }
}