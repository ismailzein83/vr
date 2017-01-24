using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class ProductFamilyManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<ProductFamilyDetail> GetFilteredProductFamilies(Vanrise.Entities.DataRetrievalInput<ProductFamilyQuery> input)
        {
            var allProductFamilies = GetCachedProductFamilies();
            Func<ProductFamily, bool> filterExpression =
                (x) =>
                {
                    if (input.Query != null && input.Query.Name != null && !x.Name.ToLower().Contains(input.Query.Name.ToLower()))
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
            //if (filter != null)
            //{
            //    filterExpression = (productFamily) =>
            //    {

            //        return true;
            //    };
            //}

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

        #endregion

        #region Mappers

        public ProductFamilyDetail ProductFamilyDetailMapper(ProductFamily productFamily)
        {
            ProductFamilyDetail productFamilyDetail = new ProductFamilyDetail()
            {
                Entity = productFamily
            };
            return productFamilyDetail;
        }

        public ProductFamilyInfo ProductFamilyInfoMapper(ProductFamily productFamily)
        {
            ProductFamilyInfo productFamilyInfo = new ProductFamilyInfo()
            {
                ProductFamilyId = productFamily.ProductFamilyId,
                Name = productFamily.Name
            };
            return productFamilyInfo;
        }

        #endregion
    }
}
