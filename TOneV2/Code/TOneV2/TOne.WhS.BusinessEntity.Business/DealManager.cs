using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class DealManager
    {

        #region Fields

        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<DealDetail> GetFilteredDeals(Vanrise.Entities.DataRetrievalInput<DealQuery> input)
        {
            Dictionary<int, Deal> cachedEntities = this.GetCachedDeals();

            Func<Deal, bool> filterExpression = (itm) =>
                (input.Query.CarrierAccountIds == null || input.Query.CarrierAccountIds.Contains(itm.Settings.CarrierAccountId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedEntities.ToBigResult(input, filterExpression, DealDetailMapper));
        }

        public Deal GetDeal(int dealId)
        {
            Dictionary<int, Deal> cachedEntities = this.GetCachedDeals();
            return cachedEntities.GetRecord(dealId);
        }


        public Vanrise.Entities.InsertOperationOutput<DealDetail> AddDeal(Deal deal)
        {
           

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DealDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IDealDataManager dataManager = BEDataManagerFactory.GetDataManager<IDealDataManager>();
            int insertedId = -1;

            if (dataManager.Insert(deal, out insertedId))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                deal.DealId = insertedId;
                insertOperationOutput.InsertedObject = DealDetailMapper(deal);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<DealDetail> UpdateDeal(Deal deal)
        {
           

            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DealDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IDealDataManager dataManager = BEDataManagerFactory.GetDataManager<IDealDataManager>();

            if (dataManager.Update(deal))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DealDetailMapper(this.GetDeal(deal.DealId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDealDataManager _dataManager = BEDataManagerFactory.GetDataManager<IDealDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDealsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, Deal> GetCachedDeals()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDeals", () =>
            {
                IDealDataManager dataManager = BEDataManagerFactory.GetDataManager<IDealDataManager>();
                IEnumerable<Deal> switchConnectivities = dataManager.GetDeals();
                return switchConnectivities.ToDictionary(kvp => kvp.DealId, kvp => kvp);
            });
        }

        #endregion

        #region Mappers

        DealDetail DealDetailMapper(Deal deal)
        {
            return new DealDetail()
            {
                Entity = deal,
                CarrierAccountName = _carrierAccountManager.GetCarrierAccountName(deal.Settings.CarrierAccountId)
            };
        }
        #endregion

    }
}
