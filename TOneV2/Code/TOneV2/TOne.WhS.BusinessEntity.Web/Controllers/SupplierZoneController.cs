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
   [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierZone")]
    public class SupplierZoneController : BaseAPIController
    {
       [HttpGet]
       [Route("GetSupplierZones")]
       public IEnumerable<SupplierZoneInfo> GetSupplierZones(int supplierId)
       {
           SupplierZoneManager manager = new SupplierZoneManager();
           return manager.GetSupplierZones(supplierId);
       }
    }
}