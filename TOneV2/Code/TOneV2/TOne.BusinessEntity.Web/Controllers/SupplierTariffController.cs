using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
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