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
    [RoutePrefix(Constants.ROUTE_PREFIX + "SaleZone")]
    public class Demo_SaleZoneController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSaleZones")]
        public object GetFilteredSaleZones(Vanrise.Entities.DataRetrievalInput<SaleZoneQuery> input)
        {
            SaleZoneManager manager = new SaleZoneManager();
            return GetWebResponse(input, manager.GetFilteredSaleZones(input));
        }


        [HttpGet]
        [Route("GetSaleZone")]
        public SaleZone GetSaleZone(int saleZoneId)
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZone(saleZoneId);
        }


        [HttpGet]
        [Route("GetSaleZonesInfo")]
        public IEnumerable<SaleZoneInfo> GetSaleZonesInfo()
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZonesInfo();
        }
      
    }
   
}