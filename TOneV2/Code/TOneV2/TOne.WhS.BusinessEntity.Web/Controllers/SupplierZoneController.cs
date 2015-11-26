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
       [HttpPost]
       [Route("GetFilteredSupplierZones")]
       public object GetFilteredSupplierZones(Vanrise.Entities.DataRetrievalInput<SupplierZoneQuery> input)
       {
           SupplierZoneManager manager = new SupplierZoneManager();
           return GetWebResponse(input, manager.GetFilteredSupplierZones(input));
       }


       [HttpGet]
       [Route("GetSupplierZoneInfo")]
       public IEnumerable<SupplierZoneInfo> GetSupplierZoneInfo(string serializedFilter, string searchValue)
       {
           SupplierZoneManager manager = new SupplierZoneManager();
           SupplierZoneInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<SupplierZoneInfoFilter>(serializedFilter) : null;
           return manager.GetSupplierZoneInfo(filter, searchValue);
       }
       [HttpGet]
       [Route("GetSupplierZoneInfoByIds")]
       public IEnumerable<SupplierZoneInfo> GetSupplierZoneInfoByIds(string serializedObj)
       {
           List<long> selectedIds = serializedObj != null ? Vanrise.Common.Serializer.Deserialize<List<long>>(serializedObj) : null;
           SupplierZoneManager manager = new SupplierZoneManager();
           return manager.GetSupplierZoneInfoByIds(selectedIds);
       }
    }
   public class SupplierZoneInput
   {
       public List<long> SupplierZoneIds { get; set; }
   }
}