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
    public class SellingProductManager
    {
        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<SellingProductDetail> GetFilteredSellingProducts(Vanrise.Entities.DataRetrievalInput<SellingProductQuery> input)
        {
            var allSellingProducts = GetCachedSellingProducts();

            Func<SellingProduct, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.SellingNumberPlanIds == null || input.Query.SellingNumberPlanIds.Contains(prod.SellingNumberPlanId))
                 ;

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSellingProducts.ToBigResult(input, filterExpression, SellingProductDetailMapper));
        }
        public IEnumerable<SellingProductInfo> GetSellingProductsInfo(SellingProductInfoFilter filter)
        {
            IEnumerable<SellingProduct> sellingProducts = null;

            if (filter != null && filter.AssignableToSellingProductId != null)
            {
                sellingProducts = this.GetAssignableSellingProducts((int)filter.AssignableToSellingProductId);
            }
            else
            {
                var cachedSellingProducts = GetCachedSellingProducts();
                if (cachedSellingProducts != null)
                    sellingProducts = cachedSellingProducts.Values;
            }

            return sellingProducts.MapRecords(SellingProductInfoMapper);
        }
        public IEnumerable<SellingProduct> GetAssignableSellingProducts(int carrierAccountId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccount carrierAccount = carrierAccountManager.GetCarrierAccount(carrierAccountId);
            var cachedSellingProducts = GetCachedSellingProducts();
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            var effectiveCustomerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(carrierAccountId, DateTime.Now, false);
            return cachedSellingProducts.Values.FindAllRecords(x => (x.SellingNumberPlanId == carrierAccount.SellingNumberPlanId && effectiveCustomerSellingProduct == null) || (x.SellingNumberPlanId == carrierAccount.SellingNumberPlanId && effectiveCustomerSellingProduct != null && x.SellingProductId != effectiveCustomerSellingProduct.SellingProductId));
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
        public string GetSellingProductName(int sellingProductId)
        {
            var sellingProduct = GetSellingProduct(sellingProductId);
            return sellingProduct != null ? sellingProduct.Name : null;
        }
        public int? GetSellingNumberPlanId(int sellingProductId)
        {
            var sellingProduct = GetSellingProduct(sellingProductId);
            if (sellingProduct == null)
                return null;
            else
                return sellingProduct.SellingNumberPlanId;
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
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                sellingProduct.SellingProductId = sellingProductId;
                insertOperationOutput.InsertedObject = SellingProductDetailMapper(sellingProduct);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
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
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Members
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISellingProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreSellingProductsUpdated(ref _updateHandle);
            }
        }
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

        #endregion

        #region  Mappers
        private SellingProductDetail SellingProductDetailMapper(SellingProduct sellingProduct)
        {
            SellingProductDetail sellingProductDetail = new SellingProductDetail();

            sellingProductDetail.Entity = sellingProduct;

            SellingNumberPlanManager sellingNumberPlanManager = new SellingNumberPlanManager();
            SellingNumberPlan sellingNumberPlan = sellingNumberPlanManager.GetSellingNumberPlan(sellingProduct.SellingNumberPlanId);

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
