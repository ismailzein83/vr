using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Business
{
    public class DIDSynchronizer : TargetBESynchronizer
    {
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
                var insertedDIDData = didManager.AddDID(sourceDID.DID);
                ProcessRelatedAccounts(sourceDID, insertedDIDData.InsertedObject.Entity.DIDId ,true);
            }
        }

        void ProcessRelatedAccounts(SourceDIDData accountData, int dIDId, bool isNewDID)
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
                    parentChildRelationManager.AddBEParentChildRelation(new BEParentChildRelation
                    {
                        BED = accountData.BED.Value,
                        ChildBEId = didIdAsString,
                        ParentBEId = accountIdAsString,
                        RelationDefinitionId = accountDIDRelationDefinitionId

                    });
                }
            }
            else
            {
                bool isSameRelatedAccountExists = false;
                IEnumerable<BEParentChildRelation> existingRelatedAccounts = parentChildRelationManager.GetParents(accountDIDRelationDefinitionId, didIdAsString);
                if(existingRelatedAccounts != null)
                {
                    DateTime dateToCloseExistingAccounts = (accountData.AccountId.HasValue && accountData.BED.HasValue) ? accountData.BED.Value : DateTime.Now;
                    foreach(var existingRelatedAccount in existingRelatedAccounts)
                    {
                        if(accountData.AccountId.HasValue && accountData.BED.HasValue)
                        {
                            if (existingRelatedAccount.ParentBEId == accountIdAsString && existingRelatedAccount.BED == accountData.BED.Value)
                            {
                                isSameRelatedAccountExists = true;
                                continue;
                            }
                        }
                         if(existingRelatedAccount.EED.VRGreaterThan(dateToCloseExistingAccounts))
                         {
                             existingRelatedAccount.EED = Utilities.Min(existingRelatedAccount.BED, dateToCloseExistingAccounts);
                             parentChildRelationManager.UpdateBEParentChildRelation(existingRelatedAccount);
                         }
                    }
                }
                if (accountData.AccountId.HasValue && accountData.BED.HasValue && !isSameRelatedAccountExists)
                {
                    parentChildRelationManager.AddBEParentChildRelation(new BEParentChildRelation
                    {
                        BED = accountData.BED.Value,
                        ChildBEId = didIdAsString,
                        ParentBEId = accountIdAsString,
                        RelationDefinitionId = accountDIDRelationDefinitionId
                    });
                }
            }
        }

        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            DIDSynchronizerInitializationData initializationData = context.InitializationData as DIDSynchronizerInitializationData;
            DID did;
            if (initializationData.DIDs.TryGetValue(context.SourceBEId as string, out did))
            {
                context.TargetBE = new SourceDIDData
                {
                    DID = Serializer.Deserialize<DID>(Serializer.Serialize(did))

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
                didManager.UpdateDID(sourceDIDData.DID);
                ProcessRelatedAccounts(sourceDIDData, sourceDIDData.DID.DIDId, false);
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
