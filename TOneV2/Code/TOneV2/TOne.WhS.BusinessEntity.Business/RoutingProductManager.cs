﻿using System;
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
            var allRoutingProducts = GetAllRoutingProducts();

            Func<RoutingProduct, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.SellingNumberPlanIds == null || input.Query.SellingNumberPlanIds.Contains(prod.SellingNumberPlanId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRoutingProducts.ToBigResult(input, filterExpression));
        }

        public IEnumerable<RoutingProductInfo> GetRoutingProductsInfoBySellingNumberPlan(int sellingNumberPlanId)
        {
            var routingProducts = GetAllRoutingProducts();
            return routingProducts.MapRecords(RoutingProductInfoMapper, x => x.SellingNumberPlanId == sellingNumberPlanId);
        }

        public IEnumerable<RoutingProductInfo> GetRoutingProductInfo(string filter)
        {
            var allRoutingProducts = GetAllRoutingProducts();
            return allRoutingProducts.MapRecords(RoutingProductInfoMapper);
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

        public RoutingProduct GetRoutingProduct(int routingProductId)
        {
            var allRoutingProducts = GetAllRoutingProducts();
            return allRoutingProducts.GetRecord(routingProductId);
        }

        public Dictionary<int, RoutingProduct> GetAllRoutingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRoutingProducts",
               () =>
               {
                   IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
                   IEnumerable<RoutingProduct> routingProducts = dataManager.GetRoutingProducts();
                   return routingProducts.ToDictionary(kvp => kvp.RoutingProductId, kvp => kvp);
               });
        }

        #region Private Methods
        
        private RoutingProductInfo RoutingProductInfoMapper(RoutingProduct routingProduct)
        {
            return new RoutingProductInfo()
            {
                RoutingProductId = routingProduct.RoutingProductId,
                Name = routingProduct.Name,
                SellingNumberPlanId = routingProduct.SellingNumberPlanId
            };
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRoutingProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreRoutingProductsUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
