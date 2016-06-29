using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public class MigrationInfoContext
    {
        public GeneratedIdsInfoContext GeneratedIdsInfoContext { get; set; }
    }

    public class GeneratedIdsInfoContext
    {
        public int TypeId { get; set; }
        public long LastTakenId { get; set; }
    }
}
