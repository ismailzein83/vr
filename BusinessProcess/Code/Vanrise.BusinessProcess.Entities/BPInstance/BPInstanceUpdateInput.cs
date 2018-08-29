using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPInstanceUpdateInput
    {
        public byte[] LastUpdateHandle { get; set; }
        public int NbOfRows { get; set; }
        public List<Guid> DefinitionsId { get; set; }
        public int ParentId { get; set; }
        public List<string> EntityIds { get; set; }
    }
}
