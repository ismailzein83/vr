using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Security.Business;

namespace Retail.BusinessEntity.Business
{
    public class ProductFamilyManager
    {
      
        #region Ctor/Fields

        static ProductDefinitionManager _productDefinitionManager;
        public ProductFamilyManager()
        {
            _productDefinitionManager = new ProductDefinitionManager();
        }

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<ProductFamilyDetail> GetFilteredProductFamilies(Vanrise.Entities.DataRetrievalInput<ProductFamilyQuery> input)
        {
            var allProductFamilies = GetCachedProductFamilies();
            var allowedProduct = _productDefinitionManager.GetViewAllowedProductDefinitions();

            Func<ProductFamily, bool> filterExpression =
                (x) =>
                {
                    if (input.Query != null && input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
                        return false;
                    if (allowedProduct.Count > 0 && !allowedProduct.Contains(x.Settings.ProductDefinitionId))
                        return false;

                    return true;
                };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allProductFamilies.ToBigResult(input, filterExpression, ProductFamilyDetailMapper));
        }

        public ProductFamily GetProductFamily(int productFamilyId)
        {
            Dictionary<int, ProductFamily> cachedProductFamilies = this.GetCachedProductFamilies();
            return cachedProductFamilies.GetRecord(productFamilyId);
        }

        public ProductFamilyEditorRuntime GetProductFamilyEditorRuntime(int productFamilyId)
        {
            var packageNameByIds = new Dictionary<int, string>();
            var productFamily = GetProductFamily(productFamilyId);

            PackageManager packageManager = new PackageManager();

            string packageName;
            if (productFamily != null && productFamily.Settings != null && productFamily.Settings.Packages != null)
            {
                foreach (var packageItem in productFamily.Settings.Packages.Values)
                {
                    if (!packageNameByIds.TryGetValue(packageItem.PackageId, out packageName))
                        packageNameByIds.Add(packageItem.PackageId, packageManager.GetPackageName(packageItem.PackageId));
                }
            }

            ProductFamilyEditorRuntime editorRuntime = new ProductFamilyEditorRuntime();  
            editorRuntime.PackageNameByIds = packageNameByIds;
            editorRuntime.Entity = productFamily;

            return editorRuntime;
        }

        public Vanrise.Entities.InsertOperationOutput<ProductFamilyDetail> AddProductFamily(ProductFamily productFamily)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ProductFamilyDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int productFamilyId = -1;

            IProductFamilyDataManager dataManager = BEDataManagerFactory.GetDataManager<IProductFamilyDataManager>();

