﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public abstract class BulkActionZoneFilter
    {
        public abstract Guid ConfigId { get; }

        public abstract IEnumerable<long> GetApplicableZoneIds(IApplicableZoneIdsContext context);
    }

    public interface IApplicableZoneIdsContext
    {
        SalePriceListOwnerType OwnerType { get; }

        int OwnerId { get; }

        IEnumerable<SaleZone> SaleZones { get; }

        Changes DraftData { get; }

        BulkActionType BulkAction { get; }

        SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long zoneId);

        SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long zoneId);

        List<CostCalculationMethod> CostCalculationMethods { get; }

        Func<Dictionary<long, ZoneItem>> GetContextZoneItems { get; set; }
        object CustomObject { get; set; }
    }
}
