﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface IRPRouteDataManager : IDataManager, IBulkApplyDataManager<RPRoute>, IRoutingDataManager
    {
        void ApplyProductRouteForDB(object preparedProductRoute);

        Vanrise.Entities.BigResult<Entities.RPRoute> GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<Entities.RPRouteQuery> input);

        Dictionary<int, IEnumerable<RPRouteOption>> GetRouteOptions(int routingProductId, long saleZoneId);

    }

}
