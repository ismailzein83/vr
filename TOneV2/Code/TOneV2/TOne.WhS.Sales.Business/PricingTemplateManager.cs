using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class PricingTemplateManager
    {
        public PricingTemplate GetPricingTemplate(int pricingTemplateId)
        {
            var cachedPricingTemplates = GetCachedPricingTemplates();
            return cachedPricingTemplates.GetRecord(pricingTemplateId);
        }

        Dictionary<int, PricingTemplate> GetCachedPricingTemplates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPricingTemplates",
               () =>
               {
                   return new Dictionary<int, PricingTemplate>();
               });
        }

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IPricingTemplateDataManager _dataManager = SalesDataManagerFactory.GetDataManager<IPricingTemplateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArePricingTemplatesUpdated(ref _updateHandle);
            }
        }
    }
}
