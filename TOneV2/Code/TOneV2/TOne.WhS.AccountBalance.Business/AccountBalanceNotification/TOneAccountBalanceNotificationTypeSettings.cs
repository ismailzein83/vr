using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;

namespace TOne.WhS.AccountBalance.Business
{
    public class TOneAccountBalanceNotificationTypeSettings : AccountBalanceNotificationTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("EE2C731E-A5F7-481E-B350-7771C8A0F3BC"); } }

        public override string NotificationQueryEditor { get { return "whs-accountbalance-notificationtype-searcheditor"; } }

        public override bool IsVRNotificationMatched(IAccountBalanceNotificationTypeIsMatchedContext context)
        {
            throw new NotImplementedException();
        }
    }
}
