using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Web.Base;

namespace Vanrise.AccountBalance.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AccountBalanceNotificationType")]
    public class AccountBalanceNotificationTypeController : Vanrise.Web.Base.BaseAPIController
    {
        AccountBalanceNotificationTypeManager manager = new AccountBalanceNotificationTypeManager();

        [HttpGet]
        [Route("GetAccountBalanceNotificationTypeExtendedSettingsConfigs")]
        public IEnumerable<AccountBalanceNotificationTypeExtendedSettingsConfig> GetAccountBalanceNotificationTypeExtendedSettingsConfigs()
        {
            return manager.GetAccountBalanceNotificationTypeExtendedSettingsConfigs();
        }

        [HttpGet]
        [Route("GetAccountColumnHeader")]
        public string GetAccountColumnHeader(Guid notificationTypeId)
        {
            return manager.GetAccountColumnHeader(notificationTypeId);
        }
    }
}