﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Entities
{
    public abstract class BulkActionType
    {
        public abstract Guid ConfigId { get; }

        public abstract void ValidateZone(IZoneValidationContext context);

        public abstract bool IsApplicableToCountry(IBulkActionApplicableToCountryContext context);

        public abstract bool IsApplicableToZone(IActionApplicableToZoneContext context);

        public abstract void ApplyBulkActionToZoneItem(IApplyBulkActionToZoneItemContext context);

        public abstract void ApplyBulkActionToZoneDraft(IApplyBulkActionToZoneDraftContext context);

        public abstract void ApplyBulkActionToDefaultDraft(IApplyBulkActionToDefaultDraftContext context);
    }

    public interface IBulkActionApplicableToCountryContext
    {
        Country Country { get; }

        int OwnerSellingNumberPlanId { get; }

        SalePriceListOwnerType OwnerType { get; }

        int OwnerId { get; }

        Dictionary<long, ZoneChanges> ZoneDraftsByZoneId { get; }

        SaleEntityZoneRoutingProduct GetCurrentSellingProductZoneRP(int sellingProductId, long saleZoneId);

        SaleEntityZoneRoutingProduct GetCurrentCustomerZoneRP(int customerId, int sellingProductId, long saleZoneId);

        SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long zoneId, bool getFutureRate);

        SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long zoneId, bool getFutureRate);

        DateTime GetRateBED(decimal? currentRateValue, decimal newRateValue);
    }

    public interface IZoneValidationContext
    {
        long ZoneId { get; }

        ZoneItem GetContextZoneItem(long zoneId);

        int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId);

        BulkActionValidationResult ValidationResult { get; set; }
    }

    public interface IActionApplicableToZoneContext
    {
        SalePriceListOwnerType OwnerType { get; }

        int OwnerId { get; }

        SaleZone SaleZone { get; }

        ZoneChanges ZoneDraft { get; }

        SaleEntityZoneRoutingProduct GetCurrentSellingProductZoneRP(int sellingProductId, long saleZoneId);

        SaleEntityZoneRoutingProduct GetCurrentCustomerZoneRP(int customerId, int sellingProductId, long saleZoneId);

        SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long zoneId, bool getFutureRate);

        SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long zoneId, bool getFutureRate);

        DateTime GetRateBED(decimal? currentRateValue, decimal newRateValue);
    }

    public interface IApplyBulkActionToZoneItemContext
    {
        SalePriceListOwnerType OwnerType { get; }

        ZoneItem ZoneItem { get; }

        ZoneChanges ZoneDraft { get; set; }

        ZoneItem GetContextZoneItem(long zoneId);

        int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId);

        SaleEntityZoneRoutingProduct GetSellingProductZoneRoutingProduct(long zoneId);
    }

    public interface IApplyBulkActionToZoneDraftContext
    {
        SalePriceListOwnerType OwnerType { get; }

        ZoneChanges ZoneDraft { get; }

        ZoneItem GetZoneItem(long zoneId);

        int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId);
    }

    public interface IApplyBulkActionToDefaultDraftContext
    {
        SaleEntityZoneRoutingProduct GetCustomerDefaultRoutingProduct();
        DefaultChanges DefaultDraft { get; }
    }
}
