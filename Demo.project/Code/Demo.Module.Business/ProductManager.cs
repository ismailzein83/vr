using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class ProductManager
    {
        ManufactoryManager _manufactoryManager = new ManufactoryManager();
        IProductDataManager _productDataManager = DemoModuleFactory.GetDataManager<IProductDataManager>();

        #region Public Methods

        public IDataRetrievalResult<ProductDetail> GetFilteredProducts(DataRetrievalInput<ProductQuery> input)
        {
            var products = GetCachedProducts();

            Func<Product, bool> filterProducts = (product) =>
            {
                if (!string.IsNullOrEmpty(input.Query.Name) && !product.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                if (input.Query.ManufactoryIds != null && input.Query.ManufactoryIds.IndexOf(product.ManufactoryId) == -1)
                    return false;

                return true;
            };

            return DataRetrievalManager.Instance.ProcessResult(input, products.ToBigResult(input, filterProducts, ProductDetailMapper));
        }

        public Product GetProductById(long productId)
        {
            Dictionary<long, Product> cachedProducts = GetCachedProducts();
            return cachedProducts != null ? cachedProducts.GetRecord(productId) : null;
        }

        public IEnumerable<ProductSettingsConfig> GetProductSettingsConfigs()
        {
            ExtensionConfigurationManager extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<ProductSettingsConfig>(ProductSettingsConfig.EXTENSION_TYPE);
        }

        public InsertOperationOutput<ProductDetail> AddProduct(Product product)
        {
            InsertOperationOutput<ProductDetail> insertOperationOutput = new InsertOperationOutput<ProductDetail>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            long insertedId = -1;
            bool succeed = _productDataManager.InsertProduct(product, out insertedId);
            if (succeed)
            {
                product.Id = insertedId;
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

        public UpdateOperationOutput<ProductDetail> UpdateProduct(Product product)
        {
            UpdateOperationOutput<ProductDetail> updateOperationOutput = new UpdateOperationOutput<ProductDetail>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool succeed = _productDataManager.UpdateProduct(product);
            if (succeed)
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

        public IEnumerable<SoftwareOperatingSystemConfig> GetOperatingSystemConfigs()
        {
            ExtensionConfigurationManager extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<SoftwareOperatingSystemConfig>(SoftwareOperatingSystemConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Methods

        private Dictionary<long, Product> GetCachedProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedProducts", () =>
            {
                IProductDataManager productDataManager = DemoModuleFactory.GetDataManager<IProductDataManager>();
                List<Product> products = productDataManager.GetProducts();
                return products != null && products.Count > 0 ? products.ToDictionary(product => product.Id, product => product) : null;
            });
        }
        private ProductDetail ProductDetailMapper(Product product)
        {
            ProductDetail productDetail = new ProductDetail()
            {
                Id = product.Id,
                ManufactoryName = _manufactoryManager.GetManufactoryName(product.ManufactoryId),
                Name = product.Name,
                ProductDescription = null
            };

            if (product.Settings != null)
                productDetail.ProductDescription = product.Settings.GetDescription();

            return productDetail;
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

    }
}
