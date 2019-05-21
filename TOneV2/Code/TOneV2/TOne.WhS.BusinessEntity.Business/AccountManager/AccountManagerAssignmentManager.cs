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
        public bool TryGetEffectiveNowAccountManagerAssignments(out IEnumerable<AccountManagerAssignment> effectiveAccountManagerAssignments)
        {
            return TryGetEffectiveAccountManagerAssignments(DateTime.Now, out effectiveAccountManagerAssignments);
        }
        public bool TryGetEffectiveAccountManagerAssignments(DateTime effectiveDate, out IEnumerable<AccountManagerAssignment> effectiveAccountManagerAssignments)
        {
            effectiveAccountManagerAssignments = null;
            IEnumerable<AccountManagerAssignment> accountManagerAssignments;
            bool result = TryGetAccountManagerAssignments(out accountManagerAssignments);
            if (accountManagerAssignments != null)
            {
                effectiveAccountManagerAssignments = accountManagerAssignments.FindAllRecords(x => x.BED <= effectiveDate && (!x.EED.HasValue || x.EED > effectiveDate));
            }
            return result;
        }
        public bool TryGetEffectiveAccountManagerByCarrierAccountId(int carrierAccountId, DateTime effectiveDate, out int? accountManagerId)
        {
            accountManagerId = null;
            var accountManagerAssignments = GetCachedAccountManagerAssignmentByCarrierAccountId().GetRecord(carrierAccountId);
            if (accountManagerAssignments != null)
            {
                foreach (var accountManagerAssignment in accountManagerAssignments)
                {
                    if (accountManagerAssignment.BED <= effectiveDate && (!accountManagerAssignment.EED.HasValue || accountManagerAssignment.EED > effectiveDate))
                    {
                        accountManagerId = accountManagerAssignment.AccountManagerId;
                        return true;
                    }
                }
            }
            return false;
        }
        public bool TryGetAccountManagerAssignments(out IEnumerable<AccountManagerAssignment> accountManagerAssignments)
        {
            accountManagerAssignments = null;

            AccountManagerManager accountManagerManager = new AccountManagerManager();
            var accountManagerId = accountManagerManager.GetCurrentUserAccountManagerId();
            if (accountManagerId.HasValue)
            {
                accountManagerAssignments = GetAccountManagerAssignments(accountManagerId.Value, true);
                return true;
            }
            return false;
        }
        #endregion


        #region Private Methods
        private List<AccountManagerAssignment> GetAccountManagerAssignments(int accountManagerId, bool withSubChildren)
        {
            var accountManagerTreeNode = GetCacheAccountManagerTreeNodes().GetRecord(accountManagerId);
            if (accountManagerTreeNode == null)
                return null;

            List<AccountManagerAssignment> accountManagerAssignments = new List<AccountManagerAssignment>();
            if (accountManagerTreeNode.AccountManagerAssignments != null)
                  accountManagerAssignments.AddRange(accountManagerTreeNode.AccountManagerAssignments);

            var childAccountManagerAssignments = GetAccountManagerAssignments(accountManagerTreeNode.ChildNodes, true);
            if (childAccountManagerAssignments != null)
                accountManagerAssignments.AddRange(childAccountManagerAssignments);

            return accountManagerAssignments;
        }
        private List<AccountManagerAssignment> GetAccountManagerAssignments(List<AccountManagerTreeNode> accountManagerTreeNodes, bool withSubChildren)
        {
            if (accountManagerTreeNodes != null)
            {
                List<AccountManagerAssignment> accountManagerAssignments = new List<AccountManagerAssignment>();
                foreach (var childNode in accountManagerTreeNodes)
                {
                    if (childNode.AccountManagerAssignments != null)
                        accountManagerAssignments.AddRange(childNode.AccountManagerAssignments);
                    if (withSubChildren)
                    {
                        var subChildren = GetAccountManagerAssignments(childNode.ChildNodes, withSubChildren);
                        if (subChildren != null)
                            accountManagerAssignments.AddRange(subChildren);
                    }
                }
                return accountManagerAssignments;
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
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedAccountManagerAssignmentByAccountManagerId", businessEntityDefinitionId, () =>
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

        private Dictionary<int, IOrderedEnumerable<AccountManagerAssignment>> GetCachedAccountManagerAssignmentByCarrierAccountId()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedAccountManagerAssignmentByCarrierAccountId", businessEntityDefinitionId, () =>
            {
                Dictionary<int, List<AccountManagerAssignment>> result = new Dictionary<int, List<AccountManagerAssignment>>();
                var accountManagerAssignments = GetCachedAccountManagerAssignment();
                if (accountManagerAssignments != null)
                {
                    foreach (var accountManagerAssignment in accountManagerAssignments.Values)
                    {
                        var assignments = result.GetOrCreateItem(accountManagerAssignment.CarrierAccountId);
                        assignments.Add(accountManagerAssignment);
                    }
                }
                return result.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(amItm => amItm.BED).ThenBy(amItm => amItm.EED.HasValue ? amItm.EED.Value : DateTime.MaxValue));
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
                                AccountManager = accountManager,
                                AccountManagerAssignments = accountManagerAssignmentsByManagerId.GetRecord(accountManager.AccountManagerId)
                            };
                            treeNodes.Add(accountManager.AccountManagerId, node);
                        }
                    }

                    //updating nodes parent info
                    foreach (var node in treeNodes.Values)
                    {
                        var accountManager = node.AccountManager;
                        if (accountManager.ParentId.HasValue)
                        {
                            AccountManagerTreeNode parentNode;
                            if (treeNodes.TryGetValue(accountManager.ParentId.Value, out parentNode))
                            {
                                node.ParentNode = parentNode;
                                parentNode.ChildNodes.Add(node);
                                while (parentNode.ParentNode != null)
                                {
                                    parentNode = parentNode.ParentNode;
                                }
                            }
                        }
                    }
                    return treeNodes;
                });
        }
        #endregion


        #region Private Classes
        internal class AccountManagerTreeNode
        {
            public AccountManager AccountManager { get; set; }
            public List<AccountManagerAssignment> AccountManagerAssignments { get; set; }
            public AccountManagerTreeNode ParentNode { get; set; }

            List<AccountManagerTreeNode> _childNodes = new List<AccountManagerTreeNode>();
            public List<AccountManagerTreeNode> ChildNodes
            {
                get
                {
                    return _childNodes;
                }
            }
        }
        #endregion

    }
}
