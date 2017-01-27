using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class StatusDefinitionInfoFilter
    {
        public Guid? BusinessEntityDefinitionId { get; set; }
        public List<IStatusDefinitionFilter> Filters { get; set; }
    }
}
