using System.Activities;
using TOne.WhS.Routing.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class GetCustomerRouteParameters : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<bool> NeedsApproval { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var configManager = new ConfigManager();

            var needsApproval = configManager.GetCustomerRouteNeedsApproval();

            if (needsApproval)
            {
                var initiatorUserId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;
                var adminUsersIds = configManager.GetAdminUsersIds();

                needsApproval = adminUsersIds == null || adminUsersIds.Count == 0 || !adminUsersIds.Contains(initiatorUserId);
            }

            this.NeedsApproval.Set(context, needsApproval);
        }
    }
}