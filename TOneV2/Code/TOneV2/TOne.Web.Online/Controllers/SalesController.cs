using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.Sales.Business;
using TOne.Sales.Entities;

namespace TOne.Web.Online.Controllers
{
    public class SalesController : ApiController
    {
        [HttpGet]
        public bool SavePriceList(string customerId, List<SaleLcrRate> saleLcrRates, string currencyId, bool sendEmail)
        {
            SaleLCRManager manager = new SaleLCRManager();
            return manager.SavePriceList(customerId, saleLcrRates, currencyId, sendEmail);
        }
    }
}