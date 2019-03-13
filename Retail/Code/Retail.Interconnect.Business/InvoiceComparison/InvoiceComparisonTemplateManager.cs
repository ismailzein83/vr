using Retail.Interconnect.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Caching;
using Retail.Interconnect.Data;

namespace Retail.Interconnect.Business
{
    public class InvoiceComparisonTemplateManager
    {
        #region Public Methods
        public bool TryAddOrUpdateInvoiceCompareTemplate(InvoiceComparisonTemplate invoiceCompareTemplate)
        {
            IInterconnectInvoiceComparisonTemplateDataManager dataManager = InterconnectInvoiceManagerFactory.GetDataManager<IInterconnectInvoiceComparisonTemplateDataManager>();
            return dataManager.TryAddOrUpdateInvoiceCompareTemplate(invoiceCompareTemplate);
        }
        public InvoiceComparisonTemplate GetInvoiceCompareTemplate(Guid invoiceTypeId, string partnerId)
        {
            var invoiceComparisonTemplates = this.GetCachedInvoiceComparisonTemplates();
            return invoiceComparisonTemplates.Values.FindRecord(x => x.InvoiceTypeId == invoiceTypeId && x.PartnerId == partnerId);
        }

        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IInterconnectInvoiceComparisonTemplateDataManager dataManager = InterconnectInvoiceManagerFactory.GetDataManager<IInterconnectInvoiceComparisonTemplateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return dataManager.AreInvoiceComparisonTemplatesUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods
        Dictionary<long, InvoiceComparisonTemplate> GetCachedInvoiceComparisonTemplates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetInvoiceCompareTemplates",
               () =>
               {
                   IInterconnectInvoiceComparisonTemplateDataManager dataManager = InterconnectInvoiceManagerFactory.GetDataManager<IInterconnectInvoiceComparisonTemplateDataManager>();
                   IEnumerable<InvoiceComparisonTemplate> invoiceComparisonTemplates = dataManager.GetInvoiceCompareTemplates();
                   return invoiceComparisonTemplates.ToDictionary(cn => cn.InvoiceComparisonTemplateId, cn => cn);
               });
        }
        #endregion
    }
}

