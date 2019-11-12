﻿using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Cataleya.Data
{
    public interface ICataleyaDataManager
    {
        Guid ConfigId { get; }
        List<CarrierAccountMapping> GetCarrierAccountMappings(bool getFromTemp);
        void PrepareTables(IRouteInitializeContext context);
        List<CustomerIdentification> GetAllCustomerIdentifications(bool getFromTemp);
        Object PrepareDataForApply(List<ConvertedRoute> routes);
        void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context);
        void Finalize(CataleyaFinalizeContext context);
    }
}