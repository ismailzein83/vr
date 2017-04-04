using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Invoice.Business;
using TOne.WhS.Invoice.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Invoice.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceAccount")]
    public class InvoiceAccountController : BaseAPIController
    {
        InvoiceAccountManager _manager = new InvoiceAccountManager();

        [HttpPost]
        [Route("GetFilteredInvoiceAccounts")]
        public object GetFilteredInvoiceAccounts(Vanrise.Entities.DataRetrievalInput<InvoiceAccountQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredInvoiceAccounts(input));
        }

        [HttpPost]
        [Route("AddInvoiceAccount")]
        public Vanrise.Entities.InsertOperationOutput<InvoiceAccountDetail> AddInvoiceAccount(InvoiceAccount invoiceAccount)
        {
            return _manager.AddInvoiceAccount(invoiceAccount);
        }

        [HttpPost]
        [Route("UpdateInvoiceAccount")]
        public Vanrise.Entities.UpdateOperationOutput<InvoiceAccountDetail> UpdateInvoiceAccount(InvoiceAccount invoiceAccount)
        {
            return _manager.UpdateInvoiceAccount(invoiceAccount);
        }

        [HttpGet]
        [Route("GetInvoiceAccount")]
        public InvoiceAccount GetInvoiceAccount(int invoiceAccountId)
        {
            return _manager.GetInvoiceAccount(invoiceAccountId);
        }
        [HttpGet]
        [Route("GetInvoiceAccountEditorRuntime")]
        public InvoiceAccountEditorRuntime GetInvoiceAccountEditorRuntime(int invoiceAccountId)
        {
            return _manager.GetInvoiceAccountEditorRuntime(invoiceAccountId);
        }

        [HttpGet]
        [Route("CheckCarrierAllowAddInvoiceAccounts")]
        public bool CheckCarrierAllowAddInvoiceAccounts(int? carrierProfileId = null, int? carrierAccountId = null)
        {
            return _manager.CheckCarrierAllowAddInvoiceAccounts(carrierProfileId, carrierAccountId);
        }

        [HttpGet]
        [Route("GetInvoiceAccountsInfo")]
        public IEnumerable<InvoiceAccountInfo> GetInvoiceAccountsInfo(string filter = null)
        {
            InvoiceAccountInfoFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<InvoiceAccountInfoFilter>(filter) : null;
            return _manager.GetInvoiceAccountsInfo(deserializedFilter);
        }
    }
}