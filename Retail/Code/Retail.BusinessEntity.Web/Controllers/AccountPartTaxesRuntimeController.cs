using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountPartTaxesRuntime")]
    public class AccountPartTaxesRuntimeController : BaseAPIController
    {
        AccountPartTaxesRuntimeManager _manager = new AccountPartTaxesRuntimeManager();

        [HttpPost]
        [Route("GetInvoiceTypesTaxesRuntime")]
        public Dictionary<Guid, InvoiceTypesTaxesRuntime> GetInvoiceTypesTaxesRuntime(List<Guid> InvoiceTypesIds) {
            return _manager.GetInvoiceTypesTaxesRuntime(InvoiceTypesIds);
        }
    }
}