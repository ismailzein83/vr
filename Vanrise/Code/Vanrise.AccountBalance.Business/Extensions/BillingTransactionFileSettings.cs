using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class BillingTransactionFileSettings : VRFileExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("ED7F9FB2-4316-4CBB-B7EF-052F2946809E"); }
        }
        public long BillingTransactionId { get; set; }
        public override bool DoesUserHaveViewAccess(IVRFileDoesUserHaveViewAccessContext context)
        {
            BillingTransactionManager billingTransactionManager = new BillingTransactionManager();
            return billingTransactionManager.DoesUserHaveViewAccess(context.UserId, BillingTransactionId);
        }
    }
}
