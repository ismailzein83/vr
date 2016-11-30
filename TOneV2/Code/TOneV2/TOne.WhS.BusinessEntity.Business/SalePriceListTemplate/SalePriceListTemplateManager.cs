using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

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

		public IEnumerable<SalePriceListTemplateInfo> GetSalePriceListTemplatesInfo()
		{
			return GetCachedSalePriceListTemplates().MapRecords(SalePriceListTemplateInfoMapper).OrderBy(x => x.Name);
		}

		public IEnumerable<SalePriceListTemplateSettingsExtensionConfig> GetSalePriceListTemplateSettingsExtensionConfigs()
		{
			var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
			return extensionConfigManager.GetExtensionConfigurations<SalePriceListTemplateSettingsExtensionConfig>(SalePriceListTemplateSettingsExtensionConfig.EXTENSION_TYPE);
		}

        public IEnumerable<SalePriceListTemplateSettingsMappedTableExtensionConfig> GetMappedTablesExtensionConfigs()
		{
			var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<SalePriceListTemplateSettingsMappedTableExtensionConfig>(SalePriceListTemplateSettingsMappedTableExtensionConfig.EXTENSION_TYPE);
		}
        public IEnumerable<BasicMappedValueExtensionConfig> GetBasicSettingsMappedValueExtensionConfigs(string configType)
		{
			var extensionConfigManager = new Vanrise.Common.Business.ExtensionConfigurationManager();
            return extensionConfigManager.GetExtensionConfigurations<BasicMappedValueExtensionConfig>(configType);
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

		private Dictionary<int, SalePriceListTemplate> GetCachedSalePriceListTemplates()
		{
			return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSalePriceListTemplates", () =>
			{
				ISalePriceListTemplateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISalePriceListTemplateDataManager>();
				IEnumerable<SalePriceListTemplate> salePriceListTemplates = dataManager.GetAll();
				return salePriceListTemplates.ToDictionary(x => x.SalePriceListTemplateId);
			});
		}

		private IEnumerable<SalePLZoneNotification> GetZoneNotifications()
		{
			var zones = new List<SalePLZoneNotification>();
			for (int i = 0; i < 3; i++)
			{
				var zone = new SalePLZoneNotification()
				{
					ZoneName = "Zone " + (i + 1)
				};

				zone.Codes = new List<SalePLCodeNotification>();
				zone.Codes.Add(new SalePLCodeNotification()
				{
					Code = "Code " + (i + 1),
					BED = DateTime.Now.Date
				});

				zone.Rate = new SalePLRateNotification()
				{
					Rate = (i + 1),
					BED = DateTime.Now.Date
				};

				zones.Add(zone);
			}
			return zones;
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

		private SalePriceListTemplateInfo SalePriceListTemplateInfoMapper(SalePriceListTemplate entity)
		{
			return new SalePriceListTemplateInfo()
			{
				SalePriceListTemplateId = entity.SalePriceListTemplateId,
				Name = entity.Name
			};
		}

		#endregion
	}
}
