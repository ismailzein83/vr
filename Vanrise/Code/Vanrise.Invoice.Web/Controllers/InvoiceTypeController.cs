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
        [HttpPost]
        [Route("GetFilteredInvoiceTypes")]
        public object GetFilteredInvoiceTypes(Vanrise.Entities.DataRetrievalInput<InvoiceTypeQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredInvoiceTypes(input));
        }

        [HttpGet]
        [Route("GetInvoiceGeneratorConfigs")]
        public IEnumerable<InvoiceGeneratorConfig> GetInvoiceGeneratorConfigs()
        {
            return _manager.GetInvoiceGeneratorConfigs();
        }
        [HttpGet]
        [Route("GetInvoiceGridActionSettingsConfigs")]
        public IEnumerable<InvoiceGridActionSettingsConfig> GetInvoiceGridActionSettingsConfigs()
        {
            return _manager.GetInvoiceGridActionSettingsConfigs();
        }
        [HttpGet]
        [Route("GetRDLCDataSourceSettingsConfigs")]
        public IEnumerable<RDLCDataSourceSettingsConfig> GetRDLCDataSourceSettingsConfigs()
        {
            return _manager.GetRDLCDataSourceSettingsConfigs();
        }
        [HttpGet]
        [Route("GetRDLCParameterSettingsConfigs")]
        public IEnumerable<RDLCParameterSettingsConfig> GetRDLCParameterSettingsConfigs()
        {
            return _manager.GetRDLCParameterSettingsConfigs();
        }
        [HttpGet]
        [Route("GetInvoiceUISubSectionSettingsConfigs")]
        public IEnumerable<InvoiceUISubSectionSettingsConfig> GetInvoiceUISubSectionSettingsConfigs()
        {
            return _manager.GetInvoiceUISubSectionSettingsConfigs();
        }
        [HttpGet]
        [Route("GetInvoiceFilterConditionConfigs")]
        public IEnumerable<InvoiceFilterConditionConfig> GetInvoiceFilterConditionConfigs()
        {
            return _manager.GetInvoiceFilterConditionConfigs();
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
        [HttpGet]
        [Route("GetInvoicePartnerSettingsConfigs")]
        public IEnumerable<InvoicePartnerSettingsConfig> GetInvoicePartnerSettingsConfigs()
        {
            return _manager.GetInvoicePartnerSettingsConfigs();
        }
        [HttpGet]
        [Route("GetItemsFilterConfigs")]
        public IEnumerable<ItemsFilterConfig> GetItemsFilterConfigs()
        {
            return _manager.GetItemsFilterConfigs();
        }
        
    }
}