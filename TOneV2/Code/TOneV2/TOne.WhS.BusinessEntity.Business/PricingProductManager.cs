using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
namespace TOne.WhS.BusinessEntity.Business
{
   public class PricingProductManager
    {
       public Vanrise.Entities.IDataRetrievalResult<PricingProductDetail> GetFilteredPricingProducts(Vanrise.Entities.DataRetrievalInput<PricingProductQuery> input)
        {
            var allPricingProducts = GetCachedPricingProducts();

            Func<PricingProductDetail, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.SaleZonePackagesIds == null || input.Query.SaleZonePackagesIds.Contains(prod.SaleZonePackageId))
                  &&
                 (input.Query.RoutingProductsIds == null || input.Query.RoutingProductsIds.Contains(prod.DefaultRoutingProductId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allPricingProducts.ToBigResult(input, filterExpression));
        }
       public IEnumerable<PricingProductInfo> GetAllPricingProduct()
       {
           List<PricingProductDetail> pricingProducts = GetCachedPricingProducts();
           return pricingProducts.MapRecords(PricingProductInfoMapper);
       }
       public PricingProductDetail GetPricingProduct(int pricingProductId)
        {
            List<PricingProductDetail> pricingProducts = GetCachedPricingProducts();
            return pricingProducts.FindRecord(x => x.PricingProductId == pricingProductId);
        }


       public TOne.Entities.InsertOperationOutput<PricingProductDetail> AddPricingProduct(PricingProduct pricingProduct)
        {
            TOne.Entities.InsertOperationOutput<PricingProductDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<PricingProductDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int pricingProductId = -1;

            IPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IPricingProductDataManager>();
            bool insertActionSucc = dataManager.Insert(pricingProduct, out pricingProductId);

            if (insertActionSucc)
            {
                List<PricingProductDetail> pricingProducts = GetCachedPricingProducts();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                pricingProduct.PricingProductId = pricingProductId;
                insertOperationOutput.InsertedObject = pricingProducts.FindRecord(x => x.PricingProductId == pricingProductId);
            }

            return insertOperationOutput;
        }

       public TOne.Entities.UpdateOperationOutput<PricingProductDetail> UpdatePricingProduct(PricingProduct pricingProduct)
        {
            IPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IPricingProductDataManager>();

            bool updateActionSucc = dataManager.Update(pricingProduct);
            TOne.Entities.UpdateOperationOutput<PricingProductDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<PricingProductDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                List<PricingProductDetail> pricingProducts = GetCachedPricingProducts();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = pricingProducts.FindRecord(x => x.PricingProductId == pricingProduct.PricingProductId); 
            }

            return updateOperationOutput;
        }

       public TOne.Entities.DeleteOperationOutput<object> DeletePricingProduct(int pricingProductId)
        {
            IPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IPricingProductDataManager>();

            TOne.Entities.DeleteOperationOutput<object> deleteOperationOutput = new TOne.Entities.DeleteOperationOutput<object>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;

            bool deleteActionSucc = dataManager.Delete(pricingProductId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }


       #region Private Members

       List<PricingProductDetail> GetCachedPricingProducts()
       {
           return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPricingProducts",
              () =>
              {
                  IPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IPricingProductDataManager>();
                  return dataManager.GetPricingProducts();
              });
       }

       private class CacheManager : Vanrise.Caching.BaseCacheManager
       {
           IPricingProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<IPricingProductDataManager>();
           object _updateHandle;

           protected override bool ShouldSetCacheExpired(object parameter)
           {
               return _dataManager.ArePricingProductsUpdated(ref _updateHandle);
           }
       }

       private PricingProductInfo PricingProductInfoMapper(PricingProductDetail pricingProductDetail)
       {
           return new PricingProductInfo()
           {
               PricingProductId = pricingProductDetail.PricingProductId,
               Name = pricingProductDetail.Name,
           };
       }

       #endregion
    }
}
