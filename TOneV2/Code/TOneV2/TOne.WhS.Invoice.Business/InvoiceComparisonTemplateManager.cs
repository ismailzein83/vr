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
          return invoiceComparisonTemplates.Values.FindRecord(x => x.InvoiceTypeId == invoiceTypeId && x.PartnerId == partnerId);
      }
      public ComparisonInvoiceDetail GetComparisonInvoiceDetail(long invoiceId)
      {
          Vanrise.Invoice.Entities.InvoiceDetail invoice = new Vanrise.Invoice.Entities.InvoiceDetail();
          Vanrise.Invoice.Business.InvoiceManager manager = new Vanrise.Invoice.Business.InvoiceManager();
           invoice = manager.GetInvoiceDetail(invoiceId);
           string timezone;
           if (invoice.Entity.Details.TimeZoneId != null)
           {
               Vanrise.Entities.VRTimeZone timeZone = new Vanrise.Common.Business.VRTimeZoneManager().GetVRTimeZone(invoice.Entity.Details.TimeZoneId);
               timezone = timeZone.Name;
           }
           else timezone = "";

           
          
          return new ComparisonInvoiceDetail() 
          {To= invoice.PartnerName ,
           toDate=invoice.Entity.ToDate,
           dueDate=invoice.Entity.DueDate,
           calls = invoice.Entity.Details.TotalNumberOfCalls,
           fromDate = invoice.Entity.FromDate,
           issuedDate = invoice.Entity.IssueDate,
           serialNumber = invoice.Entity.SerialNumber,
           duration = invoice.Entity.Details.Duration,
           totalAmount = invoice.Entity.Details.TotalAmount,
           isLocked = invoice.Lock,
           isPaid = invoice.Paid,
           issuedBy = invoice.UserName,
           PartnerId= invoice.Entity.PartnerId,
           currency= invoice.Entity.Details.SupplierCurrency,
           timeZone = timezone



             
          };
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
