using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface ICustomerZoneDetailsDataManager : IDataManager, IBulkApplyDataManager<CustomerZoneDetail>, IRoutingDataManager
    {
        void SaveCustomerZoneDetailsToDB(List<CustomerZoneDetail> customerZoneDetails);
        void ApplyCustomerZoneDetailsToDB(object preparedCustomerZoneDetails);
        IEnumerable<CustomerZoneDetail> GetCustomerZoneDetails();
    }
}
