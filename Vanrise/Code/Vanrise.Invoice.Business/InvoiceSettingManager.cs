using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Invoice.Data;
using Vanrise.Caching;
using Vanrise.Security.Business;
namespace Vanrise.Invoice.Business
{
    public class InvoiceSettingManager : IInvoiceSettingManager
    {
        #region Public Methods
        public InvoiceSetting GetInvoiceSetting(Guid invoiceSettingId)
        {
            var invoiceSettings = GetCachedInvoiceSettings();
            return invoiceSettings.GetRecord(invoiceSettingId);
        }
        public IDataRetrievalResult<InvoiceSettingDetail> GetFilteredInvoiceSettings(DataRetrievalInput<InvoiceSettingQuery> input)
        {
            var allItems = GetCachedInvoiceSettings();

            Func<InvoiceSetting, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 && (input.Query.InvoiceTypeId == itemObject.InvoiceTypeId);

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, InvoiceSettingDetailMapper));
        }
        public Vanrise.Entities.InsertOperationOutput<InvoiceSettingDetail> AddInvoiceSetting(InvoiceSetting invoiceSetting)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<InvoiceSettingDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceSettingDataManager>();
            invoiceSetting.InvoiceSettingId = Guid.NewGuid();
            if (dataManager.InsertInvoiceSetting(invoiceSetting))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = InvoiceSettingDetailMapper(invoiceSetting);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<InvoiceSettingDetail> UpdateInvoiceSetting(InvoiceSetting invoiceSetting)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<InvoiceSettingDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceSettingDataManager>();

            if (dataManager.UpdateInvoiceSetting(invoiceSetting))
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = InvoiceSettingDetailMapper(invoiceSetting);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
       
        public IEnumerable<InvoiceSettingInfo> GetInvoiceSettingsInfo(InvoiceSettingFilter filter)
        {
            var invoiceSettings = GetCachedInvoiceSettings();
            if (filter != null)
            {
            }
            return invoiceSettings.MapRecords(InvoiceSettingInfoMapper);
        }
        public IEnumerable<InvoiceSetting> GetInvoiceSettings(Guid invoiceTypeId)
        {
            return GetCachedInvoiceSettings().FindAllRecords(x=>x.InvoiceTypeId == invoiceTypeId);
        }
        public InvoiceSetting GetDefaultInvoiceSetting(Guid invoiceTypeId)
        {
            var invoiceSettings = GetInvoiceSettings(invoiceTypeId);
            foreach(var item in invoiceSettings)
            {
                if (item.Details.IsDefault)
                    return item;
            }
            throw new NullReferenceException(string.Format("No default setting available for invoicetypeid {0}", invoiceTypeId));
        }
        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IInvoiceSettingDataManager _dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceSettingDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreInvoiceSettingsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods
        private Dictionary<Guid, InvoiceSetting> GetCachedInvoiceSettings()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetInvoiceSettings",
              () =>
              {
                  IInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceSettingDataManager>();
                  IEnumerable<InvoiceSetting> invoiceSettings = dataManager.GetInvoiceSettings();
                  return invoiceSettings.ToDictionary(c => c.InvoiceSettingId, c => c);
              });
        }
     
        #endregion

        #region Mappers
        private InvoiceSettingDetail InvoiceSettingDetailMapper(InvoiceSetting invoiceSettingObject)
        {
            InvoiceSettingDetail invoiceSettingDetail = new InvoiceSettingDetail();
            invoiceSettingDetail.Entity = invoiceSettingObject;
            return invoiceSettingDetail;
        }
        private InvoiceSettingInfo InvoiceSettingInfoMapper(InvoiceSetting invoiceSettingObject)
        {
            return new InvoiceSettingInfo
            {
                InvoiceSettingId = invoiceSettingObject.InvoiceSettingId,
                Name = invoiceSettingObject.Name
            };
        }
        #endregion

        
    }
}
