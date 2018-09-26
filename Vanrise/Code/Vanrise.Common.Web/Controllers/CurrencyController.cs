using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Vanrise.Common.Web.Controllers
{
   [RoutePrefix(Constants.ROUTE_PREFIX + "Currency")]
    public class VRCommon_CurrencyController  : BaseAPIController
    {
       [HttpPost]
       [Route("GetFilteredCurrencies")]
       public object GetFilteredCurrencies(Vanrise.Entities.DataRetrievalInput<CurrencyQuery> input)
       {
           CurrencyManager manager = new CurrencyManager();
           return GetWebResponse(input, manager.GetFilteredCurrencies(input), "Currencies");
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
           return manager.GetCurrency(currencyId,true);
       }

       [HttpGet]
       [Route("GetSystemCurrency")]
       public Currency GetSystemCurrency()
       {
           CurrencyManager manager = new CurrencyManager();
           return manager.GetSystemCurrency();
       }

       [HttpGet]
       [Route("GetSystemCurrencyId")]
       public int GetSystemCurrencyId()
       {
           ConfigManager manager = new ConfigManager();
           return manager.GetSystemCurrencyId();
       }

       [HttpPost]
       [Route("AddCurrency")]
       public Vanrise.Entities.InsertOperationOutput<CurrencyDetail> AddCurrency(Currency currency)
       {
           CurrencyManager manager = new CurrencyManager();
           return manager.AddCurrency(currency);
       }
       [HttpPost]
       [Route("UpdateCurrency")]
       public Vanrise.Entities.UpdateOperationOutput<CurrencyDetail> UpdateCurrency(Currency currency)
       {
           CurrencyManager manager = new CurrencyManager();
           return manager.UpdateCurrency(currency);
       }
       
    }
}
