using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "SalePriceListTemplate")]
    public class SalePriceListTemplateController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSalePriceListTemplates")]
        public object GetFilteredSalePriceListTemplates(Vanrise.Entities.DataRetrievalInput<SalePriceListTemplateQuery> input)
        {
            var manager = new SalePriceListTemplateManager();
            return GetWebResponse(input, manager.GetFilteredSalePriceListTemplates(input), "Sale PriceList Templates");
        }

        [HttpGet]
        [Route("GetSalePriceListTemplate")]
        public SalePriceListTemplate GetSalePriceListTemplate(int salePriceListTemplateId)
        {
            var manager = new SalePriceListTemplateManager();
            return manager.GetSalePriceListTemplate(salePriceListTemplateId,true);
        }

        [HttpGet]
        [Route("GetSalePriceListTemplatesInfo")]
        public IEnumerable<SalePriceListTemplateInfo> GetSalePriceListTemplatesInfo()
        {
            var manager = new SalePriceListTemplateManager();
            return manager.GetSalePriceListTemplatesInfo();
        }

        [HttpGet]
        [Route("GetSalePriceListTemplateSettingsExtensionConfigs")]
        public IEnumerable<SalePricelistTemplateSettingsExtensionConfig> GetSalePriceListTemplateSettingsExtensionConfigs()
        {
            var manager = new SalePriceListTemplateManager();
            return manager.GetSalePricelistTemplateSettingsExtensionConfigs();
        }

        [HttpGet]
        [Route("GetMappedCellsExtensionConfigs")]
        public IEnumerable<SalePricelistTemplateSettingsMappedCellExtensionConfig> GetMappedCellsExtensionConfigs()
        {
            var manager = new SalePriceListTemplateManager();
            return manager.GetMappedCellsExtensionConfigs();
        }

        [HttpGet]
        [Route("GetMappedTablesExtensionConfigs")]
        public IEnumerable<SalePriceListTemplateSettingsMappedTableExtensionConfig> GetMappedTablesExtensionConfigs()
        {
            var manager = new SalePriceListTemplateManager();
            return manager.GetMappedTablesExtensionConfigs();
        }

        [HttpGet]
        [Route("GetBasicSettingsMappedValueExtensionConfigs")]
        public IEnumerable<BasicMappedValueExtensionConfig> GetBasicSettingsMappedValueExtensionConfigs(string priceListType)
        {
            var manager = new SalePriceListTemplateManager();
            return manager.GetBasicSettingsMappedValueExtensionConfigs(priceListType);
        }

        [HttpPost]
        [Route("AddSalePriceListTemplate")]
        public Vanrise.Entities.InsertOperationOutput<SalePriceListTemplateDetail> AddSalePriceListTemplate(SalePriceListTemplate salePriceListTemplate)
        {
            var manager = new SalePriceListTemplateManager();
            return manager.AddSalePriceListTemplate(salePriceListTemplate);
        }

        [HttpPost]
        [Route("UpdateSalePriceListTemplate")]
        public Vanrise.Entities.UpdateOperationOutput<SalePriceListTemplateDetail> UpdateSalePriceListTemplate(SalePriceListTemplate salePriceListTemplate)
        {
            var manager = new SalePriceListTemplateManager();
            return manager.UpdateSalePriceListTemplate(salePriceListTemplate);
        }
             [HttpGet]
        [Route("GetSalePriceListTemplateHistoryDetailbyHistoryId")]
        public SalePriceListTemplate GetSalePriceListTemplateHistoryDetailbyHistoryId(int salePriceListTemplateHistoryId)
        {
            var manager = new SalePriceListTemplateManager();
            return manager.GetSalePriceListTemplateHistoryDetailbyHistoryId(salePriceListTemplateHistoryId);
        }
        [HttpGet]
        [Route("DownloadSalePriceListTemplate")]
        public object DownloadSalePriceListTemplate()
        {
            string templateRelativePath = "~/Client/Modules/WhS_BusinessEntity/Templates/Sale Pricelist Template.xlsx";
            string templateAbsolutePath = HttpContext.Current.Server.MapPath(templateRelativePath);
            byte[] bytes = System.IO.File.ReadAllBytes(templateAbsolutePath);

            var memoryStream = new System.IO.MemoryStream();
            memoryStream.Write(bytes, 0, bytes.Length);
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return GetExcelResponse(memoryStream, "Sale Pricelist Template.xlsx");
        }
    }
}