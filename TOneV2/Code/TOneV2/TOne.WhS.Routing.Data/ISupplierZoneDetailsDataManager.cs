using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface ISupplierZoneDetailsDataManager : IDataManager, IBulkApplyDataManager<SupplierZoneDetail>, IRoutingDataManager
    {
        DateTime? EffectiveDate { get; set; }
        bool? IsFuture { get; set; }
        void ApplySupplierZoneDetailsForDB(object preparedSupplierZoneDetails);
        IEnumerable<SupplierZoneDetail> GetFilteredSupplierZoneDetailsBySupplierZone(IEnumerable<long> supplierZoneIds);
        IEnumerable<SupplierZoneDetail> GetSupplierZoneDetails();
        void UpdateSupplierZoneDetails(List<SupplierZoneDetail> supplierZoneDetails);

        List<SupplierZoneDetail> GetSupplierZoneDetailsAfterVersionNumber(int versionNumber);
    }
}