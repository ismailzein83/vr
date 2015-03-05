using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.Web.Controllers
{
    public class BusinessEntityController : ApiController
    {
        [HttpGet]
        public List<CarrierInfo> GetCarriers(CarrierType carrierType)
        {
            CarrierManager manager = new CarrierManager();
            return manager.GetCarriers(carrierType);
        }
    }
}
