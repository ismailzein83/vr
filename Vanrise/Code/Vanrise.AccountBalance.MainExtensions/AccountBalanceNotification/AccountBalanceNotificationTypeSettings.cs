using System;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.MainExtensions.AccountBalanceNotification
{
    public class AccountBalanceNotificationTypeSettings : VRNotificationTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("0FC411D1-90FD-417C-BFDF-EC0C35B1A666"); }
        }

        public override string SearchRuntimeEditor
        {
            get
            {
                return "vr-accountbalance-notification-searcheditor";
            }
        }

        public override string BodyRuntimeEditor
        {
            get
            {
                return "vr-accountbalance-notification-bodyeditor";
            }
        }

        public override bool IsVRNotificationMatched(IVRNotificationTypeIsMatchedContext context)
        {
            throw new NotImplementedException();
        }
    }
}
