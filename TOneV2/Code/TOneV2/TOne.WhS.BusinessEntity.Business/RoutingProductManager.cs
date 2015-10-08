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
    public class RoutingProductManager
    {
        public Vanrise.Entities.IDataRetrievalResult<RoutingProduct> GetFilteredRoutingProducts(Vanrise.Entities.DataRetrievalInput<RoutingProductQuery> input)
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredRoutingProducts(input));
        }

        public RoutingProduct GetRoutingProduct(int routingProductId)
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            return dataManager.GetRoutingProduct(routingProductId);
        }
        public List<RoutingProductInfo> GetRoutingProductsInfoBySaleZonePackage(int saleZonePackage)
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            return dataManager.GetRoutingProductsInfoBySaleZonePackage(saleZonePackage);
        }
        public List<RoutingProductInfo> GetRoutingProducts()
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            return dataManager.GetRoutingProducts();
        }

        public TOne.Entities.InsertOperationOutput<RoutingProduct> AddRoutingProduct(RoutingProduct routingProduct)
        {
            TOne.Entities.InsertOperationOutput<RoutingProduct> insertOperationOutput = new TOne.Entities.InsertOperationOutput<RoutingProduct>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            int routingProductId = -1;

            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();
            bool insertActionSucc = dataManager.Insert(routingProduct, out routingProductId);

            if (insertActionSucc)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                routingProduct.RoutingProductId = routingProductId;
                insertOperationOutput.InsertedObject = routingProduct;
            }

            return insertOperationOutput;
        }

        public TOne.Entities.UpdateOperationOutput<RoutingProduct> UpdateRoutingProduct(RoutingProduct routingProduct)
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();

            bool updateActionSucc = dataManager.Update(routingProduct);
            TOne.Entities.UpdateOperationOutput<RoutingProduct> updateOperationOutput = new TOne.Entities.UpdateOperationOutput<RoutingProduct>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = routingProduct;
            }

            return updateOperationOutput;
        }

        public TOne.Entities.DeleteOperationOutput<object> DeleteRoutingProduct(int routingProductId)
        {
            IRoutingProductDataManager dataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();

            TOne.Entities.DeleteOperationOutput<object> deleteOperationOutput = new TOne.Entities.DeleteOperationOutput<object>();
            deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Failed;

            bool deleteActionSucc = dataManager.Delete(routingProductId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;
        }
    }
}
