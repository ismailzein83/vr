using Demo.Module.Data;
using Demo.Module.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class ProductManager
    {

        public InsertOperationOutput<ProductDetails> AddProduct(Product product)
        {
            IProductDataManager productDataManager = DemoModuleFactory.GetDataManager<IProductDataManager>();
            InsertOperationOutput<ProductDetails> insertOperationOutput = new InsertOperationOutput<ProductDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int productId = -1;
            bool insertActionSuccess = productDataManager.Insert(product, out productId);
            if (insertActionSuccess)
            {
                product.ProductId = productId;
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = ProductDetailMapper(product);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;

            }
            return insertOperationOutput;

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
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = ProductDetailMapper(product);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public Product GetProductById(int productId)
        {
            var allProducts = GetCachedProducts();
            return allProducts.GetRecord(productId);
        }

        public ProductDetails ProductDetailMapper(Product product)
        {
            return new ProductDetails
            {
                Entity = product
            };
        }

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

        private Dictionary<int, Product> GetCachedProducts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
                .GetOrCreateObject("GetCachedProducts", () =>
                {
                    IProductDataManager productDataManager = DemoModuleFactory.GetDataManager<IProductDataManager>();
                    List<Product> products = productDataManager.GetProducts();
                    return products.ToDictionary(product => product.ProductId, product => product);
                });
        }
       

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IProductDataManager productDataManager = DemoModuleFactory.GetDataManager<IProductDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return productDataManager.AreProductsUpdated(ref _updateHandle);
            }
        }

        public string GetProductName(int productId)
        {
            var product = GetProductById(productId);
            return product != null ? product.Name : null;
        }

        public DeleteOperationOutput<ProductDetails> Delete(int Id)
        {
            IProductDataManager productDataManager = DemoModuleFactory.GetDataManager<IProductDataManager>();
            DeleteOperationOutput<ProductDetails> deleteOperationOutput = new DeleteOperationOutput<ProductDetails>();
            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            bool deleteActionSuccess = productDataManager.Delete(Id);
            if (deleteActionSuccess)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }
            return deleteOperationOutput;
        }

        //public IEnumerable<ProductInfo> GetUniversitiesInfo()
        //{
        //    var allUniversities = GetCachedProducts();
        //    Func<Product, bool> filterFunc = null;

        //    filterFunc = (university) =>
        //    {
        //        return true;
        //    };

        //    IEnumerable<Product> filteredUniversities = (filterFunc != null) ? allUniversities.FindAllRecords(filterFunc) : allUniversities.MapRecords(u => u.Value);
        //    return filteredUniversities.MapRecords(ProductInfoMapper).OrderBy(university => university.Name);
        //}



        
    }
}
