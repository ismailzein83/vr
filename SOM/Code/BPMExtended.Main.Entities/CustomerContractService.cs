using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CustomerContractService
    {
        public string Id { get; set; }
        public string PackageId { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime? ActivateDate { get; set; }
    }
}
