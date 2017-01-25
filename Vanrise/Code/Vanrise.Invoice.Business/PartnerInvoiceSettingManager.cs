using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Entities;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Invoice.Business.Context;
namespace Vanrise.Invoice.Business
{
    public class PartnerInvoiceSettingManager
    {
        #region Public Methods
        public PartnerInvoiceSetting GetPartnerInvoiceSetting(Guid partnerInvoiceSettingId)
        {
            var partnerInvoiceSettings = GetCachedPartnerInvoiceSettings();
            return partnerInvoiceSettings.GetRecord(partnerInvoiceSettingId);
        }
        public IDataRetrievalResult<PartnerInvoiceSettingDetail> GetFilteredPartnerInvoiceSettings(DataRetrievalInput<PartnerInvoiceSettingQuery> input)
        {
            var allItems = GetCachedPartnerInvoiceSettings();

            Func<PartnerInvoiceSetting, bool> filterExpression = (itemObject) =>
                 (input.Query.InvoiceSettingId == itemObject.InvoiceSettingID );

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, PartnerInvoiceSettingDetailMapper));
        }
        public Vanrise.Entities.InsertOperationOutput<PartnerInvoiceSettingDetail> AddPartnerInvoiceSetting(PartnerInvoiceSetting partnerInvoiceSetting)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<PartnerInvoiceSettingDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IPartnerInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IPartnerInvoiceSettingDataManager>();
            partnerInvoiceSetting.PartnerInvoiceSettingId = Guid.NewGuid();
            if (dataManager.InsertPartnerInvoiceSetting(partnerInvoiceSetting))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = PartnerInvoiceSettingDetailMapper(partnerInvoiceSetting);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<PartnerInvoiceSettingDetail> UpdatePartnerInvoiceSetting(PartnerInvoiceSetting partnerInvoiceSetting)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<PartnerInvoiceSettingDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IPartnerInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IPartnerInvoiceSettingDataManager>();

            if (dataManager.UpdatePartnerInvoiceSetting(partnerInvoiceSetting))
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = PartnerInvoiceSettingDetailMapper(partnerInvoiceSetting);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public IEnumerable<PartnerInvoiceSetting> GetPartnerInvoiceSettings()
        {
            return GetCachedPartnerInvoiceSettings().Values;
        }

        public bool CheckIfPartnerAssignedToInvoiceSetting(string partnerId)
        {
            var partnerInvoiceSettings = GetPartnerInvoiceSettings();
            if (partnerInvoiceSettings != null && partnerInvoiceSettings.Any(x => x.PartnerId == partnerId))
                return true;
            return false;
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IPartnerInvoiceSettingDataManager _dataManager = InvoiceDataManagerFactory.GetDataManager<IPartnerInvoiceSettingDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArePartnerInvoiceSettingsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods
        private Dictionary<Guid, PartnerInvoiceSetting> GetCachedPartnerInvoiceSettings()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPartnerInvoiceSettings",
              () =>
              {
                  IPartnerInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IPartnerInvoiceSettingDataManager>();
                  IEnumerable<PartnerInvoiceSetting> partnerInvoiceSettings = dataManager.GetPartnerInvoiceSettings();
                  return partnerInvoiceSettings.ToDictionary(c => c.PartnerInvoiceSettingId, c => c);
              });
        }
        #endregion

        #region Mappers

        private PartnerInvoiceSettingDetail PartnerInvoiceSettingDetailMapper(PartnerInvoiceSetting partnerInvoiceSettingObject)
        {
            InvoiceSettingManager invoiceSettingManager = new InvoiceSettingManager();
            var invoiceSetting = invoiceSettingManager.GetInvoiceSetting(partnerInvoiceSettingObject.InvoiceSettingID);
            InvoiceTypeManager invoiceTypeManager = new InvoiceTypeManager();
            var invoiceType = invoiceTypeManager.GetInvoiceType(invoiceSetting.InvoiceTypeId);
            PartnerNameManagerContext context =new PartnerNameManagerContext{
                PartnerId = partnerInvoiceSettingObject.PartnerId
            };
            string partnerName = null;
            if(invoiceType != null && invoiceType.Settings !=null && invoiceType.Settings.ExtendedSettings != null)
            {
                var invoicePartnerSettingManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
                if(invoicePartnerSettingManager != null )
                  partnerName = invoicePartnerSettingManager.GetPartnerName(context);
            }

            return new PartnerInvoiceSettingDetail
            {
                Entity = partnerInvoiceSettingObject,
                PartnerName = partnerName
            };
        }
        #endregion
    }
}
