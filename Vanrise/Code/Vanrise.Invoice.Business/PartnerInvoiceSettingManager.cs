﻿using System;
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
    public class PartnerInvoiceSettingManager : IPartnerInvoiceSettingManager
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
            if (!CheckIfPartnerAssignedToInvoiceSetting(partnerInvoiceSetting.PartnerId, partnerInvoiceSetting.InvoiceSettingID))
            {
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
            }else
            {
                insertOperationOutput.Message = "Partner already linked to another invoice setting.";
                insertOperationOutput.ShowExactMessage = true;
            }
            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<PartnerInvoiceSettingDetail> UpdatePartnerInvoiceSetting(PartnerInvoiceSetting partnerInvoiceSetting)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<PartnerInvoiceSettingDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            if (!CheckIfPartnerAssignedToInvoiceSetting(partnerInvoiceSetting.PartnerId, partnerInvoiceSetting.InvoiceSettingID, partnerInvoiceSetting.PartnerInvoiceSettingId))
            {
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
            }
            else
            {
                updateOperationOutput.Message = "Partner already linked to another invoice setting.";
                updateOperationOutput.ShowExactMessage = true;
            }

            return updateOperationOutput;
        }
        public Vanrise.Entities.DeleteOperationOutput<object> DeletePartnerInvoiceSetting(Guid partnerInvoiceSettingId)
        {
            var deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<object>()
            {
                Result = Vanrise.Entities.DeleteOperationResult.Failed
            };

            IPartnerInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IPartnerInvoiceSettingDataManager>();
            bool deleted = dataManager.DeletePartnerInvoiceSetting(partnerInvoiceSettingId);
            if (deleted)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return deleteOperationOutput;
        }

        public IEnumerable<PartnerInvoiceSetting> GetPartnerInvoiceSettings(Guid invoiceTypeId)
        {
            return GetPartnerInvoiceSettingsByInvoiceTypeId(invoiceTypeId);
        }
        public PartnerInvoiceSetting GetPartnerInvoiceSettingByPartnerId(string partnerId,Guid invoiceTypeId)
        {
            var partnerInvoiceSettings = GetPartnerInvoiceSettings(invoiceTypeId);
            if (partnerInvoiceSettings == null)
                return null;
            return partnerInvoiceSettings.FirstOrDefault(x => x.PartnerId == partnerId);
        }
        public bool LinkPartnerToInvoiceSetting(Guid partnerInvoiceSettingId,string partnerId, Guid invoiceSettingId)
        {            
            IPartnerInvoiceSettingDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IPartnerInvoiceSettingDataManager>();
            return dataManager.InsertOrUpdateInvoiceSetting(partnerInvoiceSettingId, partnerId, invoiceSettingId);
        }
        public T GetPartnerInvoiceSettingDetailByType<T>(Guid partnerInvoiceSettingId) where T : InvoiceSettingPart
        {
            var partnerInvoiceSettings = GetPartnerInvoiceSetting(partnerInvoiceSettingId);
            var configClass = Activator.CreateInstance(typeof(T)) as InvoiceSettingPart;
            if (partnerInvoiceSettings.Details != null && partnerInvoiceSettings.Details.InvoiceSettingParts != null)
            {
                InvoiceSettingPart invoiceSettingPart;
                if (partnerInvoiceSettings.Details.InvoiceSettingParts.TryGetValue(configClass.ConfigId, out invoiceSettingPart))
                    return invoiceSettingPart as T;
            }
            return null;
        }
        public bool CheckIfPartnerAssignedToInvoiceSetting(string partnerId, Guid invoiceSettingId,  Guid? partnerInvoiceSettingId = null)
        {
            InvoiceSettingManager invoiceSettingManager = new InvoiceSettingManager();
            var invoiceTypeId = invoiceSettingManager.GetSettingInvoiceTypeId(invoiceSettingId);
            var partnerInvoiceSettings = GetPartnerInvoiceSettingsByInvoiceTypeId(invoiceTypeId);
            if (partnerInvoiceSettings != null && partnerInvoiceSettings.Any(x => x.PartnerId == partnerId && (!partnerInvoiceSettingId.HasValue || partnerInvoiceSettingId.Value != x.PartnerInvoiceSettingId)))
                return true;
            return false;
        }
        public List<PartnerInvoiceSetting> GetPartnerInvoiceSettingsByInvoiceTypeId(Guid invoiceTypeId)
        {
            var partnerInvoiceSettingsByInvoiceType = GetCachedPartnerInvoiceSettingsByInvoiceTypeId();
            return partnerInvoiceSettingsByInvoiceType.GetRecord(invoiceTypeId);
        }
        public bool CheckIfInvoiceSettingHasLinkedPartners(Guid invoiceSettingId)
        {
            var cachedPartnerInvoiceSettings = GetCachedPartnerInvoiceSettings();
            return cachedPartnerInvoiceSettings.Any(x => x.Value.InvoiceSettingID == invoiceSettingId);
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
        private Dictionary<Guid, List<PartnerInvoiceSetting>> GetCachedPartnerInvoiceSettingsByInvoiceTypeId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedPartnerInvoiceSettingsByInvoiceTypeId",
              () =>
              {
                  Dictionary<Guid, List<PartnerInvoiceSetting>> partnerInvoiceSettingsByInvoiceType = new Dictionary<Guid, List<PartnerInvoiceSetting>>();
                  InvoiceSettingManager invoiceSettingManager = new InvoiceSettingManager();
                  var cachedPartnerInvoiceSettings = GetCachedPartnerInvoiceSettings();
                  foreach (var cachedPartnerInvoiceSetting in cachedPartnerInvoiceSettings)
                  {
                      var settingInvoiceTypeId = invoiceSettingManager.GetSettingInvoiceTypeId(cachedPartnerInvoiceSetting.Value.InvoiceSettingID);
                      var partnerInvoiceSettings = partnerInvoiceSettingsByInvoiceType.GetOrCreateItem(settingInvoiceTypeId);
                      partnerInvoiceSettings.Add(cachedPartnerInvoiceSetting.Value);
                  }
                  return partnerInvoiceSettingsByInvoiceType;
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

            PartnerInvoiceSettingDetail partnerInvoiceSettingDetail = new Entities.PartnerInvoiceSettingDetail {

                Entity = partnerInvoiceSettingObject
            };
            if(invoiceType != null && invoiceType.Settings !=null && invoiceType.Settings.ExtendedSettings != null)
            {
                var invoicePartnerSettingManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
                if(invoicePartnerSettingManager != null )
                    partnerInvoiceSettingDetail.PartnerName = invoicePartnerSettingManager.GetPartnerName(context);
                var invoiceAccountInfo = invoicePartnerSettingManager.GetInvoiceAccountData(new Context.InvoiceAccountDataContext
                {
                    PartnerId = partnerInvoiceSettingObject.PartnerId,
                    InvoiceTypeId= invoiceType.InvoiceTypeId
                });
                if(invoiceAccountInfo != null)
                {
                    partnerInvoiceSettingDetail.BED = invoiceAccountInfo.BED;
                    partnerInvoiceSettingDetail.EED = invoiceAccountInfo.EED;
                    partnerInvoiceSettingDetail.StatusDescription = Utilities.GetEnumDescription(invoiceAccountInfo.Status);
                }
            }
            return partnerInvoiceSettingDetail;
        }
        #endregion
    }
}
