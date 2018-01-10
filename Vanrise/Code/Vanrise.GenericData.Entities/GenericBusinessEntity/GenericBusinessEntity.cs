using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBusinessEntity
    {
        public long GenericBusinessEntityId { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }
        public Dictionary<string, Object> FieldValues { get; set; }
    }
}
