using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class WriteBusinessTransactionInput
    {
        public string ContractId { get; set; }
        public List<ServiceData> Services { get; set; } 
    }
}
