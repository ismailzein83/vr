using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Invoice.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceTypeConfigs")]
    [JSONWithTypeAttribute]
    public class InvoiceTypeConfigsController : BaseAPIController
    {
        InvoiceTypeConfigsManager _manager = new InvoiceTypeConfigsManager();
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
        [HttpGet]
        [Route("GetItemsFilterConfigs")]
        public IEnumerable<ItemsFilterConfig> GetItemsFilterConfigs()
        {
            return _manager.GetItemsFilterConfigs();
        }
        [HttpGet]
        [Route("GetInvoiceExtendedSettingsConfigs")]
        public IEnumerable<InvoiceExtendedSettingsConfig> GetInvoiceExtendedSettingsConfigs()
        {
            return _manager.GetInvoiceExtendedSettingsConfigs();
        }

    }
}