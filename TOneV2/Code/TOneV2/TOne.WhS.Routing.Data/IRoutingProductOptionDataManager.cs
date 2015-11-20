using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface IRoutingProductOptionDataManager : IDataManager, 
        //IBulkApplyDataManager<RoutingProductOption>, 
        IRoutingDataManager
    {
    }
}
 