using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
namespace Vanrise.Web.Controllers
{
   [RoutePrefix(Constants.ROUTE_PREFIX + "Currency")]
    public class VRCommon_CurrencyController  : BaseAPIController
    {
       [HttpPost]
       [Route("GetFilteredCurrencies")]
       public object GetFilteredCurrencies(Vanrise.Entities.DataRetrievalInput<CurrencyQuery> input)
       {
           CurrencyManager manager = new CurrencyManager();
           return GetWebResponse(input, manager.GetFilteredCurrencies(input));
       }

       [HttpGet]
       [Route("GetAllCurrencies")]
       public IEnumerable<Currency> GetAllCurrencies()
       {
           CurrencyManager manager = new CurrencyManager();
           return manager.GetAllCurrencies();
       }
       [HttpGet]
       [Route("GetCurrency")]
       public Currency GetCurrency(int currencyId)
       {
           CurrencyManager manager = new CurrencyManager();
           return manager.GetCurrency(currencyId);
       }

       [HttpPost]
       [Route("AddCurrency")]
       public Vanrise.Entities.InsertOperationOutput<Currency> AddCurrency(Currency currency)
       {
           CurrencyManager manager = new CurrencyManager();
           return manager.AddCurrency(currency);
       }
       [HttpPost]
       [Route("UpdateCurrency")]
       public Vanrise.Entities.UpdateOperationOutput<Currency> UpdateCurrency(Currency currency)
       {
           CurrencyManager manager = new CurrencyManager();
           return manager.UpdateCurrency(currency);
       }
       
    }
}
