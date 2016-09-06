using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class PopManager
    {

        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<Pop> GetFilteredPops(Vanrise.Entities.DataRetrievalInput<PopQuery> input)
        {
            var allPops = GetCachedPops();

            Func<Pop, bool> filterExpression = (item) =>
                 (input.Query.Name == null || item.Name.ToLower().Contains(input.Query.Name.ToLower()));


            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allPops.ToBigResult(input, filterExpression ));
        }

       
        public Pop GetPop(int popId)
        {
            var pops = GetCachedPops();
            return pops.GetRecord(popId);
        }

        public Vanrise.Entities.InsertOperationOutput<Pop> AddPop(Pop pop)
        {
            Vanrise.Entities.InsertOperationOutput<Pop> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<Pop>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int popId = -1;

            IPopDataManager dataManager = BEDataManagerFactory.GetDataManager<IPopDataManager>();
            bool insertActionSucc = dataManager.Insert(pop, out popId);
            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                pop.PopId = popId;
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = pop;
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;


            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<Pop> UpdatePop(Pop pop)
        {
            IPopDataManager dataManager = BEDataManagerFactory.GetDataManager<IPopDataManager>();

            bool updateActionSucc = dataManager.Update(pop);
            Vanrise.Entities.UpdateOperationOutput<Pop> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<Pop>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = pop;
            }
            else
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            return updateOperationOutput;
        }

        #endregion

        #region Private Methods
        Dictionary<int, Pop> GetCachedPops()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPops",
               () =>
               {
                   IPopDataManager dataManager = BEDataManagerFactory.GetDataManager<IPopDataManager>();
                   IEnumerable<Pop> pops = dataManager.GetAllPops();
                   return pops.ToDictionary(p => p.PopId, p => p);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IPopDataManager _dataManager = BEDataManagerFactory.GetDataManager<IPopDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArePopsUpdated(ref _updateHandle);
            }
        }
      

        #endregion

        #region  Mappers

        #endregion

    }
}
