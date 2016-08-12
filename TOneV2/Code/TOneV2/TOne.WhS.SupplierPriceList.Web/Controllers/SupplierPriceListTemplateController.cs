using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.SupplierPriceList.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SupplierPriceListTemplate")]
    [JSONWithTypeAttribute]
    public class SupplierPriceListTemplateController : BaseAPIController
    {
        private SupplierPriceListTemplateManager _manager;
        public SupplierPriceListTemplateController()
        {
            this._manager = new SupplierPriceListTemplateManager();
        }

        [HttpPost]
        [Route("UpdateSupplierPriceListTemplate")]
        public Vanrise.Entities.UpdateOperationOutput<SupplierPriceListTemplate> UpdateSupplierPriceListTemplate(SupplierPriceListTemplate supplierPriceListTemplate)
        {
            return _manager.UpdateInputPriceListTemplate(supplierPriceListTemplate);
        }

        [HttpPost]
        [Route("AddSupplierPriceListTemplate")]
        public Vanrise.Entities.InsertOperationOutput<SupplierPriceListTemplate> AddSupplierPriceListTemplate(SupplierPriceListTemplate supplierPriceListTemplate)
        {
            return _manager.AddSupplierPriceListTemplate(supplierPriceListTemplate);
        }
     
        [HttpGet]
        [Route("GetSupplierPriceListTemplate")]
        public SupplierPriceListTemplate GetSupplierPriceListTemplate(int supplierPriceListTemplateId)
        {
            return _manager.GetSupplierPriceListTemplate(supplierPriceListTemplateId);
        }
   
        [HttpGet]
        [Route("GetSupplierPriceListConfigurationTemplateConfigs")]
        public IEnumerable<SupplierPriceListInputConfig> GetSupplierPriceListConfigurationTemplateConfigs()
        {
            return _manager.GetSupplierPriceListConfigurationTemplateConfigs();
        }

        [HttpGet]
        [Route("GetSupplierPriceListTemplateBySupplierId")]
        public SupplierPriceListTemplate GetSupplierPriceListTemplateBySupplierId(int supplierId)
        {
            return _manager.GetSupplierPriceListTemplateBySupplierId(supplierId);
        }

        [HttpPost]
        [Route("TestConversionForSupplierPriceList")]
        public Object TestConversionForSupplierPriceList(SupplierPriceListTestConversionInput input)
        {
            return base.GetExcelResponse(_manager.TestConversionForSupplierPriceList(input.FileId, input.Settings));
        }
    }
    public class SupplierPriceListTestConversionInput
    {
        public long FileId { get; set; }
        public SupplierPriceListSettings Settings { get; set; }
    }
}