using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class AccountManagerAssignmentManager
    {
        static Guid businessEntityDefinitionId = new Guid("a2e4b8e6-2605-40e6-8184-8798add51bab");

        #region Public Methods
        public IEnumerable<AccountManagerAssignment> GetAllAccountManagerAssignmentsByCarrierAccountId(int carrierAccountId)
        {
            return GetCachedAccountManagerAssignment().FindAllRecords(itm => itm.CarrierAccountId == carrierAccountId);
        }
        public IEnumerable<int> GetAffectedCarrierAccountIds()
        {
            List<int> affectedCarrierAccountIds = new List<int>();
            var allAccountManagerAssignments = GetCachedAccountManagerAssignment().Values;
            foreach (var assignment in allAccountManagerAssignments)
            {
                affectedCarrierAccountIds.Add(assignment.CarrierAccountId);
            }
            return affectedCarrierAccountIds;
        }
        public IEnumerable<AccountManagerAssignment> TryGetEffectiveAccountManagerAssignments(DateTime effectiveDate)
        {
            var accountManagerAssignments = TryGetAccountManagerAssignments();
            if (accountManagerAssignments == null)
                return null;
            return accountManagerAssignments.FindAllRecords(x => x.BED <= effectiveDate && (!x.EED.HasValue || x.EED > effectiveDate));
        }
        public AccountManager TryGetEffectiveAccountManagerByAccountId(int carrierAccountId, DateTime effectiveDate)
        {
            var effectiveAssignments = TryGetEffectiveAccountManagerAssignments(effectiveDate);
            if (effectiveAssignments == null)
                return null;
            var effectiveAssignment = effectiveAssignments.FindRecord(x => x.CarrierAccountId == carrierAccountId);
            if (effectiveAssignment == null)
                return null;
            return new AccountManagerManager().GetAccountManagerById(effectiveAssignment.AccountManagerId);
        }
        public IEnumerable<AccountManagerAssignment> TryGetAccountManagerAssignments()
        {
            int? userId = null;
            if (Vanrise.Security.Entities.ContextFactory.GetContext().TryGetLoggedInUserId(out userId))
            {
                if (userId.HasValue)
                {
                    AccountManagerManager accountManagerManager = new AccountManagerManager();
                    var accountManagerId = accountManagerManager.GetRootAccountManagerId();
                    if (accountManagerId.HasValue)
                    {
                        return GetChildAccountManagerAssignments(accountManagerId.Value, true);
                    }
                }
            }
            return null;
        }
        #endregion

        #region Private Methods
        private List<AccountManagerAssignment> GetChildAccountManagerAssignments(int accountManagerId, bool withSubChildren)
        {
            var accountManagerTreeNode = GetCacheAccountManagerTreeNodes().GetRecord(accountManagerId);
            accountManagerTreeNode.ThrowIfNull("accountTreeNode", accountManagerId);
            if (accountManagerTreeNode.ChildNodes != null)
            {
                List<AccountManagerAssignment> childAccountManagers = new List<AccountManagerAssignment>();
                foreach (var childNode in accountManagerTreeNode.ChildNodes)
                {
                    childAccountManagers.AddRange(childNode.AccountManagerAssignmentNode.AccountManagerAssignments);
                    if (withSubChildren)
                    {
                        var subChildren = GetChildAccountManagerAssignments(childNode.AccountManagerAssignmentNode.AccountManager.AccountManagerId, withSubChildren);
                        if (subChildren != null)
                            childAccountManagers.AddRange(subChildren);
                    }
                }
                return childAccountManagers;
            }
            else
            {
                return null;
            }
        }
        private Dictionary<int, AccountManagerAssignment> GetCachedAccountManagerAssignment()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedAccountManagerAssignment", businessEntityDefinitionId, () =>
            {
                Dictionary<int, AccountManagerAssignment> result = new Dictionary<int, AccountManagerAssignment>();
                IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        AccountManagerAssignment accountManagerAssignment = new AccountManagerAssignment()
                        {
                            AccountManagerAssignmentId = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            AccountManagerId = (int)genericBusinessEntity.FieldValues.GetRecord("AccountManagerId"),
                            CarrierAccountId = (int)genericBusinessEntity.FieldValues.GetRecord("CarrierAccountId"),
                            BED = (DateTime)genericBusinessEntity.FieldValues.GetRecord("BED"),
                            EED = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("EED"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
                        };
                        result.Add(accountManagerAssignment.AccountManagerAssignmentId, accountManagerAssignment);
                    }
                }
                return result;
            });
        }
        private Dictionary<int, List<AccountManagerAssignment>> GetCachedAccountManagerAssignmentByAccountManagerId()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedAccountManagerAssignment", businessEntityDefinitionId, () =>
            {
                Dictionary<int, List<AccountManagerAssignment>> result = new Dictionary<int, List<AccountManagerAssignment>>();
                var accountManagerAssignments = GetCachedAccountManagerAssignment();
                if(accountManagerAssignments != null)
                {
                    foreach(var accountManagerAssignment in accountManagerAssignments.Values)
                    {
                        var assignments = result.GetOrCreateItem(accountManagerAssignment.AccountManagerId);
                        assignments.Add(accountManagerAssignment);
                    }
                }
                return result;
            });
        }
        internal Dictionary<int, AccountManagerTreeNode> GetCacheAccountManagerTreeNodes()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();

            return genericBusinessEntityManager.GetCachedOrCreate("GetCacheAccountManagerTreeNodes", businessEntityDefinitionId,
                () =>
                {
                    Dictionary<int, AccountManagerTreeNode> treeNodes = new Dictionary<int, AccountManagerTreeNode>();
                    var accountManagers = new AccountManagerManager().GetAllAccountManagers();
                    if(accountManagers != null)
                    {
                        var accountManagerAssignmentsByManagerId = GetCachedAccountManagerAssignmentByAccountManagerId();
                        foreach (var accountManager in accountManagers)
                        {
                            AccountManagerTreeNode node = new AccountManagerTreeNode
                            {
                                AccountManagerAssignmentNode = new AccountManagerAssignmentNode
                                {
                                    AccountManager = accountManager,
                                    AccountManagerAssignments = accountManagerAssignmentsByManagerId.GetRecord(accountManager.AccountManagerId)
                                }
                            };
                            treeNodes.Add(accountManager.AccountManagerId, node);
                        }
                    }

                    //updating nodes parent info
                    foreach (var node in treeNodes.Values)
                    {
                        var accountManager = node.AccountManagerAssignmentNode.AccountManager;
                        if (accountManager.ParentId.HasValue)
                        {
                            AccountManagerTreeNode parentNode;
                            if (treeNodes.TryGetValue(accountManager.ParentId.Value, out parentNode))
                            {
                                node.ParentNode = parentNode;
                                parentNode.ChildNodes.Add(node);
                                parentNode.TotalSubAccountManagersCount++;
                                while (parentNode.ParentNode != null)
                                {
                                    parentNode = parentNode.ParentNode;
                                    parentNode.TotalSubAccountManagersCount++;
                                }
                            }
                        }
                    }
                    return treeNodes;
                });
        }
        #endregion


        #region Private Classes
        public class AccountManagerAssignmentNode
        {
            public AccountManager AccountManager { get; set; }
            public List<AccountManagerAssignment> AccountManagerAssignments { get; set; }
        }

        internal class AccountManagerTreeNode
        {
            public AccountManagerAssignmentNode AccountManagerAssignmentNode { get; set; }

            public AccountManagerTreeNode ParentNode { get; set; }

            List<AccountManagerTreeNode> _childNodes = new List<AccountManagerTreeNode>();
            public List<AccountManagerTreeNode> ChildNodes
            {
                get
                {
                    return _childNodes;
                }
            }
            public int TotalSubAccountManagersCount { get; set; }
        }
        #endregion

    }
}
