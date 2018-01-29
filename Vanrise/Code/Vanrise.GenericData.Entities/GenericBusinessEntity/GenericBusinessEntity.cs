using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBusinessEntity : IGenericObject
    {
        public Dictionary<string, Object> FieldValues { get; set; }
    }
    public class GenericBusinessEntityToAdd : GenericBusinessEntity
    {
        public Guid BusinessEntityDefinitionId { get; set; }

    }
    public class GenericBusinessEntityToUpdate : GenericBusinessEntity
    {
        public Object GenericBusinessEntityId { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }

    }
}
