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
    public class PriceListController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<PriceList> GetPriceList()
        {
            PriceListManager manager = new PriceListManager();
            return manager.GetPriceList();
        }

    }
}
