using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class CarrierAccountConnectionController : ApiController
    {
        [HttpGet]
        public List<CarrierConnection> GetConnectionByCarrierType(CarrierType type)
        {
            //System.Threading.Thread.Sleep(2000);
            CarrierAccountConnectionManager manager = new CarrierAccountConnectionManager();
            return manager.GetConnectionByCarrierType(type);
        }
    }
}