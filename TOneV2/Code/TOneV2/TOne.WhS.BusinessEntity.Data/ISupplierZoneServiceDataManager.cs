using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierZoneServiceDataManager : IDataManager
    {
        List<SupplierZoneService> GetSupplierZonesServicesEffectiveAfter(int supplierId, DateTime minimumDate);

        List<SupplierZoneService> GetEffectiveSupplierZoneServicesBySuppliers(IEnumerable<RoutingSupplierInfo> supplierInfos, DateTime? effectiveOn, bool isEffectiveInFuture);

        List<SupplierDefaultService> GetEffectiveSupplierDefaultServicesBySuppliers(IEnumerable<RoutingSupplierInfo> supplierInfos, DateTime? effectiveOn, bool isEffectiveInFuture);

        SupplierDefaultService GetSupplierDefaultServiceBySupplier(int supplierId, DateTime effectiveOn);

        bool CloseOverlappedDefaultService(long supplierZoneServiceId, SupplierDefaultService supplierDefaultService, DateTime effectiveDate);

        bool Insert(SupplierDefaultService supplierZoneService);

        bool AreSupplierZoneServicesUpdated(ref object updateHandle);

        IEnumerable<SupplierDefaultService> GetEffectiveSupplierDefaultServices(DateTime from, DateTime to);

        IEnumerable<SupplierZoneService> GetEffectiveSupplierZoneServices(int supplierId, DateTime from, DateTime to);
    }
}
