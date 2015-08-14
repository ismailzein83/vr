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
    public class CurrencyController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<Currency> GetCurrencies()
        {
            CurrencyManager manager = new CurrencyManager();
            return manager.GetCurrencies();
        }
    }
}