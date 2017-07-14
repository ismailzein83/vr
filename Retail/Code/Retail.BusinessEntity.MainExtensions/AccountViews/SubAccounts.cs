using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
    public class SubAccounts : AccountViewDefinitionSettings
    {
        static BE360DegreeManager s_be360DegreeManager = new BE360DegreeManager();

        public override Guid ConfigId
        {
            get { return new Guid("9A5B27E1-4928-4B71-B548-71C2F89444A5"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-subaccounts-view";
            }
            set
            {

            }
        }
        public override bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }

        public override bool TryConvertToBE360DegreeNode(IAccountViewDefinitionTryConvertToBE360DegreeNodeContext context)
        {
            BE360DegreeNodeType<AccountBE360DegreeNodeType> accountNodeType = s_be360DegreeManager.GetFirstNodeTypeOfT<AccountBE360DegreeNodeType>();
            accountNodeType.ThrowIfNull("accountNodeType");
            accountNodeType.ExtendedSettings.ThrowIfNull("accountNodeType.ExtendedSettings");
            context.Node = accountNodeType.ExtendedSettings.CreateNode(context.AccountBEDefinitionId, context.AccountId);
            return true;
        }
    }

    public class AccountSubAccountsBE360DegreeNodeTypeSettings : BE360DegreeNodeTypeExtendedSettings
    {
        static AccountBEManager s_accountBEManager = new AccountBEManager();
        static BE360DegreeManager s_be360DegreeManager = new BE360DegreeManager();

        public override List<BE360DegreeNode> GetChildNodes(IBE360DegreeNodeGetChildNodesContext context)
        {
            AccountBE360DegreeNodeEntityId nodeEntityId = context.Node.EntityId.CastWithValidate<AccountBE360DegreeNodeEntityId>("context.Node.EntityId");           
            BE360DegreeNodeType<AccountBE360DegreeNodeType> accountNodeType = s_be360DegreeManager.GetFirstNodeTypeOfT<AccountBE360DegreeNodeType>();
            accountNodeType.ThrowIfNull("accountNodeType");
            accountNodeType.ExtendedSettings.ThrowIfNull("accountNodeType.ExtendedSettings");
            List<BE360DegreeNode> childNodes = new List<BE360DegreeNode>();
            var subAccounts = s_accountBEManager.GetChildAccounts(nodeEntityId.AccountBEDefinitionId, nodeEntityId.AccountId, false);
            if (subAccounts != null)
                childNodes.AddRange(subAccounts.Select(subAccount => accountNodeType.ExtendedSettings.CreateNode(nodeEntityId.AccountBEDefinitionId, subAccount)));
            return childNodes;
        }

        public override BE360DegreeNode RefreshNode(IBE360DegreeNodeRefreshNodeContext context)
        {
            AccountBE360DegreeNodeEntityId nodeEntityId = context.Node.EntityId.CastWithValidate<AccountBE360DegreeNodeEntityId>("context.Node.EntityId");
            return CreateNode(nodeEntityId.AccountBEDefinitionId, nodeEntityId.AccountId);
        }

        public BE360DegreeNode CreateNode(Guid accountBEDefinitionId, long accountId)
        {
            var subAccounts = s_accountBEManager.GetChildAccounts(accountBEDefinitionId, accountId, false);
            int subAccountsCount = subAccounts != null ? subAccounts.Count : 0;
            return new BE360DegreeNode
            {
                EntityId = new AccountBE360DegreeNodeEntityId { AccountBEDefinitionId = accountBEDefinitionId, AccountId = accountId },
                Title = String.Format("Sub Accounts ({0})", subAccountsCount)
            };
        }
    }
}
