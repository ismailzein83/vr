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
   [RoutePrefix(Constants.ROUTE_PREFIX + "CurrencyExchangeRate")]
    public class VRCommon_CurrencyExchangeRateController : BaseAPIController 
    {
       [HttpPost]
       [Route("GetFilteredExchangeRateCurrencies")]
       public object GetFilteredExchangeRateCurrencies(Vanrise.Entities.DataRetrievalInput<CurrencyExchangeRateQuery> input)
       {
           CurrencyExchangeRateManager manager = new CurrencyExchangeRateManager();
           return GetWebResponse(input, manager.GetFilteredCurrenciesExchangeRates(input), "Exchange Rate Currencies");
       }

       [HttpGet]
       [Route("GetAllCurrenciesExchangeRate")]
       public IEnumerable<CurrencyExchangeRate> GetAllCurrenciesExchangeRate()
       {
           CurrencyExchangeRateManager manager = new CurrencyExchangeRateManager();
           return manager.GetAllCurrenciesExchangeRate();
       }
       [HttpGet]
       [Route("GetCurrencyExchangeRate")]
       public CurrencyExchangeRate GetCurrencyExchangeRate(int currencyExchangeRateId)
       {
           CurrencyExchangeRateManager manager = new CurrencyExchangeRateManager();
           return manager.GetCurrencyExchangeRate(currencyExchangeRateId);
       }

       [HttpPost]
       [Route("AddCurrencyExchangeRate")]
       public Vanrise.Entities.InsertOperationOutput<CurrencyExchangeRateDetail> AddCurrencyExchangeRate(CurrencyExchangeRate currencyExchangeRate)
       {
           CurrencyExchangeRateManager manager = new CurrencyExchangeRateManager();
           return manager.AddCurrencyExchangeRate(currencyExchangeRate);
       }
       [HttpPost]
       [Route("UpdateCurrencyExchangeRate")]
       public Vanrise.Entities.UpdateOperationOutput<CurrencyExchangeRateDetail> UpdateCurrencyExchangeRate(CurrencyExchangeRate UpdateCurrencyExchangeRate)
       {
           CurrencyExchangeRateManager manager = new CurrencyExchangeRateManager();
           return manager.UpdateCurrencyExchangeRate(UpdateCurrencyExchangeRate);
       }
       
    }
}
