using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class PackageUsageVolumeGetPackageItemsInfoContext : IPackageUsageVolumeGetPackageItemsInfoContext
    {
        public PackageUsageVolumeGetPackageItemsInfoContext(AccountPackage accountPackage, List<Guid> itemIds, DateTime eventTime)
        {
            this.AccountPackage = accountPackage;
            this.ItemIds = itemIds;
            this.EventTime = eventTime;
        }

        public AccountPackage AccountPackage
        {
            get;
            private set;
        }

        public List<Guid> ItemIds
        {
            get;
            private set;
        }

        public DateTime EventTime
        {
            get;
            private set;
        }

        public DateTime FromTime
        {
            get;
            set;
        }

        public DateTime ToTime
        {
            get;
            set;
        }

        public List<PackageUsageVolumeItem> Items
        {
            get;
            set;
        }
    }
}
