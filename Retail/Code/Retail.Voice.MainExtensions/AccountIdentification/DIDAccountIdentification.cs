using System;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Voice.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.Voice.MainExtensions
{
    public class DIDAccountIdentification : AccountIdentification
    {
        public override Guid ConfigId { get { return new Guid("1A73D2E9-1419-4B41-AD2B-6AB04930466B"); } }

        public override void Execute(IAccountIdentificationContext context)
        {
            ConfigManager beConfigManager = new ConfigManager();
            Guid accountDIDRelationDefinitionID = beConfigManager.GetAccountDIDRelationDefinitionId();
            DIDManager didManager = new DIDManager();

            BEParentChildRelationManager beParentChildRelationManager = new BEParentChildRelationManager();
             
            context.CallingAccountId = GetAccountId(context.CallingNumber, accountDIDRelationDefinitionID, context.RawCDR.ConnectDateTime, didManager, beParentChildRelationManager);
            context.CalledAccountId = GetAccountId(context.CalledNumber, accountDIDRelationDefinitionID, context.RawCDR.ConnectDateTime, didManager, beParentChildRelationManager);
        }

        private long? GetAccountId(string number, Guid accountDIDRelationDefinitionID, DateTime? effectiveOn, DIDManager didManager, BEParentChildRelationManager beParentChildRelationManager)
        {
            if (!string.IsNullOrEmpty(number) && effectiveOn.HasValue)
            {
                DID relatedDID = didManager.GetDIDByNumber(number);
                if (relatedDID != null)
                {
                    BEParentChildRelation beParentChildRelation = beParentChildRelationManager.GetParent(accountDIDRelationDefinitionID, relatedDID.DIDId.ToString(), effectiveOn.Value);
                    if (beParentChildRelation != null)
                        return long.Parse(beParentChildRelation.ParentBEId);
                }
            }
            return null;
        }
    }
}
