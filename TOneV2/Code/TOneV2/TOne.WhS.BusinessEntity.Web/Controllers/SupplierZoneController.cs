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
       [Route("GetSupplierZonesInfo")]
       public IEnumerable<SupplierZoneInfo> GetSupplierZonesInfo(int supplierId,string filter)
       {
           SupplierZoneManager manager = new SupplierZoneManager();
           return manager.GetSupplierZonesInfo(supplierId, filter);
       }
       [HttpPost]
       [Route("GetSupplierZonesInfoByIds")]
       public IEnumerable<SupplierZoneInfo> GetSupplierZonesInfoByIds(SupplierZoneInput input)
       {
           SupplierZoneManager manager = new SupplierZoneManager();
           return manager.GetSupplierZonesInfoByIds(input.SupplierZoneIds);
       }
    }
   public class SupplierZoneInput
   {
       public List<long> SupplierZoneIds { get; set; }
   }
}