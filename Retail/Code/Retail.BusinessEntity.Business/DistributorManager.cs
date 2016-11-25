using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class DistributorManager
    {
        public IDataRetrievalResult<DistributorDetail> GetFilteredDistributors(DataRetrievalInput<DistributorQuery> input)
        {
            Dictionary<long, Distributor> cachedDistributors = this.GetCachedDistributors();

            Func<Distributor, bool> filterExpression = (distributor) =>
                (input.Query.Name == null || distributor.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, cachedDistributors.ToBigResult(input, filterExpression, DistributorDetailMapper));
        }

        public InsertOperationOutput<DistributorDetail> AddDistributor(Distributor distributor)
        {
            var insertOperationOutput = new InsertOperationOutput<DistributorDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };

            long distributorId;

            if (TryAddDistributor(distributor, out distributorId))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                distributor.Id = distributorId;
                insertOperationOutput.InsertedObject = DistributorDetailMapper(distributor);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<DistributorDetail> UpdateDistributor(Distributor distributor)
        {
            var updateOperationOutput = new UpdateOperationOutput<DistributorDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };

            if (TryUpdateDistributor(distributor))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DistributorDetailMapper(this.GetDistributor(distributor.Id));
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public IEnumerable<DistributorInfo> GetDistributorsInfo(string nameFilter)
        {
            string nameFilterLower = nameFilter != null ? nameFilter.ToLower() : null;
            IEnumerable<Distributor> distributors = GetCachedDistributors().Values;

            Func<Distributor, bool> distributorFilter = (distributor) =>
            {
                if (nameFilterLower != null && !distributor.Name.ToLower().Contains(nameFilterLower))
                    return false;
                return true;
            };
            return distributors.MapRecords(DistributorInfoMapper, distributorFilter).OrderBy(x => x.Name);
        }

        Distributor GetDistributor(long distributorId)
        {
            var allDistributors = GetCachedDistributors();
            return allDistributors.GetRecord(distributorId);
        }

        Dictionary<long, Distributor> GetCachedDistributors()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDistributors", () =>
            {
                IDistributorDataManager dataManager = BEDataManagerFactory.GetDataManager<IDistributorDataManager>();
                IEnumerable<Distributor> distributors = dataManager.GetDistributors();
                return distributors.ToDictionary(kvp => kvp.Id, kvp => kvp);
            });
        }

        Dictionary<string, Distributor> GetCachedDistributorsBySourceId()
        {
            return GetCachedDistributors().ToDictionary(x => x.Value.SourceId, x => x.Value);
        }


        #region Mappers

        DistributorInfo DistributorInfoMapper(Distributor distributor)
        {
            return new DistributorInfo
            {
                DistributorId = distributor.Id,
                Name = distributor.Name
            };
        }
        DistributorDetail DistributorDetailMapper(Distributor distributor)
        {
            return new DistributorDetail
            {
                Entity = distributor
            };
        }

        #endregion

        #region Classes

        private class CacheManager : BaseCacheManager
        {
            IDistributorDataManager _dataManager = BEDataManagerFactory.GetDataManager<IDistributorDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDistributorsUpdated(ref _updateHandle);
            }


        }

        #endregion


        internal bool TryAddDistributor(Distributor distributor, out long distributorId)
        {
            distributorId = 0;
            if (distributor == null)
                throw new ArgumentNullException("distributor");
            IDistributorDataManager dataManager = BEDataManagerFactory.GetDataManager<IDistributorDataManager>();
            return dataManager.Insert(distributor, out distributorId);
        }

        public Distributor GetDistributorBySourceId(string sourceId)
        {
            Dictionary<string, Distributor> cachedDistributors = this.GetCachedDistributorsBySourceId();
            return cachedDistributors.GetRecord(sourceId);
        }

        internal bool TryUpdateDistributor(Distributor distributor)
        {
            IDistributorDataManager dataManager = BEDataManagerFactory.GetDataManager<IDistributorDataManager>();
            return dataManager.Update(distributor);
        }
    }
}
