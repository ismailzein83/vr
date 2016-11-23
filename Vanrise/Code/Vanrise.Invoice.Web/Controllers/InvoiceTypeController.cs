using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Invoice.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceType")]
    [JSONWithTypeAttribute]

    public class InvoiceTypeController:BaseAPIController
    {
        InvoiceTypeManager _manager = new InvoiceTypeManager();

        [HttpGet]
        [Route("GetInvoiceType")]
        public InvoiceType GetInvoiceType(Guid invoiceTypeId)
        {
            return _manager.GetInvoiceType(invoiceTypeId);
        }      
        [HttpGet]
        [Route("GetInvoiceTypeRuntime")]
        public InvoiceTypeRuntime GetInvoiceTypeRuntime(Guid invoiceTypeId)
        {
            return _manager.GetInvoiceTypeRuntime(invoiceTypeId);
        }
        [HttpGet]
        [Route("GetGeneratorInvoiceTypeRuntime")]
        public GeneratorInvoiceTypeRuntime GetGeneratorInvoiceTypeRuntime(Guid invoiceTypeId)
        {
            return _manager.GetGeneratorInvoiceTypeRuntime(invoiceTypeId);
        }
        [HttpPost]
        [Route("GetFilteredInvoiceTypes")]
        public object GetFilteredInvoiceTypes(Vanrise.Entities.DataRetrievalInput<InvoiceTypeQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredInvoiceTypes(input));
        }
        
        [HttpPost]
        [Route("AddInvoiceType")]
        public Vanrise.Entities.InsertOperationOutput<InvoiceTypeDetail> AddInvoiceType(InvoiceType invoiceType)
        {
            return _manager.AddInvoiceType(invoiceType);
        }

        [HttpPost]
        [Route("UpdateInvoiceType")]
        public Vanrise.Entities.UpdateOperationOutput<InvoiceTypeDetail> UpdateInvoiceType(InvoiceType invoiceType)
        {
            return _manager.UpdateInvoiceType(invoiceType);
        }
        [HttpPost]
        [Route("CovertToGridColumnAttribute")]
        public IEnumerable<GridColumnAttribute> CovertToGridColumnAttribute(ConvertToGridColumnAttributeInput input)
        {
            return _manager.CovertToGridColumnAttribute(input);
        }
        [HttpGet]
        [Route("GetInvoiceTypesInfo")]
        public IEnumerable<InvoiceTypeInfo> GetInvoiceTypesInfo(string filter = null)
        {
            InvoiceTypeFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<InvoiceTypeFilter>(filter) : null;
            return _manager.GetInvoiceTypesInfo(serializedFilter);
        }
        
    }
}