            if (dataManager.Insert(productFamily, out productFamilyId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                productFamily.ProductFamilyId = productFamilyId;
                insertOperationOutput.InsertedObject = ProductFamilyDetailMapper(productFamily);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<ProductFamilyDetail> UpdateProductFamily(ProductFamily productFamily)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ProductFamilyDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IProductFamilyDataManager dataManager = BEDataManagerFactory.GetDataManager<IProductFamilyDataManager>();

            if (dataManager.Update(productFamily))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ProductFamilyDetailMapper(this.GetProductFamily(productFamily.ProductFamilyId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<ProductFamilyInfo> GetProductFamiliesInfo(ProductFamilyFilter filter)
        {
            Func<ProductFamily, bool> filterExpression = null;
            if (filter != null)
            {
              

                filterExpression = (productFamily) =>
                {


                    if (filter.Filters != null && !CheckIfFilterIsMatch(productFamily, filter.Filters))
                        return false;
                    return true;
                };
            }

            return this.GetCachedProductFamilies().MapRecords(ProductFamilyInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        //public IEnumerable<int> GetProductFamilyPackageIds(int productFamilyId)
        //{
        //    ProductFamily productFamily = new ProductFamilyManager().GetProductFamily(productFamilyId);

        //    if (productFamily == null || productFamily.Settings == null || productFamily.Settings.Packages == null || productFamily.Settings.Packages.Count == 0)
        //        return null;

        //    return productFamily.Settings.Packages.Values.Select(itm => itm.PackageId);
        //}
        
        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IProductFamilyDataManager _dataManager = BEDataManagerFactory.GetDataManager<IProductFamilyDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreProductFamilyUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, ProductFamily> GetCachedProductFamilies()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetProductFamilies",
               () =>
               {
                   IProductFamilyDataManager dataManager = BEDataManagerFactory.GetDataManager<IProductFamilyDataManager>();
                   return dataManager.GetProductFamilies().ToDictionary(x => x.ProductFamilyId, x => x);
               });
        }

        private bool CheckIfFilterIsMatch(ProductFamily productFamily, List<IProductFamilyFilter> filters)
        {
            var context = new ProductFamilyFilterContext { ProductFamilyId = productFamily.ProductFamilyId };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }


        #endregion

        #region Mappers

        public ProductFamilyDetail ProductFamilyDetailMapper(ProductFamily productFamily)
        {
            ProductFamilyDetail productFamilyDetail = new ProductFamilyDetail()
            {
                Entity = productFamily
            };
            productFamilyDetail.AllowEdit = _productDefinitionManager.DoesUserHaveEditProductDefinitions(productFamily.Settings.ProductDefinitionId);
            return productFamilyDetail;
        }

        public ProductFamilyInfo ProductFamilyInfoMapper(ProductFamily productFamily)
        {
            ProductDefinitionSettings productDefinitionSettings = new ProductDefinitionManager().GetProductDefinitionSettings(productFamily.Settings.ProductDefinitionId);

            ProductFamilyInfo productFamilyInfo = new ProductFamilyInfo()
            {
                ProductFamilyId = productFamily.ProductFamilyId,
                Name = productFamily.Name,
                AccountBEDefinitionId = productDefinitionSettings.AccountBEDefinitionId,
                ExtendedSettingsRuntimeEditor = productDefinitionSettings.ExtendedSettings != null ? productDefinitionSettings.ExtendedSettings.RuntimeEditor : null
            };
            return productFamilyInfo;
        }
    
        #endregion

        #region Security
        public HashSet<int> GetViewAllowedProductFamilies()
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return GetViewAllowedProductFamilies(userId);
        }
        public HashSet<int> GetViewAllowedProductFamilies(int userId)
        {
            HashSet<int> ids = new HashSet<int>();
            var allProducts = this.GetCachedProductFamilies();
            foreach (var p in allProducts)
            {
                if (DoesUserHaveViewAccess(userId, p.Key))
                    ids.Add(p.Key);
            }
            return ids;
        }
        public bool DoesUserHaveViewAccess(int UserId, int ProductFamilyId)
        {            
            return DoesUserHaveAccessToProductDef(ProductFamilyId, UserId, new ProductDefinitionManager().DoesUserHaveViewProductDefinition);
        }

        public bool DoesUserHaveAddProductDefinitions(int productFamilyId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAddProductDefinitions(productFamilyId, userId);
        }
        public bool DoesUserHaveAddProductDefinitions(int productFamilyId, int userId)
        {
            return DoesUserHaveAccessToProductDef(productFamilyId, userId, new ProductDefinitionManager().DoesUserHaveAddProductDefinitions);
        }

        public bool DoesUserHaveEditProductDefinitions(int productFamilyId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveEditProductDefinitions(productFamilyId, userId);
        }
        public bool DoesUserHaveEditProductDefinitions(int productFamilyId, int userId)
        {
            return DoesUserHaveAccessToProductDef(productFamilyId, userId, new ProductDefinitionManager().DoesUserHaveEditProductDefinitions);
        }
        public bool DoesUserHaveAccessToProductDef(int productFamilyId, int userId, Func<Guid, int, bool> doesUserHaveProductAccessOnAccDef)
        {
            var product = GetProductFamily(productFamilyId);
            if (product != null && product.Settings != null && product.Settings.ProductDefinitionId != null)
                return doesUserHaveProductAccessOnAccDef(product.Settings.ProductDefinitionId, userId);
            return true;
        }
        #endregion

    }
}
