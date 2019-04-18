﻿using System;
using System.Collections.Generic;
using Vanrise.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Data
{
    public interface ICustomerZoneDetailsDataManager : IDataManager, IBulkApplyDataManager<CustomerZoneDetail>, IRoutingDataManager
    {
        DateTime? EffectiveDate { get; set; }
        bool? IsFuture { get; set; }
        void ApplyCustomerZoneDetailsToDB(object preparedCustomerZoneDetails);
        IEnumerable<CustomerZoneDetail> GetCustomerZoneDetails();
        IEnumerable<CustomerZoneDetail> GetFilteredCustomerZoneDetailsByZone(IEnumerable<long> saleZoneIds);
        List<CustomerZoneDetail> GetCustomerZoneDetails(HashSet<CustomerSaleZone> customerSaleZones);
        List<CustomerZoneDetail> GetCustomerZoneDetailsAfterVersionNumber(int versionNumber);
        List<CustomerZoneDetail> GetCustomerZoneDetailsByZoneRange(long fromZoneId, long toZoneId);
        void UpdateCustomerZoneDetails(List<CustomerZoneDetail> customerZoneDetails);
        List<CustomerZoneDetail> GetCustomerZoneDetailsByZoneIdsAndCustomerIds(List<long> saleZoneIds, List<int> customerIds);
    }
}