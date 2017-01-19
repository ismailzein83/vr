using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

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
            Func<SellingProduct, bool> filterPredicate = null;

            if(filter != null)
            {
                CarrierAccount assignableToCustomer = null;
                CustomerSellingProduct effectiveCustomerSellingProduct = null;
                if (filter.AssignableToCustomerId.HasValue)
                {
                    assignableToCustomer = LoadCustomer(filter.AssignableToCustomerId.Value);
                    effectiveCustomerSellingProduct = LoadEffectiveCustomerSellingProduct(filter.AssignableToCustomerId.Value);
                }

                filterPredicate = (prod) =>
                {
                    if (filter.SellingNumberPlanId.HasValue && prod.SellingNumberPlanId != filter.SellingNumberPlanId.Value)
                        return false;

                    if (filter.AssignableToCustomerId.HasValue && !IsAssignableToCustomer(prod, filter.AssignableToCustomerId.Value, assignableToCustomer, effectiveCustomerSellingProduct))
                        return false;

                    return true;
                };
            }

            return GetCachedSellingProducts().MapRecords(SellingProductInfoMapper, filterPredicate).OrderBy(x => x.Name);
        }
        public IEnumerable<SellingProduct> GetSellingProductsBySellingNumberPlan(int sellingNumberPlanId)
        {
            IEnumerable<SellingProduct> sellingProducts = GetCachedSellingProducts().Values;
            return sellingProducts.FindAllRecords(item => item.SellingNumberPlanId == sellingNumberPlanId);
        }
        public IEnumerable<SellingProductInfo> GetAllSellingProduct()
        {
            var sellingProducts = GetCachedSellingProducts();
            return sellingProducts.MapRecords(SellingProductInfoMapper).OrderBy(x => x.Name);
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
            ValidateSellingProductToAdd(sellingProduct);

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
        public TOne.Entities.UpdateOperationOutput<SellingProductDetail> UpdateSellingProduct(SellingProductToEdit sellingProduct)
        {
            ValidateSellingProductToEdit(sellingProduct);

            ISellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();

            bool updateActionSucc = dataManager.Update(sellingProduct);
            TOne.Entities.UpdateOperationOutput<SellingProductDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<SellingProductDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SellingProductDetailMapper(this.GetSellingProduct(sellingProduct.SellingProductId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Validation Methods

        void ValidateSellingProductToAdd(SellingProduct sellingProduct)
        {
            var sellingNumberPlanManager = new SellingNumberPlanManager();
            var sellingNumberPlan = sellingNumberPlanManager.GetSellingNumberPlan(sellingProduct.SellingNumberPlanId);
            if (sellingNumberPlan == null)
                throw new DataIntegrityValidationException(String.Format("SellingNumberPlan '{0}' does not exist", sellingProduct.SellingNumberPlanId));

            ValidateSellingProduct(sellingProduct.Name);
        }

        void ValidateSellingProductToEdit(SellingProductToEdit sellingProduct)
        {
            ValidateSellingProduct(sellingProduct.Name);
        }

        void ValidateSellingProduct(string spName)
        {
            if (String.IsNullOrWhiteSpace(spName))
                throw new MissingArgumentValidationException("SellingProduct.Name");
        }
        
        #endregion

        #region Private Methods

        private bool IsAssignableToCustomer(SellingProduct sellingProduct, int customerId, CarrierAccount customer, CustomerSellingProduct effectiveCustomerSellingProduct)
        {
            if (sellingProduct.SellingNumberPlanId != customer.SellingNumberPlanId.Value)
                return false;
            if (effectiveCustomerSellingProduct != null && effectiveCustomerSellingProduct.SellingProductId == sellingProduct.SellingProductId)
                return false;
            return true;
        }

        private CarrierAccount LoadCustomer(int customerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccount customer = carrierAccountManager.GetCarrierAccount(customerId);
            if (customer == null)
                throw new NullReferenceException(String.Format("CarrierAccount '{0}'", customerId));
            if (!customer.SellingNumberPlanId.HasValue)
                throw new Exception(String.Format("Customer Account '{0}' doesnt have SellingNumberPlanId", customerId));

            return customer;
        }

        private CustomerSellingProduct LoadEffectiveCustomerSellingProduct(int customerId)
        {
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            return customerSellingProductManager.GetEffectiveSellingProduct(customerId, DateTime.Now, false);
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
				SellingNumberPlanId = sellingProduct.SellingNumberPlanId
            };
        }
        #endregion
    }
}
