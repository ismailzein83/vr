using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BusinessEntityDefinition
    {
        public Guid BusinessEntityDefinitionId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public BusinessEntityDefinitionSettings Settings { get; set; }
    }
}
