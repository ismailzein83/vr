using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Retail.BusinessEntity.Business
{
    public class PosSynchronizer : TargetBESynchronizer
    {
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            throw new NotImplementedException();
        }

        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
