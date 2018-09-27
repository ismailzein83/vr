using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class PackageVolumeChargingIsApplicableToEventContext : IPackageUsageVolumeIsApplicableToEventContext
    {
        public PackageVolumeChargingIsApplicableToEventContext(Guid accountBEDefinitionId, long accountId, AccountPackage accountPackage, Package package, Guid serviceTypeId, Guid recordTypeId, dynamic eventRecord, DateTime eventTime)
            : this(accountBEDefinitionId, accountId, accountPackage, package, serviceTypeId, recordTypeId, eventTime)
        {
            this.Event = eventRecord;
        }


        public PackageVolumeChargingIsApplicableToEventContext(Guid accountBEDefinitionId, long accountId, AccountPackage accountPackage, Package package, Guid serviceTypeId, Guid recordTypeId, DataRecordObject eventDataRecord, DateTime eventTime)
            : this(accountBEDefinitionId, accountId, accountPackage, package, serviceTypeId, recordTypeId, eventTime)
        {
            this.EventDataRecordObject = eventDataRecord;
        }

        private PackageVolumeChargingIsApplicableToEventContext(Guid accountBEDefinitionId, long accountId, AccountPackage accountPackage, Package package, Guid serviceTypeId, Guid recordTypeId, DateTime eventTime)
        {
            this.AccountBEDefinitionId = accountBEDefinitionId;
            this.AccountId = accountId;
            this.AccountPackage = accountPackage;
            this.Package = package;
            this.ServiceTypeId = serviceTypeId;
            this.RecordTypeId = recordTypeId;
            this.EventTime = EventTime;
        }

        public Guid AccountBEDefinitionId
        {
            get;
            private set;
        }

        public long AccountId
        {
            get;
            private set;
        }

        public AccountPackage AccountPackage
        {
            get;
            private set;
        }

        public Package Package
        {
            get;
            private set;
        }

        public Guid ServiceTypeId
        {
            get;
            private set;
        }

        public Guid RecordTypeId
        {
            get;
            private set;
        }

        dynamic _event;
        public dynamic Event
        {
            get
            {
                if(_event == null)
                {
                    _eventDataRecordObject.ThrowIfNull("_eventDataRecordObject");
                    _event = _eventDataRecordObject.Object;
                }
                return _event;
            }
            private set
            {
                _event = value;
            }
        }

        DataRecordObject _eventDataRecordObject;
        public DataRecordObject EventDataRecordObject
        {
            get
            {
                if(_eventDataRecordObject == null)
                {
                    if (_event == null)
                        throw new NullReferenceException("_event");
                    _eventDataRecordObject = new DataRecordObject(this.RecordTypeId, _event);
                }
                return _eventDataRecordObject;
            }
            private set
            {
                _eventDataRecordObject = value;
            }
        }

        public DateTime EventTime
        {
            get;
            private set;
        }

        public List<Guid> ApplicableItemIds
        {
            get;
            set;
        }
    }
}
