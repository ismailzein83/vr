using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.NumberingPlan.Business;
using Vanrise.NumberingPlan.Entities;
using Vanrise.NumberingPlan.Web;
using Vanrise.Web.Base;

namespace Vanrise.NumberingPlan.Web.Controllers
{
    [RoutePrefix(Vanrise.NumberingPlan.Web.Constants.ROUTE_PREFIX + "SaleZone")]
    public class NP_SaleZoneController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSaleZones")]
        public object GetFilteredSaleZones(Vanrise.Entities.DataRetrievalInput<SaleZoneQuery> input)
        {
            SaleZoneManager manager = new SaleZoneManager();
            return GetWebResponse(input, manager.GetFilteredSaleZones(input), "Sale Zones");
        }


        [HttpGet]
        [Route("GetSaleZone")]
        public SaleZone GetSaleZone(int saleZoneId)
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZone(saleZoneId);
        }


        [HttpPost]
        [Route("GetSellingNumberPlanIdBySaleZoneId")]
        public IEnumerable<SaleZoneInfo> GetSellingNumberPlanIdBySaleZoneIds(List<long> saleZoneIds)
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSellingNumberPlanIdBySaleZoneIds(saleZoneIds);
        }


        [HttpGet]
        [Route("GetSaleZonesInfo")]
        public IEnumerable<SaleZoneInfo> GetSaleZonesInfo(string nameFilter, int sellingNumberPlanId, string serializedFilter)
        {
            SaleZoneInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<SaleZoneInfoFilter>(serializedFilter) : null;
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZonesInfo(nameFilter, sellingNumberPlanId, filter);
        }


        [HttpPost]
        [Route("GetSaleZonesInfoByIds")]
        public IEnumerable<SaleZoneInfo> GetSaleZonesInfoByIds(SaleZoneInput input)
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZonesInfoByIds(input.SaleZoneIds);
        }

        [HttpGet]
        [Route("GetSaleZoneGroupTemplates")]
        public IEnumerable<SaleZoneGroupConfig> GetSaleZoneGroupTemplates()
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZoneGroupTemplates();
        }
    }

    public class SaleZoneInput
    {
        public HashSet<long> SaleZoneIds { get; set; }
    }
}