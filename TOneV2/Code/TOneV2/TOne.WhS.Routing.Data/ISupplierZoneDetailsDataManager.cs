using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface ISupplierZoneDetailsDataManager : IDataManager, IBulkApplyDataManager<SupplierZoneDetail>, IRoutingDataManager
    {
        void SaveSupplierZoneDetailsForDB(List<SupplierZoneDetail> supplierZoneDetails);
        void ApplySupplierZoneDetailsForDB(object preparedSupplierZoneDetails);
        IEnumerable<SupplierZoneDetail> GetSupplierZoneDetails();
    }
}
