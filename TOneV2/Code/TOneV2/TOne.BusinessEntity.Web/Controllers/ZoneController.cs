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
        public List<ZoneInfo> GetZones(string nameFilter, string supplierId)
        {
            ZoneManager manager = new ZoneManager();
            return manager.GetZones(supplierId, nameFilter);
        }
    }
}