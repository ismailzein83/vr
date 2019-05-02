using System;
using System.Collections.Generic;

namespace Retail.BusinessEntity.Entities
{
    public interface IPackageUsageVolume
    {
        bool IsApplicableToEvent(IPackageUsageVolumeIsApplicableToEventContext context);

        void GetPackageItemsInfo(IPackageUsageVolumeGetPackageItemsInfoContext context);

        void GetChargingItems(IPackageUsageVolumeGetChargingItemsContext context);
    }

    public interface IPackageUsageVolumeIsApplicableToEventContext
    {
        Guid AccountBEDefinitionId { get; }

        long AccountId { get; }

        RetailAccountPackage RetailAccountPackage { get; }

        Package Package { get; }

        Guid ServiceTypeId { get; }

        Guid VolumePackageDefinitionId { get; }

        Dictionary<string, dynamic> RecordsByName { get; }

        DateTime EventTime { get; }

        List<Guid> ApplicableItemIds { set; }
    }

    public interface IPackageUsageVolumeGetPackageItemsInfoContext
    {
        RetailAccountPackage RetailAccountPackage { get; }

        List<Guid> ItemIds { get; }

        DateTime EventTime { get; }

        Guid VolumePackageDefinitionId { get; }

        DateTime FromTime { set; }

        DateTime ToTime { set; }

        List<PackageUsageVolumeItem> Items { set; }
    }

    public class PackageUsageVolumeItem
    {
        public Guid ItemId { get; set; }

        public Decimal Volume { get; set; }
    }

    public interface IPackageUsageVolumeGetChargingItemsContext
    {
        AccountPackage AccountPackage { get; }

        DateTime FromDate { get; }

        DateTime ToDate { get; }

        List<PackageUsageVolumeChargingItem> ChargingItems { set; }
    }

    public class PackageUsageVolumeChargingItem
    {
        public DateTime ChargingDate { get; set; }

        public decimal Price { get; set; }

        public int CurrencyId { get; set; }
    }
}