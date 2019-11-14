using Retail.BusinessEntity.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Retail.Teles.Business
{
    public class UserRGAction: VRAction
    {
        AccountBEManager _accountBEManager = new AccountBEManager();
        UserRGManager _userRGManager = new UserRGManager();
        public override void Execute(IVRActionExecutionContext context)
        {
            //var definitionSettings = context.IVRActionEventPayload as ChangeUsersRGsDefinitionSettings;
            //definitionSettings.ThrowIfNull("definitionSettings");
            //var account = _accountBEManager.GetAccount(context.AccountBEDefinitionId, context.AccountId);
            //account.ThrowIfNull("account", context.AccountId);
            //context.WriteTrackingMessage(LogEntryType.Information, string.Format("Start loading users to {0}.", context.ActionDefinitionName));
            //List<UsersToChangeRG> usersToChangeRG = _userRGManager.GetUsersToChangeRG(context.AccountBEDefinitionId, account, definitionSettings);
            //context.WriteTrackingMessage(LogEntryType.Information, string.Format("Start {0} users.", context.ActionDefinitionName));
            //_userRGManager.ChangeRGsAndUpdateState(context, usersToChangeRG, definitionSettings);
        }
    }
}
