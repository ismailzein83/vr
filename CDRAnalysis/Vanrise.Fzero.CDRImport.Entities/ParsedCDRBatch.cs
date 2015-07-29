using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Queueing.Entities;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class ParsedCDRBatch : PersistentQueueItem
    {
        public override string GenerateDescription()
        {
            return "";
        }
    }
}
