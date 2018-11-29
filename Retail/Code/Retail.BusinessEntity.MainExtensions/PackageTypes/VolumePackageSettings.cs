using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class VolumePackageSettings : PackageExtendedSettings, IPackageUsageVolume
    {
        public List<VolumePackageItem> Items { get; set; }

        public decimal Price { get; set; }

        public int CurrencyId { get; set; }

        public PackageUsageVolumeRecurringPeriod RecurringPeriod { get; set; }

        public bool ReducePriceForIncompletePeriods { get; set; }


        #region Public Methods
        public override void ValidatePackageAssignment(IPackageSettingAssignementValidateContext context)
        {
            context.IsValid = true;
            if (this.RecurringPeriod == null)
            {
                if (!context.EED.HasValue)
                {
                    context.ErrorMessage = "Package end date must be specified.";
                    context.IsValid = false;
                }
            }
        }

        public bool IsApplicableToEvent(IPackageUsageVolumeIsApplicableToEventContext context)
        {
            VolumePackageDefinitionSettings volumePackageDefinitionSettings = new VolumePackageDefinitionSettings();// get definition based on VolumePackageDefinitionId value in context

            PackageItemToEvaluateData packageItemToEvaluateData = GetPackageItemToEvaluateData(context.VolumePackageDefinitionId, volumePackageDefinitionSettings);
            if (packageItemToEvaluateData == null)
                return false;

            List<PackageItemToEvaluate> items;
            if (packageItemToEvaluateData.ItemsByServiceId != null)
                items = packageItemToEvaluateData.ItemsByServiceId.GetRecord(context.ServiceTypeId);
            else
                items = packageItemToEvaluateData.ItemsForAllServices;

            if (items == null || items.Count == 0)
                return false;

            List<Guid> applicableItemIds = null;
            foreach (var packageItem in items)
            {
                CompositeRecordConditionEvaluateContext compositeConditionContext = null;
                if (packageItem.CompositeRecordConditionDefinitionGroup != null && packageItem.CompositeRecordConditionDefinitionGroup.CompositeRecordFilterDefinitions != null)
                {
                    compositeConditionContext = new CompositeRecordConditionEvaluateContext()
                    {
                        CompositeRecordConditionFieldsByRecordName = new Dictionary<string, CompositeRecordConditionFields>()
                    };

                    foreach (CompositeRecordConditionDefinition item in packageItem.CompositeRecordConditionDefinitionGroup.CompositeRecordFilterDefinitions)
                    {
                        var recordConditionContext = new CompositeRecordConditionDefinitionSettingsGetFieldsContext();
                        item.Settings.GetFields(recordConditionContext);

                        CompositeRecordConditionFields conditionFields = new CompositeRecordConditionFields()
                        {
                            DataRecordFields = recordConditionContext.Fields,
                            FieldValues = context.RecordsByName.GetRecord(item.Name)
                        };
                        compositeConditionContext.CompositeRecordConditionFieldsByRecordName.Add(item.Name, conditionFields);
                    }
                }

                if (packageItem.Item.Condition == null || compositeConditionContext == null || packageItem.Item.Condition.Evaluate(compositeConditionContext))
                {
                    if (applicableItemIds == null)
                        applicableItemIds = new List<Guid>();
                    applicableItemIds.Add(packageItem.Item.VolumePackageItemId);
                }
            }

            if (applicableItemIds == null)
                return false;

            context.ApplicableItemIds = applicableItemIds;
            return true;
        }

        public void GetPackageItemsInfo(IPackageUsageVolumeGetPackageItemsInfoContext context)
        {
            VolumePackageDefinitionSettings volumePackageDefinitionSettings = new VolumePackageDefinitionSettings();// get definition based on VolumePackageDefinitionId value in context

            PackageItemToEvaluateData packageItemToEvaluateData = GetPackageItemToEvaluateData(context.VolumePackageDefinitionId, volumePackageDefinitionSettings);
            if (packageItemToEvaluateData == null || packageItemToEvaluateData.ItemsByItemId == null)
                return;

            List<PackageUsageVolumeItem> items = new List<PackageUsageVolumeItem>();
            foreach (var itemId in context.ItemIds)
            {
                PackageItemToEvaluate matchItem;
                if (packageItemToEvaluateData.ItemsByItemId.TryGetValue(itemId, out matchItem))
                {
                    items.Add(new PackageUsageVolumeItem
                    {
                        ItemId = itemId,
                        Volume = matchItem.Item.Volume
                    });
                }
            }
            context.Items = items;
            if (this.RecurringPeriod == null)
            {
                context.FromTime = context.AccountPackage.BED;
                if (!context.AccountPackage.EED.HasValue)
                    throw new NullReferenceException(string.Format("context.AccountPackage.EED. AccountPackageId '{0}'", context.AccountPackage.AccountPackageId));
                context.ToTime = context.AccountPackage.EED.Value;
            }
            else
            {
                var recurringPeriodContext = new PackageUsageVolumeRecurringPeriodGetEventApplicablePeriodContext(context.EventTime, context.AccountPackage.BED, context.AccountPackage.EED);
                this.RecurringPeriod.GetEventApplicablePeriod(recurringPeriodContext);
                context.FromTime = recurringPeriodContext.PeriodStart;
                var applicablePeriodToTime = recurringPeriodContext.PeriodEnd;
                if (context.AccountPackage.EED.HasValue && context.AccountPackage.EED.Value < applicablePeriodToTime)
                    applicablePeriodToTime = context.AccountPackage.EED.Value;
                context.ToTime = applicablePeriodToTime;
            }
        }

        public void GetChargingItems(IPackageUsageVolumeGetChargingItemsContext context)
        {
            if (this.RecurringPeriod == null)
            {
                if (!context.AccountPackage.EED.HasValue)
                    throw new NullReferenceException(string.Format("context.AccountPackage.EED. AccountPackageId '{0}'", context.AccountPackage.AccountPackageId));
                if (context.FromDate <= context.AccountPackage.BED && context.ToDate > context.AccountPackage.BED)
                {
                    context.ChargingItems = new List<PackageUsageVolumeChargingItem>
                    {
                        new PackageUsageVolumeChargingItem
                        {
                             ChargingDate = context.AccountPackage.BED,
                             Price = this.Price,
                             CurrencyId = this.CurrencyId
                        }
                    };
                }
            }
            else
            {
                var recurringPeriodContext = new PackageUsageVolumeRecurringPeriodGetChargingDatesContext(context.FromDate, context.ToDate, context.AccountPackage.BED, context.AccountPackage.EED);
                this.RecurringPeriod.GetChargingDates(recurringPeriodContext);
                if (recurringPeriodContext.ChargingDates != null && recurringPeriodContext.ChargingDates.Count > 0)
                {
                    context.ChargingItems = new List<PackageUsageVolumeChargingItem>();
                    foreach (var recurringPeriodChargingDate in recurringPeriodContext.ChargingDates)
                    {
                        var chargingItem = new PackageUsageVolumeChargingItem
                        {
                            ChargingDate = recurringPeriodChargingDate.ChargingDate,
                            CurrencyId = this.CurrencyId
                        };
                        if (recurringPeriodChargingDate.PeriodFractionOfTheFullPeriod.HasValue && this.ReducePriceForIncompletePeriods)
                            chargingItem.Price = this.Price * recurringPeriodChargingDate.PeriodFractionOfTheFullPeriod.Value;
                        else
                            chargingItem.Price = this.Price;
                    }
                }
            }
        }

        public override void GetExtraFields(IPackageSettingExtraFieldsContext context)
        {
            context.CurrencyId = CurrencyId;
            context.ChargeValue = this.Price;
            context.PeriodType = this.RecurringPeriod.GetDescription();
        }
        #endregion

        #region Private Classes
        private struct GetPackageItemToEvaluateDataCacheName
        {
            public Guid VolumePackageDefinitionId { get; set; }

            public override int GetHashCode()
            {
                return VolumePackageDefinitionId.GetHashCode();
            }
        }

        private class PackageItemToEvaluate
        {
            public CompositeRecordConditionDefinitionGroup CompositeRecordConditionDefinitionGroup { get; set; }

            public VolumePackageItem Item { get; set; }
        }

        private class PackageItemToEvaluateData
        {
            public List<PackageItemToEvaluate> ItemsForAllServices { get; set; }
            public Dictionary<Guid, PackageItemToEvaluate> ItemsByItemId { get; set; }
            public Dictionary<Guid, List<PackageItemToEvaluate>> ItemsByServiceId { get; set; }
        }

        private class PackageUsageVolumeRecurringPeriodGetEventApplicablePeriodContext : IPackageUsageVolumeRecurringPeriodGetEventApplicablePeriodContext
        {
            public PackageUsageVolumeRecurringPeriodGetEventApplicablePeriodContext(DateTime eventTime, DateTime packageAssignmentStartTime, DateTime? packageAssignmentEndTime)
            {
                this.EventTime = eventTime;
                this.PackageAssignmentStartTime = packageAssignmentStartTime;
                this.PackageAssignmentEndTime = packageAssignmentEndTime;
            }

            public DateTime EventTime
            {
                get;
                private set;
            }

            public DateTime PackageAssignmentStartTime
            {
                get;
                private set;
            }

            public DateTime? PackageAssignmentEndTime
            {
                get;
                private set;
            }

            public DateTime PeriodStart
            {
                get;
                set;
            }

            public DateTime PeriodEnd
            {
                get;
                set;
            }
        }

        private class PackageUsageVolumeRecurringPeriodGetChargingDatesContext : IPackageUsageVolumeRecurringPeriodGetChargingDatesContext
        {
            public PackageUsageVolumeRecurringPeriodGetChargingDatesContext(DateTime fromDate, DateTime toDate,
                DateTime packageAssignmentStartTime, DateTime? packageAssignmentEndTime)
            {
                this.FromDate = fromDate;
                this.ToDate = toDate;
                this.PackageAssignmentStartTime = packageAssignmentStartTime;
                this.PackageAssignmentEndTime = packageAssignmentEndTime;
            }

            public DateTime FromDate
            {
                get;
                private set;
            }

            public DateTime ToDate
            {
                get;
                private set;
            }

            public DateTime PackageAssignmentStartTime
            {
                get;
                private set;
            }

            public DateTime? PackageAssignmentEndTime
            {
                get;
                private set;
            }

            public List<PackageUsageVolumeRecurringPeriodChargingDate> ChargingDates
            {
                get;
                set;
            }
        }

        #endregion

        #region Private methods
        private PackageItemToEvaluateData GetPackageItemToEvaluateData(Guid volumePackageDefinitionId, VolumePackageDefinitionSettings volumePackageDefinitionSettings)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<PackageManager.CacheManager>().GetOrCreateObject(new GetPackageItemToEvaluateDataCacheName() { VolumePackageDefinitionId = volumePackageDefinitionId },
               () =>
               {
                   if (this.Items == null || volumePackageDefinitionSettings.Items == null)
                       return null;

                   var itemsByItemId = new Dictionary<Guid, PackageItemToEvaluate>();
                   var itemsByServiceId = new Dictionary<Guid, List<PackageItemToEvaluate>>();
                   var itemsForAllServices = new List<PackageItemToEvaluate>();

                   foreach (VolumePackageItem packageItem in this.Items)
                   {
                       VolumePackageDefinitionItem volumePackageDefinitionItem = volumePackageDefinitionSettings.Items.FindRecord(itm => itm.VolumePackageDefinitionItemId == packageItem.VolumePackageDefinitionItemId);
                       PackageItemToEvaluate packageItemToEvaluate = new PackageItemToEvaluate()
                       {
                           Item = packageItem,
                           CompositeRecordConditionDefinitionGroup = volumePackageDefinitionItem.CompositeRecordConditionDefinitionGroup
                       };

                       itemsByItemId.Add(packageItem.VolumePackageItemId, packageItemToEvaluate);

                       if (volumePackageDefinitionItem.ServiceTypeIds != null)
                       {
                           foreach (Guid serviceTypeId in volumePackageDefinitionItem.ServiceTypeIds)
                           {
                               List<PackageItemToEvaluate> packageItemToEvaluateList = itemsByServiceId.GetOrCreateItem(serviceTypeId, () => { return itemsForAllServices != null ? new List<PackageItemToEvaluate>(itemsForAllServices) : new List<PackageItemToEvaluate>(); });
                               packageItemToEvaluateList.Add(packageItemToEvaluate);
                           }
                       }
                       else
                       {
                           itemsForAllServices.Add(packageItemToEvaluate);

                           if (itemsByServiceId.Count > 0)
                           {
                               foreach (var itemKvp in itemsByServiceId)
                               {
                                   itemKvp.Value.Add(packageItemToEvaluate);
                               }
                           }
                       }
                   }

                   return new PackageItemToEvaluateData()
                   {
                       ItemsByItemId = itemsByItemId,
                       ItemsByServiceId = itemsByServiceId.Count > 0 ? itemsByServiceId : null,
                       ItemsForAllServices = itemsForAllServices.Count > 0 ? itemsForAllServices : null
                   };
               });
        }
        #endregion
    }

    public class VolumePackageItem
    {
        public Guid VolumePackageItemId { get; set; }

        public Guid VolumePackageDefinitionItemId { get; set; }

        public CompositeRecordCondition Condition { get; set; }

        public decimal Volume { get; set; }
    }
}