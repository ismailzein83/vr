using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class DIDSynchronizer : TargetBESynchronizer
    {
        public Guid AccountBEDefinitionId { get; set; }
        public override string Name
        {
            get
            {
                return "DID Synchronizer";
            }
        }
        public override void Initialize(ITargetBESynchronizerInitializeContext context)
        {
            context.InitializationData = new DIDSynchronizerInitializationData();
        }
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            DIDSynchronizerInitializationData initializationData = context.InitializationData as DIDSynchronizerInitializationData;
            context.TargetBE.ThrowIfNull("context.TargetBE");
            DIDManager didManager = new DIDManager();
            foreach (var targetDID in context.TargetBE)
            {
                SourceDIDData sourceDID = targetDID as SourceDIDData;
                int DIDId;

                if (sourceDID.DID == null)
                    continue;

                DID did = sourceDID.DID;
                didManager.TryAddDID(new DIDToInsert(did.SourceId, did.Settings, false), out DIDId);
                if (DIDId > 0)
                {
                    context.WriteBusinessTrackingMsg(LogEntryType.Information, "New DID Number '{0}' is Added", sourceDID.DID.Settings.Numbers.First());
                    sourceDID.DID.DIDId = DIDId;
                    var cachedDIDs = didManager.GetCachedDIDs();
                    if (!cachedDIDs.ContainsKey((DIDId)))
                        cachedDIDs.Add(DIDId, sourceDID.DID);
                    ProcessRelatedAccounts(sourceDID, DIDId, true, context);
                }
                else
                {
                    context.WriteBusinessTrackingMsg(LogEntryType.Warning, "DID Number '{0}' was not Added", sourceDID.DID.Settings.Numbers.First());
                }
            }
        }
        void ProcessRelatedAccounts(SourceDIDData accountData, int dIDId, bool isNewDID, ITargetBESynchronizerBEsContext context)
        {
            DIDManager didManager = new DIDManager();

            Guid accountDIDRelationDefinitionId = didManager.GetAccountDIDRelationDefinitionId();
            BEParentChildRelationManager parentChildRelationManager = new BEParentChildRelationManager();
            string didIdAsString = dIDId.ToString();
            string accountIdAsString = accountData.AccountId.HasValue ? accountData.AccountId.Value.ToString() : null;
            if (isNewDID)
            {
                if (accountData.AccountId.HasValue && accountData.BED.HasValue)
                {
                    string accountName = new AccountBEManager().GetAccountName(AccountBEDefinitionId, accountData.AccountId.Value);
                    InsertParentChildRelation(accountData, accountDIDRelationDefinitionId, parentChildRelationManager, didIdAsString, accountIdAsString);
                    context.WriteBusinessTrackingMsg(LogEntryType.Information, "DID Number {0} is linked to Account {1} effective on {2}", accountData.DID.Settings.Numbers.First(), accountName, accountData.BED.Value.ToShortDateString());
                }
            }
            else
            {
                bool isSameRelatedAccountExists = false;
                IEnumerable<BEParentChildRelation> existingRelatedAccounts = parentChildRelationManager.GetParents(accountDIDRelationDefinitionId, didIdAsString);
                if (existingRelatedAccounts != null)
                {
                    DateTime dateToCloseExistingAccounts = (accountData.AccountId.HasValue && accountData.BED.HasValue) ? accountData.BED.Value : DateTime.Now;
                    foreach (var existingRelatedAccount in existingRelatedAccounts)
                    {
                        if (accountData.AccountId.HasValue && accountData.BED.HasValue)
                        {
                            if (existingRelatedAccount.ParentBEId == accountIdAsString && existingRelatedAccount.BED == accountData.BED.Value && !existingRelatedAccount.EED.HasValue)
                            {
                                isSameRelatedAccountExists = true;
                                continue;
                            }
                        }
                        if (existingRelatedAccount.EED.VRGreaterThan(dateToCloseExistingAccounts))
                        {
                            string accountName = new AccountBEManager().GetAccountName(AccountBEDefinitionId, long.Parse(existingRelatedAccount.ParentBEId.ToString()));
                            existingRelatedAccount.EED = Utilities.Max(existingRelatedAccount.BED, dateToCloseExistingAccounts);
                            parentChildRelationManager.UpdateBEParentChildRelation(existingRelatedAccount);
                            context.WriteBusinessTrackingMsg(LogEntryType.Information, "DID Number {0} link to Account {1} is closed at EED {2}.", accountData.DID.Settings.Numbers.First(), accountName, existingRelatedAccount.EED.Value.ToShortDateString());
                        }
                    }
                }
                if (accountData.AccountId.HasValue && accountData.BED.HasValue && !isSameRelatedAccountExists)
                {
                    string accountName = new AccountBEManager().GetAccountName(AccountBEDefinitionId, accountData.AccountId.Value);
                    InsertParentChildRelation(accountData, accountDIDRelationDefinitionId, parentChildRelationManager, didIdAsString, accountIdAsString);
                    context.WriteBusinessTrackingMsg(LogEntryType.Information, "DID Number {0} is linked to Account {1} effective on {2}", accountData.DID.Settings.Numbers.First(), accountName, accountData.BED.Value.ToShortDateString());
                }
            }
        }
        void InsertParentChildRelation(SourceDIDData accountData, Guid accountDIDRelationDefinitionId, BEParentChildRelationManager parentChildRelationManager, string didIdAsString, string accountIdAsString)
        {
            long beParentChildRelationId = 0;
            var beParentChildRelation = new BEParentChildRelation
            {
                BED = accountData.BED.Value,
                ChildBEId = didIdAsString,
                ParentBEId = accountIdAsString,
                RelationDefinitionId = accountDIDRelationDefinitionId
            };
            parentChildRelationManager.TryAddBEParentChildRelation(beParentChildRelation, out beParentChildRelationId);
            if (beParentChildRelationId > 0)
            {
                beParentChildRelation.BEParentChildRelationId = beParentChildRelationId;
                var cachedBEParentChildRelations = parentChildRelationManager.GetCachedBEParentChildRelations(accountDIDRelationDefinitionId);
                if (!cachedBEParentChildRelations.ContainsKey((beParentChildRelationId)))
                    cachedBEParentChildRelations.Add(beParentChildRelationId, beParentChildRelation);
            }
        }
        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            DIDSynchronizerInitializationData initializationData = context.InitializationData as DIDSynchronizerInitializationData;
            DID did;
            if (initializationData.DIDs.TryGetValue(context.SourceBEId as string, out did))
            {
                DIDManager didManager = new DIDManager();

                BEParentChildRelation firstEffectiveRelation = null;
                var parentRelations = new BEParentChildRelationManager().GetParents(new DIDManager().GetAccountDIDRelationDefinitionId(), did.DIDId.ToString());
                if (parentRelations != null)
                {
                    foreach (var relation in parentRelations)
                    {
                        if (relation.EED.HasValue)
                            continue;
                        firstEffectiveRelation = relation;
                        break;
                    }
                }
                context.TargetBE = new SourceDIDData
                {
                    DID = did,
                    BED = firstEffectiveRelation != null ? firstEffectiveRelation.BED : default(DateTime?),
                    AccountId = firstEffectiveRelation != null ? long.Parse(firstEffectiveRelation.ParentBEId) : default(long?)
                };
                return true;
            }
            return false;
        }
        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {
            foreach (var targetBe in context.TargetBE)
            {
                SourceDIDData sourceDIDData = targetBe as SourceDIDData;
                DIDManager didManager = new DIDManager();
                if (didManager.TryUpdateDID(sourceDIDData.DID))
                {
                    context.WriteBusinessTrackingMsg(LogEntryType.Information, "DID Number '{0}' is Updated", sourceDIDData.DID.Settings.Numbers.First());
                    ProcessRelatedAccounts(sourceDIDData, sourceDIDData.DID.DIDId, false, context);
                }
            }
        }
    }

    class DIDSynchronizerInitializationData
    {
        public DIDSynchronizerInitializationData()
        {
            DIDManager didManager = new DIDManager();
            this.DIDs = didManager.GetCachedDIDsGroupBySourceId();
            this.AccountDIDRelationDefinitionId = didManager.GetAccountDIDRelationDefinitionId();
        }
        public Dictionary<string, DID> DIDs { get; set; }
        public Guid AccountDIDRelationDefinitionId { get; set; }
    }
}
