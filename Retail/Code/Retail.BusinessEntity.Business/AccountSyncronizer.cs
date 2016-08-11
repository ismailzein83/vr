using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Retail.BusinessEntity.Business
{
    public class AccountSyncronizer : TargetBESynchronizer
    {
        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            throw new NotImplementedException();
        }

        public override void UpdateBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
