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
    public class SaleZoneController : BaseAPIController
    {
        //[HttpGet]
        //public List<SaleZone> GetSaleZonesByPackage(int packageId,DateTime effectiveDate)
        //{
        //    SaleZoneManager manager = new SaleZoneManager();
        //    return manager.GetSaleZones(packageId, effectiveDate);
        //}

        [HttpGet]
        public List<SaleZoneInfo> GetSaleZonesInfo(int packageId, string filter)
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZonesInfo(packageId, filter);
        }

        [HttpPost]
        public List<SaleZoneInfo> GetSaleZonesInfoByIds(SaleZoneInput input)
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZonesInfoByIds(input.PackageId, input.SaleZoneIds);
        }
    }

    public class SaleZoneInput
    {
        public int PackageId { get; set; }

        public List<long> SaleZoneIds { get; set; }
    }
}