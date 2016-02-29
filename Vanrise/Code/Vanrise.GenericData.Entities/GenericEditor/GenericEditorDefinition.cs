using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericEditorDefinition
    {
        public int GenericEditorDefinitionId { get; set; }
        public int BusinessEntityId { get; set; }
        public GenericEditor Details { get; set; }
    }
}
