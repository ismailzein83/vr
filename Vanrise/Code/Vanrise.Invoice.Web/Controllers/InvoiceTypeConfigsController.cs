﻿using System;
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
        [Route("GetInvoiceGridFilterConditionConfigs")]
        public IEnumerable<InvoiceGridFilterConditionConfig> GetInvoiceGridFilterConditionConfigs()
        {
            return _manager.GetInvoiceGridFilterConditionConfigs();
        }
        [HttpGet]
        [Route("GetInvoiceGeneratorFilterConditionConfigs")]
        public IEnumerable<InvoiceGeneratorFilterConditionConfig> GetInvoiceGeneratorFilterConditionConfigs()
        {
            return _manager.GetInvoiceGeneratorFilterConditionConfigs();
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
        [HttpGet]
        [Route("GetBillingPeriodTemplateConfigs")]
        public IEnumerable<BillingPeriodConfig> GetBillingPeriodTemplateConfigs()
        {
            return _manager.GetBillingPeriodTemplateConfigs();
        }

        [HttpGet]
        [Route("GetStartDateCalculationMethodConfigs")]
        public IEnumerable<StartDateCalculationMethodConfig> GetStartDateCalculationMethodConfigs()
        {
            return _manager.GetStartDateCalculationMethodConfigs();
        }
        [HttpGet]
        [Route("GetInvoiceSettingPartsConfigs")]
        public IEnumerable<InvoiceSettingPartConfig> GetInvoiceSettingPartsConfigs()
        {
            return _manager.GetInvoiceSettingPartsConfigs();
        }
        [HttpGet]
        [Route("GetInvoiceBulkActionSettingsConfigs")]
        public IEnumerable<InvoiceBulkActionSettingsConfig> GetInvoiceBulkActionSettingsConfigs()
        {
            return _manager.GetInvoiceBulkActionSettingsConfigs();
        }
        [HttpGet]
        [Route("GetItemSetNameStorageRuleConfigs")]
        public IEnumerable<ItemSetNameStorageRuleConfig> GetItemSetNameStorageRuleConfigs()
        {
            return _manager.GetItemSetNameStorageRuleConfigs();
        }
        [HttpGet]
        [Route("GetInvoiceUIGridColumnFilterConfigs")]
        public IEnumerable<InvoiceUIGridColumnFilterConfig> GetInvoiceUIGridColumnFilterConfigs()
        {
            return _manager.GetInvoiceUIGridColumnFilterConfigs();
        }

        [HttpGet]
        [Route("GetInvoiceSubSectionFilterConfigs")]
        public IEnumerable<InvoiceSubSectionFilterConfig> GetInvoiceSubSectionFilterConfigs()
        {
            return _manager.GetInvoiceSubSectionFilterConfigs();
        }

    }
}