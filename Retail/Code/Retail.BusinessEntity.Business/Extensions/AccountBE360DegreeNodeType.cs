using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountBE360DegreeNodeType : BE360DegreeNodeTypeExtendedSettings
    {
        static AccountBEManager s_accountBEManager = new AccountBEManager();
        static AccountBEDefinitionManager s_accountBEDefinitionManager = new AccountBEDefinitionManager();

        public override List<BE360DegreeNode> GetChildNodes(IBE360DegreeNodeGetChildNodesContext context)
        {
            AccountBE360DegreeNodeEntityId nodeEntityId = context.Node.EntityId.CastWithValidate<AccountBE360DegreeNodeEntityId>("context.Node.EntityId");
            Guid accountBEDefinitionId = nodeEntityId.AccountBEDefinitionId;
            long accountId = nodeEntityId.AccountId;
            List<BE360DegreeNode> childNodes = new List<BE360DegreeNode>();
            List<AccountViewDefinition> viewDefinitions = s_accountBEDefinitionManager.GetAccountViewDefinitionsByAccountId(accountBEDefinitionId, accountId);
            if (viewDefinitions != null)
            {
                foreach (var viewDef in viewDefinitions)
                {
                    viewDef.Settings.ThrowIfNull("viewDef.Settings", viewDef.AccountViewDefinitionId);
                    var convertToNodeContext = new AccountViewDefinitionTryConvertToBE360DegreeNodeContext { AccountBEDefinitionId = accountBEDefinitionId, AccountId = accountId };
                    if (viewDef.Settings.TryConvertToBE360DegreeNode(convertToNodeContext))
                    {
                        convertToNodeContext.Node.ThrowIfNull("convertToNodeContext.Node", viewDef.AccountViewDefinitionId);
                        childNodes.Add(convertToNodeContext.Node);
                    }
                }
            }
            return childNodes;
        }

        public override BE360DegreeNode RefreshNode(IBE360DegreeNodeRefreshNodeContext context)
        {
            AccountBE360DegreeNodeEntityId nodeEntityId = context.Node.EntityId.CastWithValidate<AccountBE360DegreeNodeEntityId>("context.Node.EntityId");
            return CreateNode(nodeEntityId.AccountBEDefinitionId, nodeEntityId.AccountId);
        }

        public BE360DegreeNode CreateNode(Guid accountBEDefinitionId, Account account)
        {
            return new BE360DegreeNode
            {
                EntityId = new AccountBE360DegreeNodeEntityId { AccountBEDefinitionId = accountBEDefinitionId, AccountId = account.AccountId },
                Title = account.Name
            };
        }

        public BE360DegreeNode CreateNode(Guid accountBEDefinitionId, long accountId)
        {
            var account = s_accountBEManager.GetAccount(accountBEDefinitionId, accountId);
            account.ThrowIfNull("account", accountId);
            return CreateNode(accountBEDefinitionId, account);
        }

        #region Private Classes

        private class AccountViewDefinitionTryConvertToBE360DegreeNodeContext : IAccountViewDefinitionTryConvertToBE360DegreeNodeContext
        {
            public Guid AccountBEDefinitionId
            {
                get;
                set;
            }

            public long AccountId
            {
                get;
                set;
            }

            public BE360DegreeNode Node
            {
                get;
                set;
            }
        }


        #endregion
    }

    public class AccountBE360DegreeNodeEntityId
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AccountId { get; set; }
    }
}
