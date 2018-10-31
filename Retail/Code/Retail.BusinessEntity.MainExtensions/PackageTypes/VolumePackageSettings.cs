using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.MainExtensions.PackageTypes
{
    public class VolumePackageSettings : PackageExtendedSettings, IPackageUsageVolume
    {
        public List<VolumePackageItem> Items { get; set; }

        public Decimal Price { get; set; }

        public int CurrencyId { get; set; }

        public PackageUsageVolumeRecurringPeriod RecurringPeriod { get; set; }

        public bool ReducePriceForIncompletePeriods { get; set; }

        Dictionary<Guid, PackageItemsToEvaluateByRecordTypeId> _itemsByServiceId;
        PackageItemsToEvaluateByRecordTypeId _itemsForAllServices;
        Dictionary<Guid, PackageItemToEvaluate> _itemsByItemId;

        static Vanrise.GenericData.Business.RecordFilterManager s_recordFilterManager = new Vanrise.GenericData.Business.RecordFilterManager();

        #region Public Methods

        public override void ValidatePackageAssignment(IPackageSettingAssignementValidateContext context)
        {
            context.IsValid = true;
            if(this.RecurringPeriod == null)
            {
                if(!context.EED.HasValue)
                {
                    context.ErrorMessage = "Package end date must be specified.";
                    context.IsValid = false;
                }
            }
        }

        public bool IsApplicableToEvent(IPackageUsageVolumeIsApplicableToEventContext context)
        {
            var packageItemsToEvaluate = GetPackageItemsToEvaluate(context.ServiceTypeId, context.RecordTypeId);
            if(packageItemsToEvaluate != null)
            {
                List<Guid> applicableItemIds = null;
                foreach (var packageItem in packageItemsToEvaluate)
                {
                    if (packageItem.Item.Condition == null
                        || s_recordFilterManager.IsFilterGroupMatch(packageItem.Item.Condition, new Vanrise.GenericData.Business.DataRecordFilterGenericFieldMatchContext(context.EventDataRecordObject)))
                    {
                        if (applicableItemIds == null)
                            applicableItemIds = new List<Guid>();
                        applicableItemIds.Add(packageItem.Item.VolumePackageItemId);
                    }
                }
                if (applicableItemIds != null)
                {
                    context.ApplicableItemIds = applicableItemIds;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void GetPackageItemsInfo(IPackageUsageVolumeGetPackageItemsInfoContext context)
        {
            List<PackageUsageVolumeItem> items = new List<PackageUsageVolumeItem>();
            foreach(var itemId in context.ItemIds)
            {
                PackageItemToEvaluate matchItem;
                if(_itemsByItemId.TryGetValue(itemId, out matchItem))
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
                if(recurringPeriodContext.ChargingDates != null && recurringPeriodContext.ChargingDates.Count > 0)
                {
                    context.ChargingItems = new List<PackageUsageVolumeChargingItem>();
                    foreach(var recurringPeriodChargingDate in recurringPeriodContext.ChargingDates)
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

		#region Private Methods

		private List<PackageItemToEvaluate> GetPackageItemsToEvaluate(Guid serviceTypeId, Guid recordTypeId)
        {
            PackageItemsToEvaluateByRecordTypeId packageItemsForServiceType;
            if (!_itemsByServiceId.TryGetValue(serviceTypeId, out packageItemsForServiceType))
                packageItemsForServiceType = _itemsForAllServices;
            if (packageItemsForServiceType != null)
            {
                List<PackageItemToEvaluate> packageItemsForRecordType;
                if (packageItemsForServiceType.TryGetValue(recordTypeId, out packageItemsForRecordType))
                {
                    return packageItemsForRecordType;
                }
            }
            return null;
        }

        #endregion

        #region Private Classes

        private class PackageItemToEvaluate
        {
            public VolumePackageDefinitionItem DefinitionItem { get; set; }

            public VolumePackageItem Item { get; set; }
        }

        private class PackageItemsToEvaluateByRecordTypeId : Dictionary<Guid, List<PackageItemToEvaluate>>
        {

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
    }

    public class VolumePackageItem
    {
        public Guid VolumePackageItemId { get; set; }

        public RecordFilterGroup Condition { get; set; }

        public Decimal Volume { get; set; }
    }
}
