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
    public class SellingProductManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SellingProductDetail> GetFilteredSellingProducts(Vanrise.Entities.DataRetrievalInput<SellingProductQuery> input)
        {
            var allSellingProducts = GetCachedSellingProducts();

            Func<SellingProduct, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.SellingNumberPlanIds == null || input.Query.SellingNumberPlanIds.Contains(prod.SellingNumberPlanId))
                  &&
                 (input.Query.RoutingProductsIds == null || input.Query.RoutingProductsIds.Contains(prod.DefaultRoutingProductId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSellingProducts.ToBigResult(input, filterExpression, SellingProductDetailMapper));
        }
        public IEnumerable<SellingProductInfo> GetAllSellingProduct()
        {
            var sellingProducts = GetCachedSellingProducts();
            return sellingProducts.MapRecords(SellingProductInfoMapper);
        }
        public SellingProduct GetSellingProduct(int sellingProductId)
        {
            var sellingProducts = GetCachedSellingProducts();
            return sellingProducts.GetRecord(sellingProductId);
        }


        public TOne.Entities.InsertOperationOutput<SellingProductDetail> AddSellingProduct(SellingProduct sellingProduct)
        {
            TOne.Entities.InsertOperationOutput<SellingProductDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<SellingProductDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int sellingProductId = -1;

            ISellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();
            bool insertActionSucc = dataManager.Insert(sellingProduct, out sellingProductId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                sellingProduct.SellingProductId = sellingProductId;
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                sellingProduct.SellingProductId = sellingProductId;
                insertOperationOutput.InsertedObject = SellingProductDetailMapper(sellingProduct);
            }

            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<SellingProductDetail> UpdateSellingProduct(SellingProduct sellingProduct)
        {
            ISellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();

            bool updateActionSucc = dataManager.Update(sellingProduct);
            TOne.Entities.UpdateOperationOutput<SellingProductDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<SellingProductDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SellingProductDetailMapper(sellingProduct);
            }

            return updateOperationOutput;
        }

        public TOne.Entities.DeleteOperationOutput<object> DeleteSellingProduct(int sellingProductId)
        {
            ISellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();

            TOne.Entities.DeleteOperationOutput<object> deleteOperationOutput = new TOne.Entities.DeleteOperationOutput<object>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;

            bool deleteActionSucc = dataManager.Delete(sellingProductId);

            if (deleteActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }


        #region Private Members
        Dictionary<int, SellingProduct> GetCachedSellingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSellingProducts",
               () =>
               {
                   ISellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();
                   IEnumerable<SellingProduct> sellingProducts = dataManager.GetSellingProducts();
                   return sellingProducts.ToDictionary(kvp => kvp.SellingProductId, kvp => kvp);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISellingProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSellingProductsUpdated(ref _updateHandle);
            }
        }
        private SellingProductDetail SellingProductDetailMapper(SellingProduct sellingProduct)
        {
            SellingProductDetail sellingProductDetail = new SellingProductDetail();

            sellingProductDetail.Entity = sellingProduct;

            SellingNumberPlanManager sellingNumberPlanManager = new SellingNumberPlanManager();
            RoutingProductManager routingProductManager = new RoutingProductManager();
            SellingNumberPlan sellingNumberPlan = sellingNumberPlanManager.GetSellingNumberPlan(sellingProduct.SellingNumberPlanId);

            if (sellingProduct.DefaultRoutingProductId.HasValue)
            {
                RoutingProduct routingProduct = routingProductManager.GetRoutingProduct(sellingProduct.DefaultRoutingProductId.Value);
                if (routingProduct != null)
                    sellingProductDetail.DefaultRoutingProductName = routingProduct.Name;
            }
            if (sellingNumberPlan != null)
            {
                sellingProductDetail.SellingNumberPlanName = sellingNumberPlan.Name;
            }
            return sellingProductDetail;
        }

        private SellingProductInfo SellingProductInfoMapper(SellingProduct sellingProduct)
        {
            return new SellingProductInfo()
            {
                SellingProductId = sellingProduct.SellingProductId,
                Name = sellingProduct.Name,
            };
        }

        #endregion
    }
}
