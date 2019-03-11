using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCallAnalysis.Entities
{
    public class UpdatedMappedCDRs
    {
        public List<long> UpdatedIds { get; set; }

        public List<TCAnalMappedCDR> MappedCDRsToUpdate { get; set; }

        public UpdatedMappedCDRs()
        {
            UpdatedIds = new List<long>();
            MappedCDRsToUpdate = new List<TCAnalMappedCDR>();
        }
    }
}
