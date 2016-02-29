using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceUpdateInput
    {
        public byte[] LastUpdateHandle { get; set; }
        public int NbOfRows { get; set; }
        public List<int> DefinitionsId { get; set; }
    }
}
