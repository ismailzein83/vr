using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.Invoice.Data;
using Vanrise.Common;

namespace TOne.WhS.Invoice.Business
{
  public  class InvoiceComparisonTemplateManager
  {
      #region Public Methods
      public bool TryAddOrUpdateInvoiceCompareTemplate(InvoiceComparisonTemplate invoiceCompareTemplate)
      {
          IInvoiceComparisonTemplateDataManager dataManager = InvoiceManagerFactory.GetDataManager<IInvoiceComparisonTemplateDataManager>();
         return dataManager.TryAddOrUpdateInvoiceCompareTemplate(invoiceCompareTemplate);
      }
      public InvoiceComparisonTemplate GetInvoiceCompareTemplate(Guid invoiceTypeId, string partnerId)
      {
          var invoiceComparisonTemplates = this.GetCachedInvoiceComparisonTemplates();
          return invoiceComparisonTemplates.Values.FindRecord(x => x.InvoiceTypeId == invoiceTypeId && x.PartnerId == partnerId );
      }
      
      #endregion

      #region Private Classes
      private class CacheManager : Vanrise.Caching.BaseCacheManager
      {
          IInvoiceComparisonTemplateDataManager dataManager = InvoiceManagerFactory.GetDataManager<IInvoiceComparisonTemplateDataManager>();
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
                 IInvoiceComparisonTemplateDataManager dataManager = InvoiceManagerFactory.GetDataManager<IInvoiceComparisonTemplateDataManager>();
                 IEnumerable<InvoiceComparisonTemplate> invoiceComparisonTemplates = dataManager.GetInvoiceCompareTemplates();
                 return invoiceComparisonTemplates.ToDictionary(cn => cn.InvoiceComparisonTemplateId, cn => cn);
             });
      }
      #endregion
    }
}
