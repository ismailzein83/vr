using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DeleteGenericBusinessEntityInput
    {
        public List<object> GenericBusinessEntityIds { get; set; }

        public Guid BusinessEntityDefinitionId { get; set; }
    }
}
