using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
 
namespace Retail.BusinessEntity.Business
{
    public class ProductManager : IBusinessEntityManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<ProductDetail> GetFilteredProducts(Vanrise.Entities.DataRetrievalInput<ProductQuery> input)
        {
            var allProducts = GetCachedProducts();
            Func<Product, bool> filterExpression = (product) =>
                {
                    if (input.Query != null && input.Query.Name != null && !product.Name.ToLower().Contains(input.Query.Name.ToLower()))
                        return false;

                    if (input.Query != null && input.Query.ProductFamilyId.HasValue && product.Settings != null 
                        && input.Query.ProductFamilyId.Value != product.Settings.ProductFamilyId)
                        return false;

                    return true;
                };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allProducts.ToBigResult(input, filterExpression, ProductDetailMapper));
        }

        public IEnumerable<Product> GetAllProducts()
        {
            Dictionary<int, Product> cachedProducts = this.GetCachedProducts();
            return cachedProducts.Values;
        }
        public Product GetProduct(int productId)
        {
            Dictionary<int, Product> cachedProducts = this.GetCachedProducts();
            return cachedProducts.GetRecord(productId);
        }
        public string GetProductName(int productId)
        {
            Product product = this.GetProduct(productId);
            if (product == null)
                return null;
            return product.Name;
        }
        public ProductEditorRuntime GetProductEditorRuntime(int productId)
        {
            //var packageNameByIds = new Dictionary<int,string>();
            //var product = GetProduct(productId);

            //PackageManager packageManager = new PackageManager();

            //string packageName;
            //if (product != null && product.Settings != null && product.Settings.Packages != null)
            //{
            //    foreach (var packageItem in product.Settings.Packages.Values)
            //    {
            //        if (!packageNameByIds.TryGetValue(packageItem.PackageId, out packageName))
            //            packageNameByIds.Add(packageItem.PackageId, packageManager.GetPackageName(packageItem.PackageId));
            //    }
            //}

            //ProductEditorRuntime editorRuntime = new ProductEditorRuntime();
            //editorRuntime.PackageNameByIds = packageNameByIds;
            //editorRuntime.Entity = product;

            ProductEditorRuntime editorRuntime = new ProductEditorRuntime();
            editorRuntime.Entity = GetProduct(productId);
 
            return editorRuntime;
        }

        public Vanrise.Entities.InsertOperationOutput<ProductDetail> AddProduct(Product productItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<ProductDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int productId = -1;

            IProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IProductDataManager>();

            if (dataManager.Insert(productItem, out productId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                productItem.ProductId = productId;
                insertOperationOutput.InsertedObject = ProductDetailMapper(productItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<ProductDetail> UpdateProduct(Product productItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<ProductDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IProductDataManager>();

            if (dataManager.Update(productItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ProductDetailMapper(this.GetProduct(productItem.ProductId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<ProductInfo> GetProductsInfo(ProductInfoFilter filter)
        {
            Func<Product, bool> filterExpression = null;
            if (filter != null)
            {
                filterExpression = (product) =>
                {
                    if (filter.Filters != null && !CheckIfFilterIsMatch(product, filter.Filters))
                        return false;

                    return true;
                };
            }

            return this.GetCachedProducts().MapRecords(ProductInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        public IEnumerable<int> GetProductPackageIds(int productId)
        {
            Product product = new ProductManager().GetProduct(productId);
            ProductFamily ProductFamily = new ProductFamilyManager().GetProductFamily(product.Settings.ProductFamilyId);

            if (ProductFamily == null || ProductFamily.Settings == null || ProductFamily.Settings.Packages == null || ProductFamily.Settings.Packages.Count == 0)
                return null;

            return ProductFamily.Settings.Packages.Values.Select(itm => itm.PackageId);
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<IProductDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreProductUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, Product> GetCachedProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetProducts",
               () =>
               {
                   IProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IProductDataManager>();
                   return dataManager.GetProducts().ToDictionary(x => x.ProductId, x => x);
               });
        }

        private bool CheckIfFilterIsMatch(Product product, List<IProductFilter> filters)
        {
            ProductFilterContext context = new ProductFilterContext { Product = product };
            foreach (var filter in filters)
            {
                if (!filter.IsMatched(context))
                    return false;
            }
            return true;
        }

        #endregion

        #region Mappers

        public ProductDetail ProductDetailMapper(Product product)
        {
            ProductDetail productDetail = new ProductDetail()
            {
                Entity = product
            };
            return productDetail;
        }

        public ProductInfo ProductInfoMapper(Product product)
        {
            ProductInfo productInfo = new ProductInfo()
            {
                ProductId = product.ProductId,
                Name = product.Name
            };
            return productInfo;
        }

        #endregion

        #region IBusinessEntityManager

        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetProductName(Convert.ToInt32(context.EntityId));
        }

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetProduct(context.EntityId);
        }

        public dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var product = context.Entity as Product;
            return product.ProductId;
        }

        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetAllProducts().Select(itm => itm as dynamic).ToList();
        }

        public dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
 