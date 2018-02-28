using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Data;
using Vanrise.Caching;

namespace TOne.WhS.Deal.Business
{
    public abstract class BaseDealManager
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
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DealDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IDealDataManager dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();
            int insertedId = -1;

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

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<DealDefinitionDetail> UpdateDeal(DealDefinition deal)
        {

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DealDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IDealDataManager dataManager = DealDataManagerFactory.GetDataManager<IDealDataManager>();
            if (deal.Settings.ValidateDataBeforeSave())
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
                updateOperationOutput.ShowExactMessage = true;
                updateOperationOutput.Message = "Grace Period should be less than difference between BED and EED";
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