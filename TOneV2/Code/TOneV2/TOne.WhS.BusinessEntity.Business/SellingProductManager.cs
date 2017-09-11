﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
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

            ResultProcessingHandler<SellingProductDetail> handler = new ResultProcessingHandler<SellingProductDetail>()
            {
                ExportExcelHandler = new SellingProductExcelExportHandler()
            };
            VRActionLogger.Current.LogGetFilteredAction(SellingProductLoggableEntity.Instance, input);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSellingProducts.ToBigResult(input, filterExpression, SellingProductDetailMapper), handler);
        }

        public SellingProduct GetSellingProductHistoryDetailbyHistoryId(int sellingProductHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var sellingProduct = s_vrObjectTrackingManager.GetObjectDetailById(sellingProductHistoryId);
            return sellingProduct.CastWithValidate<SellingProduct>("SellingProduct : historyId ", sellingProductHistoryId);
        }
        public IEnumerable<SellingProductInfo> GetSellingProductsInfo(SellingProductInfoFilter filter)
        {
            Func<SellingProduct, bool> filterPredicate = null;

            if (filter != null)
            {
                CarrierAccount assignableToCustomer = null;
                int? effectiveSellingProductId = null;

                if (filter.AssignableToCustomerId.HasValue)
                {
                    assignableToCustomer = LoadCustomer(filter.AssignableToCustomerId.Value);
                    effectiveSellingProductId = new CarrierAccountManager().GetSellingProductId(filter.AssignableToCustomerId.Value);
                }

                filterPredicate = (prod) =>
                {
                    if (filter.SellingNumberPlanId.HasValue && prod.SellingNumberPlanId != filter.SellingNumberPlanId.Value)
                        return false;

                    if (filter.AssignableToCustomerId.HasValue && !IsAssignableToCustomer(prod, filter.AssignableToCustomerId.Value, assignableToCustomer, effectiveSellingProductId))
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
        public SellingProduct GetSellingProduct(int sellingProductId, bool isViewedFromUI)
        {
            var sellingProducts = GetCachedSellingProducts();
            var sellingProduct = sellingProducts.GetRecord(sellingProductId);
            if (sellingProduct != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(SellingProductLoggableEntity.Instance, sellingProduct);
            return sellingProduct;
        }
        public SellingProduct GetSellingProduct(int sellingProductId)
        {
            return GetSellingProduct(sellingProductId, false);
        }

        public int GetSellingProductCurrencyId(int sellingProductId)
        {
            var sellingProduct = GetSellingProduct(sellingProductId);
            if (sellingProduct == null)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Selling product '{0}' was not found", sellingProductId));
            if (sellingProduct.Settings == null)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Settings of selling product '{0}' were not found", sellingProductId));
            return sellingProduct.Settings.CurrencyId;
        }

        public PricingSettings GetSellingProductPricingSettings(int sellingProductId)
        {
            var configManager = new ConfigManager();
            var sellingProduct = GetSellingProduct(sellingProductId);
            var saleAreaPricingSettings = new PricingSettings();

            sellingProduct.ThrowIfNull("Selling Product", sellingProductId);
            sellingProduct.Settings.ThrowIfNull("SellingProduct.Setting", sellingProductId);
            //sellingProduct.Settings.PricingSettings.ThrowIfNull("SellingProduct.Setting.PricingSettings", sellingProductId);

            saleAreaPricingSettings = configManager.GetSaleAreaPricingSettings();

            return configManager.MergePricingSettings(saleAreaPricingSettings, sellingProduct.Settings.PricingSettings);
        }

        public PricelistSettings GetSellingProductPricelistSettings(int sellingProductId)
        {
            var configManager = new ConfigManager();
            var sellingProduct = GetSellingProduct(sellingProductId);
            var saleAreaPricelistSettings = new PricelistSettings();

            sellingProduct.ThrowIfNull("Selling Product", sellingProductId);
            sellingProduct.Settings.ThrowIfNull("SellingProduct.Setting", sellingProductId);
            //sellingProduct.Settings.PricelistSettings.ThrowIfNull("SellingProduct.Setting.PricelistSettings", sellingProductId);

            saleAreaPricelistSettings = configManager.GetSaleAreaPricelistSettings();

            return configManager.MergePricelistSettings(saleAreaPricelistSettings, sellingProduct.Settings.PricelistSettings);
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
        public InsertOperationOutput<SellingProductDetail> AddSellingProduct(SellingProduct sellingProduct)
        {
            ValidateSellingProductToAdd(sellingProduct);

            InsertOperationOutput<SellingProductDetail> insertOperationOutput = new InsertOperationOutput<SellingProductDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int sellingProductId = -1;

            ISellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();
            bool insertActionSucc = dataManager.Insert(sellingProduct, out sellingProductId);

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                sellingProduct.SellingProductId = sellingProductId;
                VRActionLogger.Current.TrackAndLogObjectAdded(SellingProductLoggableEntity.Instance, sellingProduct);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = SellingProductDetailMapper(sellingProduct);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }


            return insertOperationOutput;
        }
        public UpdateOperationOutput<SellingProductDetail> UpdateSellingProduct(SellingProductToEdit sellingProductToEdit)
        {
            SellingProduct existingSellingProduct = GetSellingProduct(sellingProductToEdit.SellingProductId);
            ValidateSellingProductToEdit(sellingProductToEdit, existingSellingProduct);

            ISellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();

            bool updateActionSucc = dataManager.Update(sellingProductToEdit);
            UpdateOperationOutput<SellingProductDetail> updateOperationOutput = new UpdateOperationOutput<SellingProductDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                SellingProduct updatedSellingProduct = GetSellingProduct(sellingProductToEdit.SellingProductId);
                VRActionLogger.Current.TrackAndLogObjectUpdated(SellingProductLoggableEntity.Instance, updatedSellingProduct);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = SellingProductDetailMapper(updatedSellingProduct);
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

        void ValidateSellingProductToEdit(SellingProductToEdit sellingProduct, SellingProduct existingSellingProduct)
        {
            ValidateSellingProduct(sellingProduct.Name);

            if (existingSellingProduct == null)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Selling product '{0}' was not found", sellingProduct.SellingProductId));
            if (existingSellingProduct.Settings == null)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Settings of selling product '{0}' were not found", sellingProduct.SellingProductId));

            bool anyPriceListExists = new SalePriceListManager().CheckIfAnyPriceListExists(SalePriceListOwnerType.SellingProduct, sellingProduct.SellingProductId);
            if (anyPriceListExists && sellingProduct.Settings.CurrencyId != existingSellingProduct.Settings.CurrencyId)
                throw new Vanrise.Entities.VRBusinessException("Cannot change the currency of a priced selling product");
        }

        void ValidateSellingProduct(string spName)
        {
            if (String.IsNullOrWhiteSpace(spName))
                throw new MissingArgumentValidationException("SellingProduct.Name");
        }

        #endregion

        #region Private Methods

        private bool IsAssignableToCustomer(SellingProduct sellingProduct, int customerId, CarrierAccount customer, int? effectiveSellingProductId)
        {
            if (sellingProduct.SellingNumberPlanId != customer.SellingNumberPlanId.Value)
                return false;
            if (effectiveSellingProductId.HasValue && effectiveSellingProductId.Value == sellingProduct.SellingProductId)
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

        #endregion

        #region Private Members

        private class SellingProductExcelExportHandler : ExcelExportHandler<SellingProductDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<SellingProductDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Selling Products",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Description", Width = 45 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Selling Number Plan", Width = 40 });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.SellingProductId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell { Value = record.SellingNumberPlanName });
                        }
                    }
                }
                context.MainSheet = sheet;
            }
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

        public class SellingProductLoggableEntity : VRLoggableEntityBase
        {
            public static SellingProductLoggableEntity Instance = new SellingProductLoggableEntity();

            private SellingProductLoggableEntity()
            {

            }

            static SellingProductManager s_sellingProductManager = new SellingProductManager();

            public override string EntityUniqueName
            {
                get { return "WhS_BusinessEntity_SellingProduct"; }
            }

            public override string ModuleName
            {
                get { return "Business Entity"; }
            }

            public override string EntityDisplayName
            {
                get { return "Selling Product"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "WhS_BusinessEntity_SellingProduct_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                SellingProduct sellingProduct = context.Object.CastWithValidate<SellingProduct>("context.Object");
                return sellingProduct.SellingProductId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                SellingProduct sellingProduct = context.Object.CastWithValidate<SellingProduct>("context.Object");
                return s_sellingProductManager.GetSellingProductName(sellingProduct.SellingProductId);
            }
        }
        #region  Mappers
        private SellingProductDetail SellingProductDetailMapper(SellingProduct sellingProduct)
        {
            SellingProductDetail sellingProductDetail = new SellingProductDetail();

            sellingProductDetail.Entity = sellingProduct;

            SellingNumberPlanManager sellingNumberPlanManager = new SellingNumberPlanManager();
            SellingNumberPlan sellingNumberPlan = sellingNumberPlanManager.GetSellingNumberPlan(sellingProduct.SellingNumberPlanId);
            CurrencyManager currencyManager = new CurrencyManager();
            string sellingProductCurrencyName = currencyManager.GetCurrencyName(sellingProduct.Settings.CurrencyId);

            if (sellingNumberPlan != null)
            {
                sellingProductDetail.SellingNumberPlanName = sellingNumberPlan.Name;
            }
            if (sellingProductCurrencyName != null)
            {
                sellingProductDetail.SellingProductCurrencyName = sellingProductCurrencyName;
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
