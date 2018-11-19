using System;
using System.Collections.Generic;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class PackageVolumeChargingIsApplicableToEventContext : IPackageUsageVolumeIsApplicableToEventContext
    {
        public PackageVolumeChargingIsApplicableToEventContext(Guid accountBEDefinitionId, long accountId, AccountPackage accountPackage, Package package, Guid serviceTypeId,
            DateTime eventTime, Guid volumePackageDefinitionId, Dictionary<string, dynamic> recordsByName)
        {
            this.AccountBEDefinitionId = accountBEDefinitionId;
            this.AccountId = accountId;
            this.AccountPackage = accountPackage;
            this.Package = package;
            this.ServiceTypeId = serviceTypeId;
            this.EventTime = eventTime;
            this.VolumePackageDefinitionId = volumePackageDefinitionId;
            this.RecordsByName = recordsByName;
        }

        public Guid AccountBEDefinitionId { get; private set; }

        public long AccountId { get; private set; }

        public AccountPackage AccountPackage { get; private set; }

        public Package Package { get; private set; }

        public Guid ServiceTypeId { get; private set; }

        public DateTime EventTime { get; private set; }

        public Guid VolumePackageDefinitionId { get; set; }

        public Dictionary<string, dynamic> RecordsByName { get; set; }

        public List<Guid> ApplicableItemIds { get; set; }
    }
}