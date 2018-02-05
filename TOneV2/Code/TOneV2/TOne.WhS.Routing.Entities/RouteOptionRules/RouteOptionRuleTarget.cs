using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class BaseRouteOptionRuleTarget : Vanrise.Rules.BaseRuleTarget,
        IRouteOptionOrderTarget, IRouteOptionFilterTarget,
        IRuleSupplierTarget, IRuleSupplierZoneTarget, IRuleCodeTarget, IRuleSaleZoneTarget, IRuleCustomerTarget, IRuleRoutingProductTarget
    {
        public RouteRuleTarget RouteTarget { get; set; }

        public int SupplierId { get; set; }

        public string SupplierCode { get; set; }

        public long SupplierZoneId { get; set; }

        public Decimal SupplierRate { get; set; }

        public bool BlockOption { get; set; }

        public bool FilterOption { get; set; }

        public int? ExecutedRuleId { get; set; }

        public HashSet<int> ExactSupplierServiceIds { get; set; }

        public HashSet<int> SupplierServiceIds { get; set; }

        public int SupplierServiceWeight { get; set; }

        public DateTime? SupplierRateEED { get; set; }

        public decimal OptionWeight { get; set; }

        public long SupplierRateId { get; set; }

        public Object OptionSettings { get; set; }

        public int NumberOfTries { get; set; }

        #region Interfaces

        string IRuleCodeTarget.Code
        {
            get { return this.RouteTarget.Code; }
        }

        long? IRuleSaleZoneTarget.SaleZoneId
        {
            get { return this.RouteTarget.SaleZoneId; }
        }

        int? IRuleCustomerTarget.CustomerId
        {
            get { return this.RouteTarget.CustomerId; }
        }
        int? IRuleRoutingProductTarget.RoutingProductId
        {
            get { return this.RouteTarget.RoutingProductId; }
        }

        int? IRuleSupplierTarget.SupplierId
        {
            get { return this.SupplierId; }
        }

        long? IRuleSupplierZoneTarget.SupplierZoneId
        {
            get { return this.SupplierZoneId; }
        }

        long? IRouteOptionOrderTarget.SaleZoneId
        {
            get { return this.RouteTarget.SaleZoneId; }
        }

        long? IRouteOptionOrderTarget.SupplierZoneId
        {
            get { return this.SupplierZoneId; }
        }
        #endregion
    }

    public class RouteOptionRuleTarget : BaseRouteOptionRuleTarget, IRouteOptionPercentageTarget
    {
        public int? Percentage { get; set; }

        public List<RouteBackupOptionRuleTarget> Backups { get; set; }

        public RouteOptionRuleTarget CloneObject()
        {
            RouteOptionRuleTarget target = new RouteOptionRuleTarget()
            {
                Backups = CloneBackups(this.Backups),
                BlockOption = this.BlockOption,
                EffectiveOn = this.EffectiveOn,
                ExactSupplierServiceIds = CloneServices(this.ExactSupplierServiceIds),
                ExecutedRuleId = this.ExecutedRuleId,
                FilterOption = this.FilterOption,
                IsEffectiveInFuture = this.IsEffectiveInFuture,
                NumberOfTries = this.NumberOfTries,
                OptionSettings = this.OptionSettings,
                OptionWeight = this.OptionWeight,
                Percentage = this.Percentage,
                RouteTarget = this.RouteTarget,
                SupplierCode = this.SupplierCode,
                SupplierId = this.SupplierId,
                SupplierRate = this.SupplierRate,
                SupplierRateEED = this.SupplierRateEED,
                SupplierRateId = this.SupplierRateId,
                SupplierServiceIds = CloneServices(this.SupplierServiceIds),
                SupplierServiceWeight = this.SupplierServiceWeight,
                SupplierZoneId = this.SupplierZoneId
            };
            return target;
        }

        private HashSet<int> CloneServices(HashSet<int> services)
        {
            if (services == null)
                return null;

            HashSet<int> clonedServices = new HashSet<int>();
            foreach (int service in services)
                clonedServices.Add(service);

            return clonedServices;
        }

        private List<RouteBackupOptionRuleTarget> CloneBackups(List<RouteBackupOptionRuleTarget> backups)
        {
            if (backups == null)
                return null;

            List<RouteBackupOptionRuleTarget> clonedBackups = new List<RouteBackupOptionRuleTarget>();
            foreach (RouteBackupOptionRuleTarget backup in backups)
                clonedBackups.Add(backup.CloneObject());

            return clonedBackups;
        }
    }

    public class RouteBackupOptionRuleTarget : BaseRouteOptionRuleTarget
    {
        public RouteBackupOptionRuleTarget CloneObject()
        {
            RouteBackupOptionRuleTarget target = new RouteBackupOptionRuleTarget()
            {
                BlockOption = this.BlockOption,
                EffectiveOn = this.EffectiveOn,
                ExactSupplierServiceIds = CloneServices(this.ExactSupplierServiceIds),
                ExecutedRuleId = this.ExecutedRuleId,
                FilterOption = this.FilterOption,
                IsEffectiveInFuture = this.IsEffectiveInFuture,
                NumberOfTries = this.NumberOfTries,
                OptionSettings = this.OptionSettings,
                OptionWeight = this.OptionWeight,
                RouteTarget = this.RouteTarget,
                SupplierCode = this.SupplierCode,
                SupplierId = this.SupplierId,
                SupplierRate = this.SupplierRate,
                SupplierRateEED = this.SupplierRateEED,
                SupplierRateId = this.SupplierRateId,
                SupplierServiceIds = CloneServices(this.SupplierServiceIds),
                SupplierServiceWeight = this.SupplierServiceWeight,
                SupplierZoneId = this.SupplierZoneId
            };
            return target;
        }

        private HashSet<int> CloneServices(HashSet<int> services)
        {
            if (services == null)
                return null;

            HashSet<int> clonedServices = new HashSet<int>();
            foreach (int service in services)
                clonedServices.Add(service);

            return clonedServices;
        }
    }
}