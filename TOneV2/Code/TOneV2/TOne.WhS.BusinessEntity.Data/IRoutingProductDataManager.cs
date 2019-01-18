using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IRoutingProductDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<Entities.RoutingProduct> GetFilteredRoutingProducts(Vanrise.Entities.DataRetrievalInput<Entities.RoutingProductQuery> input);
        //List<RoutingProductInfo> GetRoutingProductsInfoBySellingNumberPlan(int sellingNumberPlan);
        RoutingProduct GetRoutingProduct(int routingProductId);

        IEnumerable<RoutingProduct> GetRoutingProducts();

        bool Insert(RoutingProduct routingProduct, out int insertedId);

        bool Update(RoutingProductToEdit routingProduct);

        bool AreRoutingProductsUpdated(ref object updateHandle);

        bool CheckIfRoutingProductHasRelatedSaleEntities(int routingProductId);
    }
}
