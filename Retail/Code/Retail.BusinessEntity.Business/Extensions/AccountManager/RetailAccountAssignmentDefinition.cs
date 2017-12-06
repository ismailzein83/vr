using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Business;
using Vanrise.AccountManager.Entities;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class RetailAccountAssignmentDefinition : AccountManagerAssignmentDefinitionSettings
    {
        public override string GetAccountName(string accountId)
        {
            AccountBEManager accountBeManager = new AccountBEManager();
            return accountBeManager.GetAccountName(this.AccountBEDefinitionId, Convert.ToInt64(accountId));
        }
        public override void TrackAndLogObject(IAssignmentDefinitionTrackAndLogObject context)
        {
            AccountBEManager accountBeManager = new AccountBEManager();
            AccountManagerManager accountManager = new AccountManagerManager();
            var account = accountBeManager.GetAccount(this.AccountBEDefinitionId, Convert.ToInt64(context.AccountManagerAssignmentToTrack.AccountId));
            var accountManagerName = accountManager.GetAccountManagerName(context.AccountManagerAssignmentToTrack.AccountManagerId);
            VRActionLogger.Current.LogObjectCustomAction(new Retail.BusinessEntity.Business.AccountBEManager.AccountBELoggableEntity(this.AccountBEDefinitionId), "Assign Account Manager", true, account, String.Format("Account -> Account Manager {0} {1} {2}", accountManagerName, context.AccountManagerAssignmentToTrack.BED, context.AccountManagerAssignmentToTrack.EED));
        }
        public Guid AccountBEDefinitionId { get; set; }
        public Retail.BusinessEntity.Entities.AccountCondition AccountCondition { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("4CC58F9E-5ED1-4C12-8E3E-E38FC19DFF53"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-accountassignment-runtime";
            }
        }
    }
}
