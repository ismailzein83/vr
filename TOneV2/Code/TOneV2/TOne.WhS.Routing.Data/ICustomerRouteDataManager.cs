using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
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

        IEnumerable<Entities.CustomerRoute> GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<Entities.CustomerRouteQuery> input);

        void LoadRoutes(int? customerId, string codePrefix, Func<bool> shouldStop, Action<CustomerRoute> onRouteLoaded);

        List<CustomerRouteData> GetAffectedCustomerRoutes(List<AffectedRoutes> affectedRoutesList, List<AffectedRouteOptions> affectedRouteOptionsList, long partialRoutesNumberLimit, out bool maximumExceeded);

        void UpdateCustomerRoutes(List<CustomerRouteData> customerRouteDataList);

        long GetTotalCount();

        List<CustomerRoute> GetCustomerRoutesAfterVersionNb(int versionNb);
    }
}
