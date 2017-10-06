using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business 
{
    public class PricingTemplateManager
    {
        #region Public Methods

        public IDataRetrievalResult<PricingTemplateDetail> GetFilteredPricingTemplates(DataRetrievalInput<PricingTemplateQuery> input)
        {
            var allPricingTemplates = this.GetCachedPricingTemplates();

            Func<PricingTemplate, bool> filterExpression = (x) =>
            {
                if (!string.IsNullOrEmpty(input.Query.Name) && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                return true;
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allPricingTemplates.ToBigResult(input, filterExpression, PricingTemplateDetailMapper));
        }

        public PricingTemplate GetPricingTemplate(int pricingTemplateId)
        {
            var cachedPricingTemplates = GetCachedPricingTemplates();
            return cachedPricingTemplates.GetRecord(pricingTemplateId);
        }

        public PricingTemplateEditorRuntime GetPricingTemplateEditorRuntime(int pricingTemplateId)
        {
            PricingTemplate pricingTemplate = this.GetPricingTemplate(pricingTemplateId);
            
            return new PricingTemplateEditorRuntime()
            {
                Entity = pricingTemplate,
                RulesEditorRuntime = this.GetPricingTemplateRuleEditorRuntime(pricingTemplate)
            };
        }

        public Vanrise.Entities.InsertOperationOutput<PricingTemplateDetail> AddPricingTemplate(PricingTemplate pricingTemplate)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<PricingTemplateDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IPricingTemplateDataManager dataManager = SalesDataManagerFactory.GetDataManager<IPricingTemplateDataManager>();

            int pricingTemplateId;

            if (dataManager.Insert(pricingTemplate, out pricingTemplateId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                pricingTemplate.PricingTemplateId = pricingTemplateId;
                insertOperationOutput.InsertedObject = PricingTemplateDetailMapper(pricingTemplate);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<PricingTemplateDetail> UpdatePricingTemplate(PricingTemplateToEdit pricingTemplateToEdit)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<PricingTemplateDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IPricingTemplateDataManager dataManager = SalesDataManagerFactory.GetDataManager<IPricingTemplateDataManager>();

            if (dataManager.Update(pricingTemplateToEdit))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = PricingTemplateDetailMapper(this.GetPricingTemplate(pricingTemplateToEdit.PricingTemplateId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<MarginRateCalculationConfig> GetMarginRateCalculationExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<MarginRateCalculationConfig>(MarginRateCalculationConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Methods

        private Dictionary<int, PricingTemplate> GetCachedPricingTemplates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPricingTemplates",
                    () =>
                    {
                        IPricingTemplateDataManager dataManager = SalesDataManagerFactory.GetDataManager<IPricingTemplateDataManager>();
                        return dataManager.GetPricingTemplates().ToDictionary(itm => itm.PricingTemplateId , itm => itm);
                    });
        }

        private PricingTemplateRulesEditorRuntime GetPricingTemplateRuleEditorRuntime(PricingTemplate pricingTemplate)
        {
            pricingTemplate.ThrowIfNull("pricingTemplate");
            pricingTemplate.Settings.ThrowIfNull("pricingTemplate.Settings");
            pricingTemplate.Settings.Rules.ThrowIfNull("pricingTemplate.Settings.Rules");

            Dictionary<int, string> CountryNameByIds = new Dictionary<int, string>();
            Dictionary<long, string> ZoneNameByIds = new Dictionary<long, string>();

            var countryManager = new Vanrise.Common.Business.CountryManager();
            var saleZoneManager = new TOne.WhS.BusinessEntity.Business.SaleZoneManager();

            foreach (var pricingTemplateRule in pricingTemplate.Settings.Rules)
            {
                if (pricingTemplateRule.Countries != null)
                {
                    foreach (var country in pricingTemplateRule.Countries)
                    {
                        if (!CountryNameByIds.ContainsKey(country.CountryId))
                        {
                            string countryName = countryManager.GetCountryName(country.CountryId);
                            CountryNameByIds.Add(country.CountryId, countryName);
                        }
                    }
                }

                if (pricingTemplateRule.Zones != null)
                {
                    foreach (var zoneId in pricingTemplateRule.Zones.SelectMany(itm => itm.IncludedZoneIds))
                    {
                        if (!ZoneNameByIds.ContainsKey(zoneId))
                        {
                            string zoneName = saleZoneManager.GetSaleZoneName(zoneId);
                            ZoneNameByIds.Add(zoneId, zoneName);
                        }
                    }
                }
            }

            return new PricingTemplateRulesEditorRuntime() { CountryNameByIds = CountryNameByIds, ZoneNameByIds = ZoneNameByIds };
        }


        #endregion 

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IPricingTemplateDataManager _dataManager = SalesDataManagerFactory.GetDataManager<IPricingTemplateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArePricingTemplatesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        public PricingTemplateDetail PricingTemplateDetailMapper(PricingTemplate pricingTemplate)
        {
            PricingTemplateDetail pricingTemplateDetail = new PricingTemplateDetail()
            {
                Entity = pricingTemplate
            };
            return pricingTemplateDetail;
        }

        #endregion
    }
}
