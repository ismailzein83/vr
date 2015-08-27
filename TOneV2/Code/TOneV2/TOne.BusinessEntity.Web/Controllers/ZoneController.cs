using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class ZoneController : ApiController
    {
        [HttpGet]
        public List<ZoneInfo> GetOwnZones(string nameFilter)
        {
            ZoneManager manager = new ZoneManager();
            return manager.GetOwnZones(nameFilter);
        }

        [HttpGet]
        public List<ZoneInfo> GetSupplierZones(string nameFilter, string supplierId)
        {
            ZoneManager manager = new ZoneManager();
            return manager.GetSupplierZones(supplierId, nameFilter);
        }
        [HttpGet]
        public List<ZoneInfo> GetCustomerZones(string nameFilter, string customerId)
        {
            ZoneManager manager = new ZoneManager();
            return manager.GetCustomerZones(customerId, nameFilter);
        }
        [HttpGet]
        public ZoneInfo GetZoneById(int zoneId)
        {
            ZoneManager manager = new ZoneManager();
            Zone z=manager.GetZone(zoneId);
            ZoneInfo zoneInfo= new ZoneInfo();
            zoneInfo.ZoneId=z.ZoneId;
            zoneInfo.Name=z.Name;
            return zoneInfo;
        }
    }
}