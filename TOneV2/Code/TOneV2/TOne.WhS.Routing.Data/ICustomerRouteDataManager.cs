using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface ICustomerRouteDataManager : IDataManager, IBulkApplyDataManager<CustomerRoute>, IRoutingDataManager
    {
        int ParentWFRuntimeProcessId { set; }

        long ParentBPInstanceId { set; }

        IBPContext BPContext { set; }

        void ApplyCustomerRouteForDB(object preparedCustomerRoute);

        Vanrise.Entities.BigResult<Entities.CustomerRoute> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<Entities.CustomerRouteQuery> input);

        void LoadRoutes(int? customerId, string codePrefix, Action<CustomerRoute> onRouteLoaded);

        void FinalizeCurstomerRoute(Action<string> trackStep);
    }
}
