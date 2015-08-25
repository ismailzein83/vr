using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.Analytics.Business;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.Analytics.Web.Controllers
{
    public class SupplierTariffController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredSupplierTariffs(Vanrise.Entities.DataRetrievalInput<SupplierTariffQuery> input)
        {
            SupplierTariffManager manager = new SupplierTariffManager();
            return GetWebResponse(input, manager.GetFilteredSupplierTariffs(input));
        }
    }
}