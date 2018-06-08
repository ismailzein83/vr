using Demo.Module.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Demo.Module.Entities;
using Demo.Module.Entities.ProductInfo;


namespace Demo.Module.Business
{
    public class ProductManager
    {

        #region Public Methods
        public IDataRetrievalResult<ProductDetails> GetFilteredProducts(DataRetrievalInput<ProductQuery> input)
        {
            var allProducts = GetCachedProducts();
            Func<Product, bool> filterExpression = (product) =>
            {
                if (input.Query.Name != null && !product.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allProducts.ToBigResult(input, filterExpression, ProductDetailMapper));

        }
        public string GetProductName(long productId)
        {
            var product = GetProductById(productId);
            if (product == null)
                return null;
            return product.Name;
        }
        public InsertOperationOutput<ProductDetails> AddProduct(Product product)
        {
            IProductDataManager productDataManager = DemoModuleFactory.GetDataManager<IProductDataManager>();
            InsertOperationOutput<ProductDetails> insertOperationOutput = new InsertOperationOutput<ProductDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long productId = -1;

            bool insertActionSuccess = productDataManager.Insert(product, out productId);
            if (insertActionSuccess)
            {
                product.ProductId = productId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ProductDetailMapper(product);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }
        public Product GetProductById(long productId)
        {
            return GetCachedProducts().GetRecord(productId);
        }
        public IEnumerable<Demo.Module.Entities.ProductInfo.ProductInfo> GetProductsInfo(ProductInfoFilter productInfoFilter)
        {
            var allProducts = GetCachedProducts();
            Func<Product, bool> filterFunc = (product) =>
            {
                if (productInfoFilter != null)
                {
                    if (productInfoFilter.Filters != null)
                    {
                        var context = new ProductInfoFilterContext
                        {
                            ProductId = product.ProductId
                        };
                        foreach (var filter in productInfoFilter.Filters)
                        {
                            if (!filter.IsMatch(context))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            };
            return allProducts.MapRecords(ProductInfoMapper, filterFunc).OrderBy(product => product.Name);
        }
        public UpdateOperationOutput<ProductDetails> UpdateProduct(Product product)
        {
            IProductDataManager productDataManager = DemoModuleFactory.GetDataManager<IProductDataManager>();
            UpdateOperationOutput<ProductDetails> updateOperationOutput = new UpdateOperationOutput<ProductDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            bool updateActionSuccess = productDataManager.Update(product);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ProductDetailMapper(product);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }
        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IProductDataManager productDataManager = DemoModuleFactory.GetDataManager<IProductDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return productDataManager.AreProductsUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Private Methods

        private Dictionary<long, Product> GetCachedProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedProducts", () =>
               {
                   IProductDataManager productDataManager = DemoModuleFactory.GetDataManager<IProductDataManager>();
                   List<Product> products = productDataManager.GetProducts();
                   return products.ToDictionary(product => product.ProductId, product => product);
               });
        }
        #endregion

        #region Mappers
        public ProductDetails ProductDetailMapper(Product product)
        {
            return new ProductDetails
            {
                Name = product.Name,
                ProductId = product.ProductId
            };
        }

        public Demo.Module.Entities.ProductInfo.ProductInfo ProductInfoMapper(Product product)
        {
            return new Demo.Module.Entities.ProductInfo.ProductInfo
            {
                Name = product.Name,
                ProductId = product.ProductId
            };
        }
        #endregion 
        
    }
}
