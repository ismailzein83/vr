﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IRoutingProductDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<Entities.RoutingProduct> GetFilteredRoutingProducts(Vanrise.Entities.DataRetrievalInput<Entities.RoutingProductQuery> input);

        RoutingProduct GetRoutingProduct(int routingProductId);

        List<RoutingProductInfo> GetRoutingProducts();

        bool Insert(RoutingProduct routingProduct, out int insertedId);

        bool Update(RoutingProduct routingProduct);

        bool Delete(int routingProductId);
    }
}
