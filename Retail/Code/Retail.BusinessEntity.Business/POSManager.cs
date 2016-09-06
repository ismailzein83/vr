using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Entities.POS;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business
{
    public class POSManager
    {
        public InsertOperationOutput<PosDetail> AddPointOfSale(PointOfSale pos)
        {
            var insertOperationOutput = new InsertOperationOutput<PosDetail>
            {
                Result = InsertOperationResult.Failed,
                InsertedObject = null
            };

            long posId;
            IPOSDataManager dataManager = BEDataManagerFactory.GetDataManager<IPOSDataManager>();
            if (dataManager.Insert(pos, out posId))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                pos.Id = posId;
                insertOperationOutput.InsertedObject = PosDetailMapper(pos);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<PosDetail> UpdatePointOfSale(PointOfSale pos)
        {

            var updateOperationOutput = new UpdateOperationOutput<PosDetail>
            {
                Result = UpdateOperationResult.Failed,
                UpdatedObject = null
            };

            IPOSDataManager dataManager = BEDataManagerFactory.GetDataManager<IPOSDataManager>();

            if (dataManager.Update(pos))
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = PosDetailMapper(this.GetPos(pos.Id));
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<POSInfo> GetPointOfSalesInfo(string nameFilter)
        {
            string nameFilterLower = nameFilter != null ? nameFilter.ToLower() : null;
            IEnumerable<PointOfSale> pointOfSales = GetCachedPointOfSales().Values;

            Func<PointOfSale, bool> posFilter = (pos) =>
            {
                if (nameFilterLower != null && !pos.Name.ToLower().Contains(nameFilterLower))
                    return false;
                return true;
            };
            return pointOfSales.MapRecords(POSInfoMapper, posFilter).OrderBy(x => x.Name);
        }

        PointOfSale GetPos(long id)
        {
            var allPos = GetCachedPointOfSales();
            return allPos.GetRecord(id);
        }

        Dictionary<long, PointOfSale> GetCachedPointOfSales()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPointOfSales", () =>
            {
                IPOSDataManager dataManager = BEDataManagerFactory.GetDataManager<IPOSDataManager>();
                IEnumerable<PointOfSale> pointOfSales = dataManager.GetPointOfSales();
                return pointOfSales.ToDictionary(kvp => kvp.Id, kvp => kvp);
            });
        }

        #region Mappers
        PosDetail PosDetailMapper(PointOfSale pos)
        {
            return new PosDetail
            {
                Entity = pos
            };
        }

        POSInfo POSInfoMapper(PointOfSale pos)
        {
            return new POSInfo
            {
                PosId = pos.Id,
                Name = pos.Name
            };
        }

        #endregion

        #region Classes
        private class CacheManager : BaseCacheManager
        {
            IPOSDataManager _dataManager = BEDataManagerFactory.GetDataManager<IPOSDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArePOSsUpdated(ref _updateHandle);
            }

        }

        #endregion

    }
}
