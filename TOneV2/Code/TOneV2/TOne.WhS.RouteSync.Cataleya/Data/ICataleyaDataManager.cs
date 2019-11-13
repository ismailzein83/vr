using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Cataleya.Data
{
    public interface ICataleyaDataManager
    {
        Guid ConfigId { get; }
        void Initialize(ICataleyaInitializeContext context);
        List<CarrierAccountMapping> GetCarrierAccountMappings(bool getFromTemp);
        List<CustomerIdentification> GetAllCustomerIdentifications(bool getFromTemp);
        Object PrepareDataForApply(List<ConvertedRoute> routes);
        void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context);
        void Finalize(ICataleyaFinalizeContext context);
        bool UpdateCarrierAccountMappingStatus(String customerId , CarrierAccountStatus status);
    }
}