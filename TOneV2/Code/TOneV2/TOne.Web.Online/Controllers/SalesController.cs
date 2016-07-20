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
        public bool SavePriceList(string customerId, string saleLcrRatesString, string currencyId, bool sendEmail)
        {
            SaleLCRManager manager = new SaleLCRManager();
            Vanrise.Common.LoggerFactory.GetLogger().WriteInformation("{0}-CustomerId={1}-saleLCRRatesString={2}-currencyId={3}-sendEmail={4}.", "SavePriceList", customerId, saleLcrRatesString, customerId, sendEmail);
            List<SaleLcrRate> saleLcrRates = Vanrise.Common.Serializer.Deserialize<List<SaleLcrRate>>(saleLcrRatesString);
            return manager.SavePriceList(customerId, saleLcrRates, currencyId, sendEmail);
        }
    }
}