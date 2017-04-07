using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;
using Vanrise.BEBridge.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business.Extensions
{
    public class DIDSynchronizer : TargetBESynchronizer
    {
        public override string Name
        {
            get
            {
                return "DID";
            }
        }
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            if (context.TargetBE == null)
                throw new NullReferenceException("context.TargetBE");
            DIDManager didManager = new DIDManager();
            foreach (var targetDID in context.TargetBE)
            {
                SourceDIDData accountData = targetDID as SourceDIDData;
                var insertedData = didManager.AddDID(accountData.DID);
            }
        }

        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            DIDManager didManager = new DIDManager();
            DID did = didManager.GetDIDBySourceId(context.SourceBEId as string);
            if (did != null)
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
            throw new NotImplementedException();
        }
    }
}
