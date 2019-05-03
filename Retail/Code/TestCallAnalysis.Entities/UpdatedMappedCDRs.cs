using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCallAnalysis.Entities
{
    public class UpdatedMappedCDRs
    {
        public List<TCAnalMappedCDR> MappedCDRsToUpdate { get; set; }

        public UpdatedMappedCDRs()
        {
            MappedCDRsToUpdate = new List<TCAnalMappedCDR>();
        }
    }
}
