using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class RoutingProductManager
    {
        public Vanrise.Entities.IDataRetrievalResult<RoutingProduct> GetFilteredRoutingProducts(Vanrise.Entities.DataRetrievalInput<RoutingProductQuery> input)
        {
            var allRoutingProducts = GetCachedRoutingProducts();

            Func<IEnumerable<RoutingProduct>, IEnumerable<RoutingProduct>> filterResult = (listToFilter) =>
            {
                return listToFilter.Where(itm =>
                 (input.Query.Name == null || itm.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.SaleZonePackageIds == null || input.Query.SaleZonePackageIds.Contains(itm.SaleZonePackageId)));
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRoutingProducts.ToBigResult(input, filterResult));
        }

        public RoutingProduct GetRoutingProduct(int routingProductId)
        {
            List<RoutingProduct> routingProducts = GetCachedRoutingProducts();
            return routingProducts.Find(x => x.RoutingProductId == routingProductId);
        }

        public List<RoutingProductInfo> GetRoutingProductsInfoBySaleZonePackage(int saleZonePackageId)
        {
            List<RoutingProduct> routingProducts = GetCachedRoutingProducts();
            return routingProducts.Where(x => x.SaleZonePackageId == saleZonePackageId).Select(RoutingProductInfoMapper).ToList();
        }
        
        public List<RoutingProductInfo> GetRoutingProducts()
        {
            List<RoutingProduct> routingProducts = GetCachedRoutingProducts();
            return routingProducts.Select(RoutingProductInfoMapper).ToList();
        }

        public TOne.Entities.InsertOperationOutput<RoutingProduct> AddRoutingProduct(RoutingProduct routingProduct)
        {
            TOne.Entities.InsertOperationOutput<RoutingProduct> insertOperationOutput = new TOne.Entities.InsertOperationOutput<RoutingProduct>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int routingProductId = -1;

            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            bool insertActionSucc = dataManager.Insert(routingProduct, out routingProductId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                routingProduct.RoutingProductId = routingProductId;
                insertOperationOutput.InsertedObject = routingProduct;
            }

            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<RoutingProduct> UpdateRoutingProduct(RoutingProduct routingProduct)
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();

            bool updateActionSucc = dataManager.Update(routingProduct);
            TOne.Entities.UpdateOperationOutput<RoutingProduct> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<RoutingProduct>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = routingProduct;
            }

            return updateOperationOutput;
        }

        public TOne.Entities.DeleteOperationOutput<object> DeleteRoutingProduct(int routingProductId)
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();

            TOne.Entities.DeleteOperationOutput<object> deleteOperationOutput = new TOne.Entities.DeleteOperationOutput<object>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;

            bool deleteActionSucc = dataManager.Delete(routingProductId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }

        #region Private Members

        List<RoutingProduct> GetCachedRoutingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRoutingProducts",
               () =>
               {
                   IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
                   return dataManager.GetRoutingProducts();
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRoutingProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreRoutingProductsUpdated(ref _updateHandle);
            }
        }

        private RoutingProductInfo RoutingProductInfoMapper(RoutingProduct routingProduct)
        {
            return new RoutingProductInfo()
            {
                RoutingProductId = routingProduct.RoutingProductId,
                Name = routingProduct.Name,
                SaleZonePackageId = routingProduct.SaleZonePackageId
            };
        }

        #endregion
    }
}
