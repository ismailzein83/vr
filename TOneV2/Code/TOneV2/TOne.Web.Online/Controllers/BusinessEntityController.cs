﻿using System;
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
        public List<TOne.Entities.CarrierInfo> GetCarriers(string carrierType = null)
        {
            BusinessEntityManager businessmanager = new BusinessEntityManager();

            return businessmanager.GetCarriers(carrierType);
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