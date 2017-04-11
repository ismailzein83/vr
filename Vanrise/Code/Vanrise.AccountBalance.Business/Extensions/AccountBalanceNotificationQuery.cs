using System;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountBalanceNotificationQuery : VRNotificationExtendedQuery
    {
        //public Decimal CurrentBalance { get; set; }

        public AccountBalanceNotificationExtendedQuery AccountBalanceNotificationExtendedQuery { get; set; }
    }

    public abstract class AccountBalanceNotificationExtendedQuery
    {

    }
}
