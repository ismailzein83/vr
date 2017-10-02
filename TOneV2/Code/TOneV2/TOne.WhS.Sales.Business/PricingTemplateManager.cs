using System;
using System.Collections.Generic;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
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

        public string GetPricingTemplateName(PricingTemplate pricingTemplate)
        {
            return pricingTemplate != null ? pricingTemplate.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<PricingTemplateDetail> AddPricingTemplate(PricingTemplate pricingTemplateItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<PricingTemplateDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IPricingTemplateDataManager dataManager = SalesDataManagerFactory.GetDataManager<IPricingTemplateDataManager>();

            int pricingTemplateId;

            if (dataManager.Insert(pricingTemplateItem, out pricingTemplateId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                pricingTemplateItem.PricingTemplateID = pricingTemplateId;
                insertOperationOutput.InsertedObject = PricingTemplateDetailMapper(pricingTemplateItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<PricingTemplateDetail> UpdatePricingTemplate(PricingTemplate pricingTemplateItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<PricingTemplateDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IPricingTemplateDataManager dataManager = SalesDataManagerFactory.GetDataManager<IPricingTemplateDataManager>();

            if (dataManager.Update(pricingTemplateItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = PricingTemplateDetailMapper(pricingTemplateItem);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods

        Dictionary<int, PricingTemplate> GetCachedPricingTemplates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPricingTemplates",
               () =>
               {
                   return new Dictionary<int, PricingTemplate>();
               });
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
