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
    public class DealProgressManager
    {

        #region Fields

        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<DealProgressDetail> GetFilteredDealsProgress(Vanrise.Entities.DataRetrievalInput<DealProgressQuery> input)
        {
            Dictionary<int, DealProgress> cachedEntities = this.GetCachedDealsProgress();
            Func<DealProgress, bool> filterExpression = (prod) =>
            {
                if ( prod.ProgressDate < input.Query.FromDate)
                    return false;
                if (input.Query.ToDate != null && prod.ProgressDate > input.Query.ToDate)
                    return false;

                if (input.Query.IsSelling != prod.IsSelling)
                    return false;
              
                return true;
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, cachedEntities.ToBigResult(input, filterExpression, DealProgressDetailMapper));
        }


        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDealProgressDataManager _dataManager = BEDataManagerFactory.GetDataManager<IDealProgressDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDealsProgressUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<int, DealProgress> GetCachedDealsProgress()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDealsProgress", () =>
            {
                IDealProgressDataManager dataManager = BEDataManagerFactory.GetDataManager<IDealProgressDataManager>();
                IEnumerable<DealProgress> dealsProgress = dataManager.GetDealsProgress();
                return dealsProgress.ToDictionary(dp => dp.DealProgressId, dp => dp);
            });
        }

        #endregion

        #region Mappers

        DealProgressDetail DealProgressDetailMapper(DealProgress dealProgress)
        {
            return new DealProgressDetail()
            {
                Entity = dealProgress
            };
        }
        #endregion

    }
}
