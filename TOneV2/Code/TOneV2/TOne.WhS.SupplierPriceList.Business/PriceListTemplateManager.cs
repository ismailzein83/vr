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
    public class PriceListTemplateManager
    {
        #region Fields

        int _loggedInUserId = new Vanrise.Security.Business.SecurityContext().GetLoggedInUserId();

        #endregion

        #region Public Methods
        public PriceListTemplate GetPriceListTemplate(int priceListTemplateId)
        {
            Dictionary<int, PriceListTemplate> priceListTemplates = GetCachedPriceListTemplates();
            PriceListTemplate priceListTemplate = priceListTemplates.GetRecord(priceListTemplateId);
            return (priceListTemplate != null && priceListTemplate.UserId == _loggedInUserId) ? priceListTemplate : null;
        }
        public Vanrise.Entities.InsertOperationOutput<PriceListTemplateInfo> AddInputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            InsertOperationOutput<PriceListTemplateInfo> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<PriceListTemplateInfo>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int priceListTemplateId = -1;

            IPriceListTemplateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IPriceListTemplateDataManager>();
            priceListTemplate.UserId = _loggedInUserId;
            bool insertActionSucc = dataManager.InsertPriceListTemplate(priceListTemplate, out priceListTemplateId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                priceListTemplate.PriceListTemplateId = priceListTemplateId;
                insertOperationOutput.InsertedObject = PriceListTemplateInfoMapper(priceListTemplate);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<PriceListTemplateInfo> UpdateInputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            IPriceListTemplateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IPriceListTemplateDataManager>();
            priceListTemplate.UserId = _loggedInUserId;
            bool updateActionSucc = dataManager.UpdatePriceListTemplate(priceListTemplate);
            UpdateOperationOutput<PriceListTemplateInfo> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<PriceListTemplateInfo>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = PriceListTemplateInfoMapper(priceListTemplate);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public IEnumerable<PriceListTemplateInfo> GetInputPriceListTemplates(PriceListTemplateFilter filter)
        {
            Dictionary<int, PriceListTemplate> priceListTemplates = GetCachedPriceListTemplates();
            return priceListTemplates.Values.MapRecords(PriceListTemplateInfoMapper, itm => itm.UserId == _loggedInUserId);
        }

        public IEnumerable<PriceListInputConfig> GetInputPriceListConfigurationTemplateConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<PriceListInputConfig>(PriceListInputConfig.EXTENSION_TYPE);
        }
        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IPriceListTemplateDataManager _dataManager = SupPLDataManagerFactory.GetDataManager<IPriceListTemplateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArePriceListTemplatesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, PriceListTemplate> GetCachedPriceListTemplates()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPriceListTemplates", () =>
            {
                IPriceListTemplateDataManager dataManager = SupPLDataManagerFactory.GetDataManager<IPriceListTemplateDataManager>();
                IEnumerable<PriceListTemplate> priceListTemplates = dataManager.GetPriceListTemplates();
                return priceListTemplates.ToDictionary(kvp => kvp.PriceListTemplateId, kvp => kvp);
            });
        }

        #endregion

        #region Mapper

        private PriceListTemplateInfo PriceListTemplateInfoMapper(PriceListTemplate priceListTemplate)
        {
            return new PriceListTemplateInfo
            {
                Name = priceListTemplate.Name,
                PriceListTemplateId = priceListTemplate.PriceListTemplateId,
            };
        }
        #endregion
    }
}
