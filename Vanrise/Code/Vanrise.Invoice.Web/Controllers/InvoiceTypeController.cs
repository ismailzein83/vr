using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common;
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
        [Route("GetInvoiceTypeExtendedSettings")]
        public InvoiceTypeExtendedSettings GetInvoiceTypeExtendedSettings(Guid invoiceTypeId)
        {
            return _manager.GetInvoiceTypeExtendedSettings(invoiceTypeId);
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
        [Route("ConvertToGridColumnAttribute")]
        public IEnumerable<GridColumnAttribute> ConvertToGridColumnAttribute(ConvertToGridColumnAttributeInput input)
        {
            return _manager.ConvertToGridColumnAttribute(input);
        }
        [HttpGet]
        [Route("GetInvoiceTypesInfo")]
        public IEnumerable<InvoiceTypeInfo> GetInvoiceTypesInfo(string filter = null)
        {
            InvoiceTypeInfoFilter serializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<InvoiceTypeInfoFilter>(filter) : null;
            return _manager.GetInvoiceTypesInfo(serializedFilter);
        }
        [HttpPost]
        [Route("GetInvoiceGeneratorActions")]
        public List<InvoiceGeneratorAction> GetInvoiceGeneratorActions(GenerateInvoiceInput generateInvoiceInput)
        {
            return _manager.GetInvoiceGeneratorActions(generateInvoiceInput);
        }
        [HttpGet]
        [Route("GetInvoicePartnerSelector")]
        public string GetInvoicePartnerSelector(Guid invoiceTypeId)
        {
            return _manager.GetInvoicePartnerSelector(invoiceTypeId);
        }
        [HttpGet]
        [Route("GetInvoiceAction")]
        public InvoiceAction GetInvoiceAction(Guid invoiceTypeId, Guid invoiceActionId)
        {
            return _manager.GetInvoiceAction(invoiceTypeId, invoiceActionId);
        }
        [HttpGet]
        [Route("GetRemoteInvoiceFieldsInfo")]
        public IEnumerable<InvoiceFieldInfo> GetRemoteInvoiceFieldsInfo(string filter = null)
        {
            return _manager.GetRemoteInvoiceFieldsInfo();
        }
        [HttpGet]
        [Route("GetRemoteInvoiceTypeCustomFieldsInfo")]
        public IEnumerable<string> GetRemoteInvoiceTypeCustomFieldsInfo(Guid invoiceTypeId)
        {
            return _manager.GetRemoteInvoiceTypeCustomFieldsInfo(invoiceTypeId);
        }
        [HttpGet]
        [Route("GetInvoiceTypeGridColumns")]
        public List<InvoiceUIGridColumnRunTime> GetInvoiceTypeGridColumns(Guid invoiceTypeId)
        {
            return _manager.GetInvoiceTypeGridColumns(invoiceTypeId);
        }

        [HttpGet]
        [Route("GetRemoteInvoiceTypeAttachmentsInfo")]
        public IEnumerable<InvoiceAttachmentInfo> GetRemoteInvoiceTypeAttachmentsInfo(Guid invoiceTypeId)
        {
            return _manager.GetRemoteInvoiceTypeAttachmentsInfo(invoiceTypeId);
        }

        [HttpGet]
        [Route("GetMenualInvoiceBulkActionsDefinitions")]
        public List<InvoiceBulkActionDefinitionEntity> GetMenualInvoiceBulkActionsDefinitions(Guid invoiceTypeId)
        {
            return _manager.GetMenualInvoiceBulkActionsDefinitions(invoiceTypeId);
        }

        [HttpGet]
        [Route("GetPartnerName")]
        public string GetPartnerName(Guid invoiceTypeId, string partnerId)
        {
            return _manager.GetPartnerName(invoiceTypeId, partnerId);
        }
        [HttpGet]
        [Route("GetPartnerInvoiceSettingFilterFQTN")]
        public string GetPartnerInvoiceSettingFilterFQTN(Guid invoiceTypeId)
        {
            return _manager.GetPartnerInvoiceSettingFilterFQTN(invoiceTypeId);
        }
        [HttpGet]
        [Route("GetInvoiceSettingPartsInfo")]
        public IEnumerable<InvoiceSettingPartDefinitionInfo> GetInvoiceSettingPartsInfo(string serializedFilter = null)
        {
            InvoiceSettingPartsInfoFilter deserializedFilter = serializedFilter != null? Serializer.Deserialize<InvoiceSettingPartsInfoFilter>(serializedFilter):null;
            return _manager.GetInvoiceSettingPartsInfo(deserializedFilter);
        }
    }
}