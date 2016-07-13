using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Data;
using TOne.WhS.SupplierPriceList.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace TOne.WhS.SupplierPriceList.Business
{
    public class SupplierPriceListTemplateManager
    {
        #region Public Methods
        public SupplierPriceListTemplate GetSupplierPriceListTemplate(int priceListTemplateId)
        {
            Dictionary<int, SupplierPriceListTemplate> priceListTemplates = GetCachedSupplierPriceListTemplates();
            SupplierPriceListTemplate supplierPriceListTemplate = priceListTemplates.GetRecord(priceListTemplateId);
            return (supplierPriceListTemplate != null) ? supplierPriceListTemplate : null;
        }
        public SupplierPriceListTemplate GetSupplierPriceListTemplateBySupplierId(int supplierId)
        {
            Dictionary<int, SupplierPriceListTemplate> priceListTemplates = GetCachedSupplierPriceListTemplatesBySupplierId();
            SupplierPriceListTemplate supplierPriceListTemplate = priceListTemplates.GetRecord(supplierId);
            return (supplierPriceListTemplate != null) ? supplierPriceListTemplate : null;
        }

        public SupplierPriceListSettings GetSupplierPriceListTemplateSettings(int priceListTemplateId, bool getDraftIfExists)
        {
            Dictionary<int, SupplierPriceListTemplate> priceListTemplates = GetCachedSupplierPriceListTemplates();
            SupplierPriceListTemplate supplierPriceListTemplate = priceListTemplates.GetRecord(priceListTemplateId);

            SupplierPriceListSettings settings = supplierPriceListTemplate.ConfigDetails;
            if (getDraftIfExists && supplierPriceListTemplate.Draft != null)
            {
                settings = supplierPriceListTemplate.Draft;
            }
            return settings;
        }

        public Vanrise.Entities.InsertOperationOutput<SupplierPriceListTemplate> AddSupplierPriceListTemplate(SupplierPriceListTemplate supplierPriceListTemplate)
        {
            InsertOperationOutput<SupplierPriceListTemplate> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<SupplierPriceListTemplate>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int supplierPriceListTemplateId = -1;

            ISupplierPriceListTemplateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListTemplateDataManager>();
            bool insertActionSucc = dataManager.InsertSupplierPriceListTemplate(supplierPriceListTemplate, out supplierPriceListTemplateId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                supplierPriceListTemplate.SupplierPriceListTemplateId = supplierPriceListTemplateId;
                insertOperationOutput.InsertedObject = supplierPriceListTemplate;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<SupplierPriceListTemplate> UpdateInputPriceListTemplate(SupplierPriceListTemplate supplierPriceListTemplate)
        {
            ISupplierPriceListTemplateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListTemplateDataManager>();
            bool updateActionSucc = dataManager.UpdateSupplierPriceListTemplate(supplierPriceListTemplate);
            UpdateOperationOutput<SupplierPriceListTemplate> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<SupplierPriceListTemplate>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = supplierPriceListTemplate;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public IEnumerable<SupplierPriceListInputConfig> GetSupplierPriceListConfigurationTemplateConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<SupplierPriceListInputConfig>(SupplierPriceListInputConfig.EXTENSION_TYPE);
        }
        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierPriceListTemplateDataManager _dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListTemplateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSupplierPriceListTemplatesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, SupplierPriceListTemplate> GetCachedSupplierPriceListTemplates()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSupplierPriceListTemplates", () =>
            {
                ISupplierPriceListTemplateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListTemplateDataManager>();
                IEnumerable<SupplierPriceListTemplate> priceListTemplates = dataManager.GetSupplierPriceListTemplates();
                return priceListTemplates.ToDictionary(kvp => kvp.SupplierPriceListTemplateId, kvp => kvp);
            });
        }
        private Dictionary<int, SupplierPriceListTemplate> GetCachedSupplierPriceListTemplatesBySupplierId()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSupplierPriceListTemplatesBySupplierId", () =>
            {
                ISupplierPriceListTemplateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<ISupplierPriceListTemplateDataManager>();
                IEnumerable<SupplierPriceListTemplate> priceListTemplates = dataManager.GetSupplierPriceListTemplates();
                return priceListTemplates.ToDictionary(kvp => kvp.SupplierId, kvp => kvp);
            });
        }
        #endregion
    }
}
