using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBusinessEntityQuery
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public Dictionary<string, object> FilterValuesByFieldPath { get; set; }
    }
}
