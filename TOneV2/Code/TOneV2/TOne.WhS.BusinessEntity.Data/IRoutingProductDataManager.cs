using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities.RatePlanning;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IRoutingProductDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<Entities.RoutingProduct> GetFilteredRoutingProducts(Vanrise.Entities.DataRetrievalInput<Entities.RoutingProductQuery> input);
        //List<RoutingProductInfo> GetRoutingProductsInfoBySellingNumberPlan(int sellingNumberPlan);
        RoutingProduct GetRoutingProduct(int routingProductId);

        IEnumerable<RoutingProduct> GetRoutingProducts();

        IEnumerable<DefaultRoutingProduct> GetEffectiveDefaultRoutingProducts(DateTime effectiveOn);

        bool Insert(RoutingProduct routingProduct, out int insertedId);

        bool InsertOrUpdateDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, NewDefaultRoutingProduct newDefaultRoutingProducts);

        bool Update(RoutingProduct routingProduct);

        bool UpdateDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, DefaultRoutingProductChange defaultRoutingProductChange);

        bool Delete(int routingProductId);

        bool AreRoutingProductsUpdated(ref object updateHandle);
    }
}
