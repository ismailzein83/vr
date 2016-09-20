﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SaleZone")]
    public class WhSBE_SaleZoneController : BaseAPIController
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

        //[HttpGet]
        //[Route("GetSaleZonesInfoByCountryId")]
        //public IEnumerable<SaleZoneInfo> GetSaleZonesInfoByCountryId(int sellingNumberPlanId, int countryId)
        //{
        //    SaleZoneManager manager = new SaleZoneManager();
        //    return manager.GetSaleZonesByCountryId(sellingNumberPlanId, countryId);
        //}

        [HttpPost]
        [Route("GetSaleZonesInfoByIds")]
        public IEnumerable<SaleZoneInfo> GetSaleZonesInfoByIds(SaleZoneInput input)
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZonesInfoByIds(input.SaleZoneIds, input.SaleZoneFilterSettings);
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
       // public int SellingNumberPlanId { get; set; }

        public HashSet<long> SaleZoneIds { get; set; }

        public SaleZoneFilterSettings SaleZoneFilterSettings { get; set; }
    }
}