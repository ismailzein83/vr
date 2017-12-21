    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class Pop3MailMessageSynchronizer : TargetBESynchronizer
    {
        public override string Name
        {
            get
            {
                return "Supplier POP3 Mail Message Synchronizer";
            }
        }

        public override void InsertBEs(ITargetBESynchronizerInsertBEsContext context)
        {
            foreach(var targetBE in context.TargetBE)
            {
                int x = 0;
            }
        }

        public override bool TryGetExistingBE(ITargetBESynchronizerTryGetExistingBEContext context)
        {
            return true;
        }

        public override void UpdateBEs(ITargetBESynchronizerUpdateBEsContext context)
        {
            
        }
    }
}
