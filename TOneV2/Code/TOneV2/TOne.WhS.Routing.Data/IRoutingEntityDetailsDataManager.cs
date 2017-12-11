using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface IRoutingEntityDetailsDataManager : IDataManager, IRoutingDataManager
    {
        RoutingEntityDetails GetRoutingEntityDetails(RoutingEntityType routingEntityType);

        void ApplyRoutingEntityDetails(RoutingEntityDetails routingEntityDetails);
    }
}