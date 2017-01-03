using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class StatusDefinitionQuery
    {
        public string Name { get; set; }
        public Guid? BusinessEntityDefinitionId { get; set; }
    }
}
