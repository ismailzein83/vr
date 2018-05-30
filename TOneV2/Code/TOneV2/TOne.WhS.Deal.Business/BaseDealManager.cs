using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Data;
using Vanrise.Caching;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Deal.Business
{
    public abstract class BaseDealManager : BaseBusinessEntityManager
    {
        #region Public Methods
        public DealDefinition GetDeal(int dealId)
        {
            Dictionary<int, DealDefinition> cachedEntities = this.GetCachedDeals();
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
            ValidateBeforeSaveContext context = new ValidateBeforeSaveContext();
            context.IsEditMode = false;
            context.DealSaleZoneIds = deal.Settings.GetDealSaleZoneIds();
            context.DealSupplierZoneIds = deal.Settings.GetDealSupplierZoneIds();
            InsertDealOperationOutput insertOperationOutput = new InsertDealOperationOutput();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IDealDataManager dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();
            int insertedId = -1;
            if (deal.Settings.ValidateDataBeforeSave(context))
            {
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
        public bool IsZoneExcluded(long zoneId, DateTime BED, DateTime? EED, int carrierAccountId, int? dealId, bool isSale)
        {
            Dictionary<int, DealZoneInfoByZoneId> cachedDealInfo = new Dictionary<int, DealZoneInfoByZoneId>();
            SwapDealManager swapDealManager = new SwapDealManager();
            if (isSale)
                cachedDealInfo = GetCachedCustomerDealZoneInfoByCustomerId();
            else
                cachedDealInfo = GetCachedSupplierDealZoneInfoBySupplierId();
            if (cachedDealInfo != null)
            {
                DealZoneInfoByZoneId dealZoneInfo = cachedDealInfo.GetRecord(carrierAccountId);
                if (dealZoneInfo != null)
                {
                    var zoneInfos = dealZoneInfo.GetRecord(zoneId);
                    if (zoneInfos != null)
                    {
                        foreach (var zoneInfo in zoneInfos)
                        {
                            if ((dealId != zoneInfo.DealId || dealId == null) && IsOverlapped(BED, EED, zoneInfo.BED, zoneInfo.EED))
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        public List<long> AreZonesExcluded(OverlappedZonesContext context,bool isSale)
        {
            List<long> excludedSaleZones = new List<long>();
            if (context.ZoneIds != null)
                foreach (var saleZoneId in context.ZoneIds)
                {
                    if (IsZoneExcluded(saleZoneId, context.BED, context.EED, context.CarrierAccountId, context.DealId, isSale))
                        excludedSaleZones.Add(saleZoneId);
                }
            return excludedSaleZones;
        }
        private bool IsOverlapped(DateTime firstBeginEffectiveDate, DateTime? firstEndEffectiveDate, DateTime secondBeginEffectiveDate, DateTime? secondEndEffectiveDate)
        {
            return (secondEndEffectiveDate.VRGreaterThan(firstBeginEffectiveDate) && firstEndEffectiveDate > secondBeginEffectiveDate);
        }
        public Vanrise.Entities.UpdateOperationOutput<DealDefinitionDetail> UpdateDeal(DealDefinition deal)
        {
            var dealDefinition = GetDeal(deal.DealId);
            UpdateDealOperationOutput updateOperationOutput = new UpdateDealOperationOutput();
            ValidateBeforeSaveContext context = new ValidateBeforeSaveContext();
            context.IsEditMode = true;
            context.ExistingDeal = deal;
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
        public List<long> GetExcludedSaleZones(int? dealId, int carrierAccountId, List<long> saleZoneIds, DateTime beginDate, DateTime? endDate)
        {
            var overlappedSaleZonesContext = new OverlappedZonesContext
            {
                ZoneIds = saleZoneIds,
                BED = beginDate,
                EED = endDate,
                CarrierAccountId = carrierAccountId,
                DealId = dealId,
            };
           return AreZonesExcluded(overlappedSaleZonesContext,true);
        }
        public List<long> GetExcludedSupplierZones(int? dealId, int carrierAccountId, List<long> supplierZoneIds, DateTime beginDate, DateTime? endDate)
        {
            var overlappedSupplierZonesContext = new OverlappedZonesContext
            {
                ZoneIds = supplierZoneIds,
                BED = beginDate,
                EED = endDate,
                CarrierAccountId = carrierAccountId,
                DealId = dealId,
            };
            return AreZonesExcluded(overlappedSupplierZonesContext,false);
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

            List<DealDefinition> effectiveDeals = deals.Where(item => item.Settings.EndDate != item.Settings.BeginDate && item.Settings.EndDate.VRGreaterThan(effectiveAfter)).ToList();
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

        public Dictionary<int, DealZoneInfoByZoneId> GetCachedCustomerDealZoneInfoByCustomerId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustomerDealZoneInfoByZoneId", () =>
            {
                Dictionary<int, DealZoneInfoByZoneId> customerDealZoneInfo = new Dictionary<int, DealZoneInfoByZoneId>();
                var cachedDeals = GetCachedDeals();
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                foreach (var dealDef in cachedDeals.Values)
                {
                    var zoneIds = dealDef.Settings.GetDealSaleZoneIds();
                    if (zoneIds != null)
                    {
                        var carrierAccountId = dealDef.Settings.GetCarrierAccountId();
                        var carrierAccount = carrierAccountManager.GetCarrierAccount(carrierAccountId);

                        if (CarrierAccountManager.IsCustomer(carrierAccount.AccountType))
                        {
                            DealZoneInfoByZoneId dealZoneInfoByZoneId = customerDealZoneInfo.GetOrCreateItem(carrierAccountId);
                            foreach (var zoneId in zoneIds)
                            {
                                List<DealZoneInfo> dealZoneInfos = dealZoneInfoByZoneId.GetOrCreateItem(zoneId);
                                var overlappedItem = dealZoneInfos.FindRecord(x => IsOverlapped(x, dealDef.Settings.BeginDate, dealDef.Settings.EndDate));
                                if (overlappedItem == null)
                                {
                                    dealZoneInfos.Add(new DealZoneInfo
                                        {
                                            DealId = dealDef.DealId,
                                            ZoneId = zoneId,
                                            BED = dealDef.Settings.BeginDate,
                                            EED = dealDef.Settings.EndDate
                                        });
                                }
                            }
                        }
                    }
                }
                return customerDealZoneInfo;
            });

        }
        public Dictionary<int, DealZoneInfoByZoneId> GetCachedSupplierDealZoneInfoBySupplierId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSupplierDealZoneInfoByZoneId", () =>
            {
                Dictionary<int, DealZoneInfoByZoneId> supplierDealZoneInfo = new Dictionary<int, DealZoneInfoByZoneId>();
                var cachedDeals = GetCachedDealsByConfigId();
                IEnumerable<DealDefinition> dealDefinitions = cachedDeals.Values.SelectMany(item => item);
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                foreach (var deal in dealDefinitions)
                {
                    var zoneIds = deal.Settings.GetDealSupplierZoneIds();
                    if (zoneIds != null)
                    {
                        var carrierAccountId = deal.Settings.GetCarrierAccountId();
                        var carrierAccount = carrierAccountManager.GetCarrierAccount(carrierAccountId);

                        if (carrierAccount.AccountType == CarrierAccountType.Supplier || carrierAccount.AccountType == CarrierAccountType.Exchange)
                        {
                            DealZoneInfoByZoneId dealZoneInfoByZoneId;
                            if (!supplierDealZoneInfo.TryGetValue(carrierAccountId, out dealZoneInfoByZoneId))
                            {
                                dealZoneInfoByZoneId = new DealZoneInfoByZoneId();
                                supplierDealZoneInfo.Add(carrierAccountId, dealZoneInfoByZoneId);
                            }
                            foreach (var zoneId in zoneIds)
                            {
                                List<DealZoneInfo> dealZoneInfos = dealZoneInfoByZoneId.GetOrCreateItem(zoneId);
                                var overlappedItem = dealZoneInfos.FirstOrDefault(x => IsOverlapped(x, deal.Settings.BeginDate, deal.Settings.EndDate));
                               if (overlappedItem == null)
                               {
                                   dealZoneInfos.Add(new DealZoneInfo
                                   {
                                       DealId = deal.DealId,
                                       ZoneId = zoneId,
                                       BED = deal.Settings.BeginDate,
                                       EED = deal.Settings.EndDate
                                   });
                               }
                            }
                        }
                    }
                }
                return supplierDealZoneInfo;
            });
        }
        private bool IsOverlapped(DealZoneInfo dealZoneInfo, DateTime beginEffectiveDate, DateTime? endEffectiveDate)
        {
            return (endEffectiveDate.VRGreaterThan(dealZoneInfo.BED) && dealZoneInfo.EED > beginEffectiveDate);
        }

        protected Dictionary<int, DealDefinition> GetCachedDeals()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDeals", () =>
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