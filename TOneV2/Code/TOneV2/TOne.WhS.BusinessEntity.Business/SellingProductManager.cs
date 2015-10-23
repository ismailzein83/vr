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
   public class SellingProductManager
    {
       public Vanrise.Entities.IDataRetrievalResult<SellingProductDetail> GetFilteredSellingProducts(Vanrise.Entities.DataRetrievalInput<SellingProductQuery> input)
        {
            var allSellingProducts = GetCachedSellingProducts();

            Func<SellingProductDetail, bool> filterExpression = (prod) =>
                 (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.SellingNumberPlanIds == null || input.Query.SellingNumberPlanIds.Contains(prod.SellingNumberPlanId))
                  &&
                 (input.Query.RoutingProductsIds == null || input.Query.RoutingProductsIds.Contains(prod.DefaultRoutingProductId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allSellingProducts.ToBigResult(input, filterExpression));
        }
       public IEnumerable<SellingProductInfo> GetAllSellingProduct()
       {
           List<SellingProductDetail> sellingProducts = GetCachedSellingProducts();
           return sellingProducts.MapRecords(SellingProductInfoMapper);
       }
       public SellingProductDetail GetSellingProduct(int sellingProductId)
        {
            List<SellingProductDetail> sellingProducts = GetCachedSellingProducts();
            return sellingProducts.FindRecord(x => x.SellingProductId == sellingProductId);
        }


       public TOne.Entities.InsertOperationOutput<SellingProductDetail> AddSellingProduct(SellingProduct sellingProduct)
        {
            TOne.Entities.InsertOperationOutput<SellingProductDetail> insertOperationOutput = new TOne.Entities.InsertOperationOutput<SellingProductDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int sellingProductId = -1;

            ISellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();
            bool insertActionSucc = dataManager.Insert(sellingProduct, out sellingProductId);

            if (insertActionSucc)
            {
                List<SellingProductDetail> sellingProducts = GetCachedSellingProducts();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                sellingProduct.SellingProductId = sellingProductId;
                insertOperationOutput.InsertedObject = sellingProducts.FindRecord(x => x.SellingProductId == sellingProductId);
            }

            return insertOperationOutput;
        }

       public TOne.Entities.UpdateOperationOutput<SellingProductDetail> UpdateSellingProduct(SellingProduct sellingProduct)
        {
            ISellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();

            bool updateActionSucc = dataManager.Update(sellingProduct);
            TOne.Entities.UpdateOperationOutput<SellingProductDetail> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<SellingProductDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                List<SellingProductDetail> sellingProducts = GetCachedSellingProducts();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = sellingProducts.FindRecord(x => x.SellingProductId == sellingProduct.SellingProductId); 
            }

            return updateOperationOutput;
        }

       public TOne.Entities.DeleteOperationOutput<object> DeleteSellingProduct(int sellingProductId)
        {
            ISellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();

            TOne.Entities.DeleteOperationOutput<object> deleteOperationOutput = new TOne.Entities.DeleteOperationOutput<object>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;

            bool deleteActionSucc = dataManager.Delete(sellingProductId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }


       #region Private Members

       List<SellingProductDetail> GetCachedSellingProducts()
       {
           return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSellingProducts",
              () =>
              {
                  ISellingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<ISellingProductDataManager>();
                  return dataManager.GetSellingProducts();
              });
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

       private SellingProductInfo SellingProductInfoMapper(SellingProductDetail sellingProductDetail)
       {
           return new SellingProductInfo()
           {
               SellingProductId = sellingProductDetail.SellingProductId,
               Name = sellingProductDetail.Name,
           };
       }

       #endregion
    }
}
