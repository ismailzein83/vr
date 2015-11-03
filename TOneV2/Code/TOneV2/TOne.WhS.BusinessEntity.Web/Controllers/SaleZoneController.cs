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
    public class SaleZoneController : BaseAPIController
    {
        public IEnumerable<SaleZoneInfo> GetSaleZonesInfo(string nameFilter, string serializedFilter)
        {
            SaleZoneInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<SaleZoneInfoFilter>(serializedFilter) : null;
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZonesInfo(nameFilter, filter);
        }

        [HttpPost]
        public IEnumerable<SaleZoneInfo> GetSaleZonesInfoByIds(SaleZoneInput input)
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZonesInfoByIds(input.SellingNumberPlanId, input.SaleZoneIds);
        }

        [HttpGet]
        public List<TemplateConfig> GetSaleZoneGroupTemplates()
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetSaleZoneGroupTemplates();
        }
    }

    public class SaleZoneInput
    {
        public int SellingNumberPlanId { get; set; }

        public List<long> SaleZoneIds { get; set; }
    }
}