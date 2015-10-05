using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
   public class PricingProductManager
    {
       public Vanrise.Entities.IDataRetrievalResult<PricingProduct> GetFilteredPricingProducts(Vanrise.Entities.DataRetrievalInput<PricingProductQuery> input)
        {
            IPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IPricingProductDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredPricingProducts(input));
        }

       public PricingProduct GetPricingProduct(int pricingProductId)
        {
            IPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IPricingProductDataManager>();
            return dataManager.GetPricingProduct(pricingProductId);
        }


       public TOne.Entities.InsertOperationOutput<PricingProduct> AddPricingProduct(PricingProduct pricingProduct)
        {
            TOne.Entities.InsertOperationOutput<PricingProduct> insertOperationOutput = new TOne.Entities.InsertOperationOutput<PricingProduct>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int pricingProductId = -1;

            IPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IPricingProductDataManager>();
            bool insertActionSucc = dataManager.Insert(pricingProduct, out pricingProductId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                pricingProduct.PricingProductId = pricingProductId;
                insertOperationOutput.InsertedObject = pricingProduct;
            }

            return insertOperationOutput;
        }

       public TOne.Entities.UpdateOperationOutput<PricingProduct> UpdatePricingProduct(PricingProduct pricingProduct)
        {
            IPricingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IPricingProductDataManager>();

            bool updateActionSucc = dataManager.Update(pricingProduct);
            TOne.Entities.UpdateOperationOutput<PricingProduct> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<PricingProduct>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = pricingProduct;
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
    }
}
