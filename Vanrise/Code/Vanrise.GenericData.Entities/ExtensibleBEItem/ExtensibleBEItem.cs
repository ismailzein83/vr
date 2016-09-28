using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class ExtensibleBEItem
    {
        public int ExtensibleBEItemId { get; set; }
        public int BusinessEntityDefinitionId { get; set; }
        public Guid DataRecordTypeId { get; set; }
        public List<GenericEditorSection> Sections { get; set; }
    }
}
