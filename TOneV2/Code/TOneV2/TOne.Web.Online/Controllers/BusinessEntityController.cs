using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Business;
using TOne.BusinessEntity.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.Web.Online.Controllers
{
    public class BusinessEntityController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        public List<CarrierInfo> GetCarriers(CarrierType carrierType = CarrierType.Exchange)
        {
            CarrierManager manager = new CarrierManager();
            return manager.GetCarriers(carrierType);
        }

        [HttpGet]
        public List<CodeGroupInfo> GetCodeGroups()
        {
            CodeManager manager = new CodeManager();
            return manager.GetCodeGroups();
        }

        [HttpGet]
        public List<TOne.Entities.SwitchInfo> GetSwitches()
        {
            BusinessEntityManager businessmanager = new BusinessEntityManager();
            return businessmanager.GetSwitches();
        }
    }
}