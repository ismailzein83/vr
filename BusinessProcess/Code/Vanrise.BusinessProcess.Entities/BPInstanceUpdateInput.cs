using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceUpdateInput
    {
        public byte[] LastUpdateHandle { get; set; }
        public int NbOfRows { get; set; }
        public List<int> DefinitionsId { get; set; }
        public int ParentId { get; set; }
    }
}
