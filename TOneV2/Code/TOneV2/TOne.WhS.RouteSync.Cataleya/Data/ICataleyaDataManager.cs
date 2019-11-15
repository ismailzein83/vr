using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Cataleya.Data
{
    public interface ICataleyaDataManager
    {
        Guid ConfigId { get; }
        List<CarrierAccountMapping> GetCarrierAccountMappings(bool getFromTemp);
        void InitializeCustomerIdentificationTable();
        void InitializeCarrierAccountMappingTable();
        void InitializeRouteTables(IEnumerable<CarrierAccountMapping> carrierAccountsMappings);
        void FillTempCustomerIdentificationTable(IEnumerable<CustomerIdentification> customerIdentifications);
        void FillTempCarrierAccountMappingTable(IEnumerable<CarrierAccountMapping> carrierAccountsMappings);

        List<CustomerIdentification> GetCustomerIdentifications(bool getFromTemp);
        Object PrepareDataForApply(List<ConvertedRoute> routes);
        void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context);
        void Finalize(ICataleyaFinalizeContext context);
        //bool UpdateCarrierAccountMappingStatus(String customerId , CarrierAccountStatus status);
    }
}