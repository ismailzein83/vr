using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Entities;
using TOne.Data;
using TOne.Business;

namespace TOne.Web.Online.Controllers
{
    public class BusinessEntityController : ApiController
    {
        // GET api/<controller>
        [HttpGet]
        public List<TOne.Entities.CarrierInfo> GetCarriers(int carrierType = 0)
        {
            BusinessEntityManager businessmanager = new BusinessEntityManager();
            TOne.Entities.CarrierType type = (CarrierType)carrierType;
            return businessmanager.GetCarriers(type.ToString());
        }

        [HttpGet]
        public List<TOne.Entities.CodeGroupInfo> GetCodeGroups()
        {
            BusinessEntityManager businessmanager = new BusinessEntityManager();
            return businessmanager.GetCodeGroups();
        }

        [HttpGet]
        public List<TOne.Entities.SwitchInfo> GetSwitches()
        {
            BusinessEntityManager businessmanager = new BusinessEntityManager();
            return businessmanager.GetSwitches();
        }
    }
}