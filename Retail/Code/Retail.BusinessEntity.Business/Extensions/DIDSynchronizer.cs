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
                ProcessParentChildRelation(sourceDID, insertedDIDData.InsertedObject.Entity.DIDId);
            }
        }

        void ProcessParentChildRelation(SourceDIDData accountData, int dIDId)
        {
            DIDManager didManager = new DIDManager();

            Guid accountDIDRelationDefinitionId = didManager.GetAccountDIDRelationDefinitionId();
            BEParentChildRelationManager parentChildRelationManager = new BEParentChildRelationManager();
            BEParentChildRelation beParentChildRelation = parentChildRelationManager.GetParent(accountDIDRelationDefinitionId, accountData.DID.DIDId.ToString(), DateTime.Now);

            if (beParentChildRelation == null)
            {
                if (accountData.AccountId.HasValue && accountData.BED.HasValue)
                    parentChildRelationManager.AddBEParentChildRelation(new BEParentChildRelation
                    {
                        BED = accountData.BED.Value,
                        ChildBEId = dIDId.ToString(),
                        ParentBEId = accountData.AccountId.Value.ToString(),
                        RelationDefinitionId = accountDIDRelationDefinitionId

                    });
            }
            else
            {
                if (!accountData.AccountId.HasValue)
                {
                    beParentChildRelation.EED = DateTime.Now;
                    parentChildRelationManager.UpdateBEParentChildRelation(beParentChildRelation);
                }
                if (accountData.AccountId.HasValue && accountData.AccountId.Value.ToString() != beParentChildRelation.ParentBEId)
                {
                    beParentChildRelation.EED = DateTime.Now;
                    parentChildRelationManager.UpdateBEParentChildRelation(beParentChildRelation);
                    parentChildRelationManager.AddBEParentChildRelation(new BEParentChildRelation
                    {
                        BED = accountData.BED.HasValue ? accountData.BED.Value : default(DateTime),
                        ChildBEId = dIDId.ToString(),
                        ParentBEId = accountData.AccountId.HasValue ? accountData.AccountId.Value.ToString() : null,
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
                ProcessParentChildRelation(sourceDIDData, sourceDIDData.DID.DIDId);
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
