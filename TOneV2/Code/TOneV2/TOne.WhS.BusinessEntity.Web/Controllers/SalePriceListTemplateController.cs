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
			return GetWebResponse(input, manager.GetFilteredSalePriceListTemplates(input));
		}

		[HttpGet]
		[Route("GetSalePriceListTemplate")]
		public SalePriceListTemplate GetSalePriceListTemplate(int salePriceListTemplateId)
		{
			var manager = new SalePriceListTemplateManager();
			return manager.GetSalePriceListTemplate(salePriceListTemplateId);
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
		public IEnumerable<SalePriceListTemplateSettingsExtensionConfig> GetSalePriceListTemplateSettingsExtensionConfigs()
		{
			var manager = new SalePriceListTemplateManager();
			return manager.GetSalePriceListTemplateSettingsExtensionConfigs();
		}

		[HttpGet]
		[Route("GetBasicSettingsMappedValueExtensionConfigs")]
		public IEnumerable<BasicSalePriceListTemplateSettingsMappedValueExtensionConfig> GetBasicSettingsMappedValueExtensionConfigs()
		{
			var manager = new SalePriceListTemplateManager();
			return manager.GetBasicSettingsMappedValueExtensionConfigs();
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
	}
}