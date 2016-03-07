using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class BusinessEntityDescriptionContext : IBusinessEntityDescriptionContext
    {
        public BusinessEntityDefinition EntityDefinition { get; set; }
        public List<object> EntityIds { get; set; }
    }
}
