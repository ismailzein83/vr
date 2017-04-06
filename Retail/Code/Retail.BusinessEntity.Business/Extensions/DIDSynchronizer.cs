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
            throw new NotImplementedException();
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
