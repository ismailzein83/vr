using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Branch
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
        public int CompanyId { get; set; }
        public Setting Setting { get; set; }
    }
}
