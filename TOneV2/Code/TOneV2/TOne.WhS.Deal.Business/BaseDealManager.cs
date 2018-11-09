﻿using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.Entities;
using TOne.WhS.Deal.Data;
using TOne.WhS.Deal.Entities;
using Vanrise.Common.Business;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Deal.Business
{
    public abstract class BaseDealManager : BaseBusinessEntityManager
    {
        #region Public Methods
        public DealDefinition GetDeal(int dealId)
        {
            Dictionary<int, DealDefinition> cachedEntities = this.GetCachedDealsWithDeleted();
            return cachedEntities.GetRecord(dealId);
        }

        public string GetDealName(DealDefinition dealDefinition)
        {
            return dealDefinition != null ? dealDefinition.Name : null;
        }

        public string GetDealName(int dealId)
        {
            DealDefinition dealDefinition = GetDeal(dealId);
            return dealDefinition != null ? dealDefinition.Name : null;
        }

        public Vanrise.Entities.InsertOperationOutput<DealDefinitionDetail> AddDeal(DealDefinition deal)
        {
            var context = new ValidateBeforeSaveContext
            {
                IsEditMode = false,
                DealSaleZoneIds = deal.Settings.GetDealSaleZoneIds(),
                DealSupplierZoneIds = deal.Settings.GetDealSupplierZoneIds()
            };
            var insertOperationOutput = new InsertDealOperationOutput<DealDefinitionDetail>
            {
                Result = Vanrise.Entities.InsertOperationResult.Failed,
                InsertedObject = null
            };
            IDealDataManager dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();

            int insertedId = -1;

            TimeSpan? offSet = deal.Settings.GetCarrierOffSet();
            deal.Settings.OffSet = offSet;

            if (deal.Settings.ValidateDataBeforeSave(context))
            {
                deal.Settings.IsRecurrable = true;
                if (dataManager.Insert(deal, out insertedId))
                {
                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    deal.DealId = insertedId;
                    VRActionLogger.Current.TrackAndLogObjectAdded(GetLoggableEntity(), deal);

                    insertOperationOutput.InsertedObject = DealDeinitionDetailMapper(deal);
                }
                else
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
                }
            }
            else
            {
                insertOperationOutput.ValidationMessages = context.ValidateMessages;
            }
            return insertOperationOutput;
        }

        public bool DeleteDeal(int dealId)
        {
            IDealDataManager dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();
            return dataManager.Delete(dealId);
        }

        public Vanrise.Entities.UpdateOperationOutput<DealDefinitionDetail> UpdateDeal(DealDefinition deal)
        {
            var existingDealDefinition = GetDeal(deal.DealId);
            UpdateDealOperationOutput updateOperationOutput = new UpdateDealOperationOutput();
            ValidateBeforeSaveContext context = new ValidateBeforeSaveContext
            {
                DealId = deal.DealId,
                IsEditMode = true,
                ExistingDeal = deal,
                DealSaleZoneIds = deal.Settings.GetDealSaleZoneIds()
            };

            TimeSpan? offSet = deal.Settings.GetCarrierOffSet();
            if (existingDealDefinition.Settings.OffSet == null)
                deal.Settings.OffSet = offSet;

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IDealDataManager dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();
            if (deal.Settings.ValidateDataBeforeSave(context))
            {
                if (dataManager.Update(deal))
                {
                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    var dealEntity = this.GetDeal(deal.DealId);
                    VRActionLogger.Current.TrackAndLogObjectUpdated(GetLoggableEntity(), dealEntity);
                    updateOperationOutput.UpdatedObject = DealDeinitionDetailMapper(dealEntity);
                }
                else
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }
            }
            else
            {
                updateOperationOutput.ValidationMessages = context.ValidateMessages;
            }
            return updateOperationOutput;
        }

        public T GetDealSettings<T>(int dealId) where T : DealSettings
        {
            DealDefinition deal = GetDeal(dealId);
            deal.ThrowIfNull("deal", dealId);

            T dealSettings = deal.Settings.CastWithValidate<T>("deal.Settings", dealId);
            return dealSettings;
        }

        public abstract DealDefinitionDetail DealDeinitionDetailMapper(DealDefinition deal);

        public abstract BaseDealLoggableEntity GetLoggableEntity();

        public Byte[] GetMaxTimestamp()
        {
            IDealDataManager _dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();
            return _dataManager.GetMaxTimestamp();
        }

        public DateTime? GetDealEvaluatorBeginDate(byte[] lastTimestamp, DateTime effectiveAfter)
        {
            IDealDataManager _dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();
            IEnumerable<DealDefinition> deals = _dataManager.GetDealsModifiedAfterTimestamp(lastTimestamp);
            if (deals == null || !deals.Any())
                return null;

            List<DealDefinition> effectiveDeals = deals.Where(item => item.Settings.RealEED != item.Settings.BeginDate && item.Settings.RealEED.VRGreaterThan(effectiveAfter)).ToList();
            if (effectiveDeals == null || effectiveDeals.Count == 0)
                return null;

            return effectiveDeals.MinBy(item => item.Settings.BeginDate).Settings.BeginDate;
        }

        #endregion

        #region Protected Methods
        protected Dictionary<Guid, List<DealDefinition>> GetCachedDealsByConfigId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDealsByConfig", () =>
            {
                var allDeal = this.GetCachedDeals();
                Dictionary<Guid, List<DealDefinition>> cachedByConfig = new Dictionary<Guid, List<DealDefinition>>();
                List<DealDefinition> list;
                foreach (var d in allDeal)
                {
                    if (!cachedByConfig.TryGetValue(d.Value.Settings.ConfigId, out list))
                    {
                        list = new List<DealDefinition>();
                        list.Add(d.Value);
                        cachedByConfig.Add(d.Value.Settings.ConfigId, list);
                    }
                    else
                    {
                        list.Add(d.Value);
                    }
                }
                return cachedByConfig;
            });
        }



        protected Dictionary<int, DealDefinition> GetCachedDeals()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDeals", () =>
            {
                var cachedDealsWithDeleted = GetCachedDealsWithDeleted();
                Dictionary<int, DealDefinition> cachedDeals = new Dictionary<int, DealDefinition>();

                if (cachedDealsWithDeleted != null && cachedDealsWithDeleted.Count > 0)
                {
                    foreach (var cachedDeal in cachedDealsWithDeleted)
                    {
                        int dealId = cachedDeal.Key;
                        DealDefinition dealDefinition = cachedDeal.Value;
                        if (!dealDefinition.IsDeleted)
                            cachedDeals.Add(dealId, dealDefinition);
                    }
                }
                return cachedDeals;
            });
        }

        protected Dictionary<int, DealDefinition> GetCachedDealsWithDeleted()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDealsWithDeleted", () =>
            {
                IDealDataManager dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();
                IEnumerable<DealDefinition> deals = dataManager.GetDeals();
                return deals.ToDictionary(deal => deal.DealId, deal => deal);
            });
        }

        #endregion

        #region Private Classes
        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDealDataManager _dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDealsUpdated(ref _updateHandle);
            }
        }

        public abstract class BaseDealLoggableEntity : VRLoggableEntityBase
        {
            public override string ModuleName
            {
                get { return "Deal"; }
            }
            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                DealDefinition dealDefinition = context.Object.CastWithValidate<DealDefinition>("context.Object");
                return dealDefinition.DealId;
            }
        }

        #endregion

        #region Private Methods


        #endregion
    }
}