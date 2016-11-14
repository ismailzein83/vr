using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
	public class SalePriceListTemplateManager
	{
		#region Public Methods

		public Vanrise.Entities.IDataRetrievalResult<SalePriceListTemplateDetail> GetFilteredSalePriceListTemplates(Vanrise.Entities.DataRetrievalInput<SalePriceListTemplateQuery> input)
		{
			Dictionary<int, SalePriceListTemplate> cachedSalePriceListTemplates = GetCachedSalePriceListTemplates();
			Func<SalePriceListTemplate, bool> filterFunc = (salePriceListTemplate) => (input.Query.Name == null || salePriceListTemplate.Name.ToLower().Contains(input.Query.Name.ToLower()));
			return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedSalePriceListTemplates.ToBigResult(input, filterFunc, SalePriceListTemplateDetailMapper));
		}

		public SalePriceListTemplate GetSalePriceListTemplate(int salePriceListTemplateId)
		{
			return GetCachedSalePriceListTemplates().GetRecord(salePriceListTemplateId);
		}

		public IEnumerable<SalePriceListTemplateSettingsExtensionConfig> GetSalePriceListTemplateSettingsExtensionConfigs()
		{
			var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
			return extensionConfigManager.GetExtensionConfigurations<SalePriceListTemplateSettingsExtensionConfig>(SalePriceListTemplateSettingsExtensionConfig.EXTENSION_TYPE);
		}

		public IEnumerable<BasicSalePriceListTemplateSettingsMappedValueExtensionConfig> GetBasicSettingsMappedValueExtensionConfigs()
		{
			var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
			return extensionConfigManager.GetExtensionConfigurations<BasicSalePriceListTemplateSettingsMappedValueExtensionConfig>(BasicSalePriceListTemplateSettingsMappedValueExtensionConfig.EXTENSION_TYPE);
		}

		public Vanrise.Entities.InsertOperationOutput<SalePriceListTemplateDetail> AddSalePriceListTemplate(SalePriceListTemplate salePriceListTemplate)
		{
			var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SalePriceListTemplateDetail>();
			insertOperationOutput.InsertedObject = null;
			insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;

			var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListTemplateDataManager>();
			int insertedId;

			if (dataManager.Insert(salePriceListTemplate, out insertedId))
			{
				Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
				salePriceListTemplate.SalePriceListTemplateId = insertedId;
				insertOperationOutput.InsertedObject = SalePriceListTemplateDetailMapper(salePriceListTemplate);
				insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
			}
			else
			{
				insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
			}

			return insertOperationOutput;
		}

		public Vanrise.Entities.UpdateOperationOutput<SalePriceListTemplateDetail> UpdateSalePriceListTemplate(SalePriceListTemplate salePriceListTemplate)
		{
			var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SalePriceListTemplateDetail>();
			updateOperationOutput.UpdatedObject = null;
			updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;

			var dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListTemplateDataManager>();

			if (dataManager.Update(salePriceListTemplate))
			{
				Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
				updateOperationOutput.UpdatedObject = SalePriceListTemplateDetailMapper(salePriceListTemplate);
				updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
			}
			else
			{
				updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
			}

			return updateOperationOutput;
		}

		#endregion

		#region Private Classes

		private class CacheManager : Vanrise.Caching.BaseCacheManager
		{
			ISalePriceListTemplateDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListTemplateDataManager>();
			object _updateHandle;

			protected override bool ShouldSetCacheExpired()
			{
				return _dataManager.AreSalePriceListTemplatesUpdated(ref _updateHandle);
			}
		}

		#endregion

		#region Private Methods

		Dictionary<int, SalePriceListTemplate> GetCachedSalePriceListTemplates()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSalePriceListTemplates", () =>
			{
				ISalePriceListTemplateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListTemplateDataManager>();
				IEnumerable<SalePriceListTemplate> salePriceListTemplates = dataManager.GetAll();
				return salePriceListTemplates.ToDictionary(x => x.SalePriceListTemplateId);
			});
		}

		#endregion

		#region Mappers

		private SalePriceListTemplateDetail SalePriceListTemplateDetailMapper(SalePriceListTemplate salePriceListTemplate)
		{
			return new SalePriceListTemplateDetail()
			{
				Entity = salePriceListTemplate
			};
		}

		#endregion
	}
}
