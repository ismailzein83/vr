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
            DIDManager didManager = new DIDManager();
            BEParentChildRelationManager beParentChildRelationManager = new BEParentChildRelationManager();
            Guid accountDIDRelationDefinitionID = new ConfigManager().GetAccountDIDRelationDefinitionId();

            long? callingAccountId;
            if (IsAccountOnNet(context.CallingNumber, accountDIDRelationDefinitionID, context.RawCDR.AttemptDateTime, didManager, beParentChildRelationManager, out callingAccountId))
            {
                context.IsCallingAccountOnNet = true;
                context.CallingAccountId = callingAccountId;
            }

            long? calledAccountId;
            if (IsAccountOnNet(context.CalledNumber, accountDIDRelationDefinitionID, context.RawCDR.AttemptDateTime, didManager, beParentChildRelationManager, out calledAccountId))
            {
                context.IsCalledAccountOnNet = true;
                context.CalledAccountId = calledAccountId;
            }
        }

        private bool IsAccountOnNet(string number, Guid accountDIDRelationDefinitionID, DateTime? effectiveOn, DIDManager didManager, BEParentChildRelationManager beParentChildRelationManager, out long? accountId)
        {
            accountId = null;
            if (!string.IsNullOrEmpty(number) && effectiveOn.HasValue)
            {
                DID relatedDID = didManager.GetDIDByNumber(number);
                if (relatedDID != null)
                {
                    BEParentChildRelation beParentChildRelation = beParentChildRelationManager.GetParent(accountDIDRelationDefinitionID, relatedDID.DIDId.ToString(), effectiveOn.Value);
                    if (beParentChildRelation != null)
                    {
                        long accountIdAsLong;
                        if (long.TryParse(beParentChildRelation.ParentBEId, out accountIdAsLong))
                            accountId = accountIdAsLong;
                        else
                            throw new Exception(String.Format("Cannot parse beParentChildRelation.ParentBEId '{0}' to long", beParentChildRelation.ParentBEId));
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
