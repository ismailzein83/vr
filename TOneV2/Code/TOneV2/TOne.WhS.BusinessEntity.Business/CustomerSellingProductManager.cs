﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class CustomerSellingProductManager
    {
        #region Fields

        SellingProductManager _sellingProductManager = new SellingProductManager();
        CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<CustomerSellingProductDetail> GetFilteredCustomerSellingProducts(Vanrise.Entities.DataRetrievalInput<CustomerSellingProductQuery> input)
        {

            var allCustomerSellingProducts = GetEffectiveSellingProducts(input.Query.EffectiveDate);

            Func<CustomerSellingProduct, bool> filterExpression = (prod) =>
              {
                  if (input.Query.CustomersIds != null && !input.Query.CustomersIds.Contains(prod.CustomerId))
                      return false;

                  if (input.Query.SellingProductsIds != null && !input.Query.SellingProductsIds.Contains(prod.SellingProductId))
                      return false;

                  return ShouldSelectCustomerSellingProduct(prod.CustomerId);
              };

            var resultProcessingHandler = new ResultProcessingHandler<CustomerSellingProductDetail>()
            {
                ExportExcelHandler = new CustomerSellingProductDetailExportExcelHandler()
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allCustomerSellingProducts.ToBigResult(input, filterExpression, CustomerSellingProductDetailMapper), resultProcessingHandler);
        }

        private bool ShouldSelectCustomerSellingProduct(int customerId)
        {
            return _carrierAccountManager.IsCarrierAccountActive(customerId);
        }

        public IEnumerable<int> GetCustomerIdsAssignedToSellingProduct(int sellingProductId, DateTime effectiveOn)
        {
            return new CarrierAccountManager().GetCarrierAccountIdsAssignedToSellingProduct(sellingProductId);
        }

        public IEnumerable<CarrierAccountInfo> GetCustomersBySellingProductId(int sellingProductId, DateTime effectiveOn)
        {
            return new CarrierAccountManager().GetCarrierAccountsAssignedToSellingProduct(sellingProductId);
        }

        public IEnumerable<CarrierAccountInfo> GetOrderedCustomersBySellingProductId(int sellingProductId)
        {
            IEnumerable<CarrierAccountInfo> customers = GetCustomersBySellingProductId(sellingProductId, DateTime.Today);
            return (customers != null) ? customers.OrderBy(x => x.Name) : null;
        }

        private Dictionary<int, CustomerSellingProduct> GetEffectiveSellingProducts(DateTime? effectiveOn)
        {
            var allCustomerSellingProducts = GetCachedCustomerSellingProducts();


            if (effectiveOn == null)
                return allCustomerSellingProducts;

            Dictionary<int, List<CustomerSellingProduct>> customerSellingProductsByCustomerId = new Dictionary<int, List<CustomerSellingProduct>>();

            foreach (CustomerSellingProduct item in allCustomerSellingProducts.Values)
            {
                List<CustomerSellingProduct> customerSellingProducts = null;
                customerSellingProductsByCustomerId.TryGetValue(item.CustomerId, out customerSellingProducts);
                if (customerSellingProducts == null)
                {
                    customerSellingProducts = new List<CustomerSellingProduct>();
                    customerSellingProductsByCustomerId.Add(item.CustomerId, customerSellingProducts);
                }

                customerSellingProducts.Add(item);
            }

            Dictionary<int, CustomerSellingProduct> filteredCustomerSellingProducts = new Dictionary<int, CustomerSellingProduct>();

            foreach (KeyValuePair<int, List<CustomerSellingProduct>> kvp in customerSellingProductsByCustomerId)
            {
                CustomerSellingProduct effectiveCustomerSellingProduct = kvp.Value.OrderByDescending(x => x.BED).FirstOrDefault(x => effectiveOn.Value >= x.BED);
                if (effectiveCustomerSellingProduct != null)
                    filteredCustomerSellingProducts.Add(effectiveCustomerSellingProduct.CustomerSellingProductId, effectiveCustomerSellingProduct);
            }

            return filteredCustomerSellingProducts;
        }

        public CustomerSellingProduct GetCustomerSellingProduct(int customerSellingProductId)
        {
            var customerSellingProducts = GetCachedCustomerSellingProducts();
            return customerSellingProducts.GetRecord(customerSellingProductId);
        }

        public CustomerSellingProduct GetEffectiveSellingProduct(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            var cacheName = new EffectiveSellingProductCacheName { CustomerId = customerId, EffectiveOn = effectiveOn.HasValue ? effectiveOn.Value.Date : default(DateTime), IsEffectiveInFuture = isEffectiveInFuture };// String.Concat("GetEffectiveSellingProduct_", customerId, effectiveOn.HasValue ? effectiveOn.Value.Date : default(DateTime), isEffectiveInFuture);

            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, () =>
            {
                int? sellingProductId = new CarrierAccountManager().GetSellingProductId(customerId);

                if (!sellingProductId.HasValue)
                    return null;
                else
                {
                    return new CustomerSellingProduct()
                    {
                        CustomerSellingProductId = -1,
                        CustomerId = customerId,
                        SellingProductId = sellingProductId.Value,
                        BED = DateTime.MinValue
                    };
                }
            });
        }

        public InsertOperationOutput<List<CustomerSellingProductDetail>> AddCustomerSellingProduct(List<CustomerSellingProduct> customerSellingProducts)
        {
            foreach (CustomerSellingProduct customerSellingProduct in customerSellingProducts)
                ValidateCustomerSellingProductToAdd(customerSellingProduct);

            InsertOperationOutput<List<CustomerSellingProductDetail>> insertOperationOutput = new InsertOperationOutput<List<CustomerSellingProductDetail>>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            foreach (var obj in customerSellingProducts)
            {
                var effectiveCustomerSellingProduct = GetEffectiveSellingProduct(obj.CustomerId, DateTime.Now, false);

                if (effectiveCustomerSellingProduct != null && (obj.BED <= effectiveCustomerSellingProduct.BED || (obj.SellingProductId == effectiveCustomerSellingProduct.SellingProductId)))
                {
                    return insertOperationOutput;
                }
            }

            ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();
            List<CustomerSellingProduct> customerSellingProductObject = new List<CustomerSellingProduct>();

            foreach (CustomerSellingProduct customerSellingProduct in customerSellingProducts)
            {
                customerSellingProductObject.Add(new CustomerSellingProduct
                {
                    CustomerId = customerSellingProduct.CustomerId,
                    BED = customerSellingProduct.BED,
                    SellingProductId = customerSellingProduct.SellingProductId,
                });
            }
            List<CustomerSellingProduct> insertedObjects = new List<CustomerSellingProduct>();

            List<CustomerSellingProductDetail> returnedData = new List<CustomerSellingProductDetail>();
            bool insertActionSucc = dataManager.Insert(customerSellingProductObject, out  insertedObjects);
            if (insertActionSucc)
            {
                CarrierAccount carrierAccount;
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                foreach (CustomerSellingProduct customerSellingProduct in insertedObjects)
                {
                    var obj = CustomerSellingProductDetailMapper(customerSellingProduct);
                    returnedData.Add(new CustomerSellingProductDetail
                    {
                        Entity = obj.Entity,
                        CustomerName = obj.CustomerName,
                        SellingProductName = obj.SellingProductName
                    });
                    carrierAccount = _carrierAccountManager.GetCarrierAccount(customerSellingProduct.CustomerId);
                    VRActionLogger.Current.LogObjectCustomAction(TOne.WhS.BusinessEntity.Business.CarrierAccountManager.CarrierAccountLoggableEntity.Instance, "Assign Customer", true, carrierAccount, null);
                }
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = returnedData;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<CustomerSellingProductDetail> UpdateCustomerSellingProduct(CustomerSellingProductToEdit customerSellingProduct)
        {
            int customerId;
            ValidateCustomerSellingProductToEdit(customerSellingProduct, out customerId);

            ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();
            UpdateOperationOutput<CustomerSellingProductDetail> updateOperationOutput = new UpdateOperationOutput<CustomerSellingProductDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            var effectiveCustomerSellingProduct = GetEffectiveSellingProduct(customerId, DateTime.Now, false);

            if (customerSellingProduct.BED < DateTime.Now || (effectiveCustomerSellingProduct != null && customerSellingProduct.BED < effectiveCustomerSellingProduct.BED))
            {
                return updateOperationOutput;
            }
            bool updateActionSucc = dataManager.Update(customerSellingProduct);

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                CustomerSellingProductDetail customerSellingProductDetail = CustomerSellingProductDetailMapper(this.GetCustomerSellingProduct(customerSellingProduct.CustomerSellingProductId));
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = customerSellingProductDetail;
            }

            return updateOperationOutput;
        }

        public IEnumerable<CustomerSellingProduct> GetEffectiveInFutureCustomerSellingProduct()
        {
            var customerSellingProducts = GetCachedCustomerSellingProducts();
            return customerSellingProducts.Values.FindAllRecords(x => x.BED > DateTime.Now);
        }

        public bool IsCustomerAssignedToSellingProduct(int customerId)
        {
            int? sellingProductId = new CarrierAccountManager().GetSellingProductId(customerId);
            return sellingProductId.HasValue;
        }

        public int? GetEffectiveSellingProductId(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            return new CarrierAccountManager().GetSellingProductId(customerId);
        }

        public IEnumerable<ProcessedCustomerSellingProduct> GetProcessedCustomerSellingProducts(int customerId)
        {
            Dictionary<int, List<ProcessedCustomerSellingProduct>> entitiesByCustomerId = GetCachedProcessedCustomerSellingProductsByCustomerId();
            return entitiesByCustomerId.GetRecord(customerId);
        }

        public IEnumerable<int> GetEffectiveOrFutureAssignedCustomerIds(int sellingProductId, DateTime minimumDate)
        {
            return new CarrierAccountManager().GetCarrierAccountIdsAssignedToSellingProduct(sellingProductId);
        }

        #endregion

        #region Validation Methods

        void ValidateCustomerSellingProductToAdd(CustomerSellingProduct customerSellingProduct)
        {
            ValidateCustomerSellingProduct(customerSellingProduct.CustomerId, customerSellingProduct.SellingProductId);
        }

        void ValidateCustomerSellingProductToEdit(CustomerSellingProductToEdit customerSellingProduct, out int customerId)
        {
            CustomerSellingProduct customerSellingProductEntity = this.GetCustomerSellingProduct(customerSellingProduct.CustomerSellingProductId);
            if (customerSellingProductEntity == null)
                throw new DataIntegrityValidationException(String.Format("CustomerSellingProduct '{0}' does not exist", customerSellingProduct.CustomerSellingProductId));

            customerId = customerSellingProductEntity.CustomerId;
            ValidateCustomerSellingProduct(customerSellingProductEntity.CustomerId, customerSellingProduct.SellingProductId);
        }

        void ValidateCustomerSellingProduct(int customerId, int sellingProductId)
        {
            var sellingProduct = _sellingProductManager.GetSellingProduct(sellingProductId);
            if (sellingProduct == null)
                throw new DataIntegrityValidationException(String.Format("SellingProduct '{0}' does not exist", sellingProductId));

            var customer = _carrierAccountManager.GetCarrierAccount(customerId);
            if (customer == null)
                throw new DataIntegrityValidationException(String.Format("Customer '{0}' does not exist", customerId));
            else if (!CarrierAccountManager.IsCustomer(customer.AccountType))
                throw new DataIntegrityValidationException(String.Format("CarrierAccount '{0}' is a {1} and not a customer", customerId, customer.AccountType.ToString().ToLower()));
            else if (!customer.SellingNumberPlanId.HasValue)
                throw new DataIntegrityValidationException(String.Format("Customer '{0}' is not associated with a SellingNumberPlan", customerId));

            if (sellingProduct.SellingNumberPlanId != customer.SellingNumberPlanId.Value)
                throw new DataIntegrityValidationException(String.Format("Customer '{0}' and SellingProduct '{1}' belong to different SellingNumberPlans", customerId, sellingProductId));
        }

        #endregion

        #region Private Members

        private Dictionary<int, CustomerSellingProduct> GetCachedCustomerSellingProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedOrderedCustomerSellingProducts", () =>
            {
                ICustomerSellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ICustomerSellingProductDataManager>();
                IEnumerable<CustomerSellingProduct> customerSellingProducts = dataManager.GetCustomerSellingProducts();

                var customerSellingProductsById = new Dictionary<int, CustomerSellingProduct>();

                if (customerSellingProducts != null)
                {
                    var carrierAccountManager = new CarrierAccountManager();

                    foreach (CustomerSellingProduct customerSellingProduct in customerSellingProducts.OrderBy(x => x.BED))
                    {
                        if (!carrierAccountManager.IsCarrierAccountDeleted(customerSellingProduct.CustomerId))
                            customerSellingProductsById.Add(customerSellingProduct.CustomerSellingProductId, customerSellingProduct);
                    }
                }

                return customerSellingProductsById;
            });
        }

        private Dictionary<int, List<ProcessedCustomerSellingProduct>> GetCachedProcessedCustomerSellingProductsByCustomerId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedProcessedCustomerSellingProductsByCustomerId", () =>
            {
                var entitiesByCustomerId = new Dictionary<int, List<ProcessedCustomerSellingProduct>>();

                var carrierAccountManager = new CarrierAccountManager();
                IEnumerable<CarrierAccount> allCustomers = carrierAccountManager.GetAllCustomers();

                if (allCustomers != null && allCustomers.Count() > 0)
                {
                    foreach (CarrierAccount carrierAccount in allCustomers)
                    {
                        if (carrierAccountManager.IsCarrierAccountActive(carrierAccount) && carrierAccount.SellingProductId.HasValue)
                        {
                            entitiesByCustomerId.Add(carrierAccount.CarrierAccountId, new List<ProcessedCustomerSellingProduct>()
                            {
                                new ProcessedCustomerSellingProduct()
                                {
                                    CustomerId = carrierAccount.CarrierAccountId,
                                    SellingProductId = carrierAccount.SellingProductId.Value,
                                    BED = DateTime.MinValue,
                                    EED = null
                                }
                            });
                        }
                    }
                }

                return entitiesByCustomerId;
            });
        }

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _carrierAccountLastCheck;
            protected override bool UseCentralizedCacheRefresher
            {
                get
                {
                    return true;
                }
            }
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return base.ShouldSetCacheExpired()
                  | Vanrise.Caching.CacheManagerFactory.GetCacheManager<CarrierAccountManager.CacheManager>().IsCacheExpired(ref _carrierAccountLastCheck);
            }
        }

        private class CustomerSellingProductDetailExportExcelHandler : ExcelExportHandler<CustomerSellingProductDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<CustomerSellingProductDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Selling Products",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Customer", Width = 60 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Selling Product", Width = 60 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Effective On", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    sheet.Rows = new List<ExportExcelRow>();
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.CustomerName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.SellingProductName });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.BED });
                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }

        private struct EffectiveSellingProductCacheName
        {
            public int CustomerId { get; set; }

            public DateTime EffectiveOn { get; set; }

            public bool IsEffectiveInFuture { get; set; }
        }

        #endregion

        #region  Mappers

        private CarrierAccountInfo CarrierAccountInfoMapper(CustomerSellingProduct customerSellingProduct)
        {
            return new CarrierAccountInfo()
            {
                AccountType = CarrierAccountType.Customer,
                CarrierAccountId = customerSellingProduct.CustomerId,
                Name = _carrierAccountManager.GetCarrierAccountName(customerSellingProduct.CustomerId)
            };
        }

        private CustomerSellingProductDetail CustomerSellingProductDetailMapper(CustomerSellingProduct customerSellingProduct)
        {
            CustomerSellingProductDetail customerSellingProductDetail = new CustomerSellingProductDetail();
            customerSellingProductDetail.Entity = customerSellingProduct;
            SellingProduct sellingProduct = _sellingProductManager.GetSellingProduct(customerSellingProduct.SellingProductId);
            customerSellingProductDetail.CustomerName = _carrierAccountManager.GetCarrierAccountName(customerSellingProduct.CustomerId);
            if (sellingProduct != null)
            {
                customerSellingProductDetail.SellingProductName = sellingProduct.Name;
            }
            return customerSellingProductDetail;
        }

        #endregion
    }
}
