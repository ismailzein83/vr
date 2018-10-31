using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class PageRunTimeQuery
    {
        public int? PageDefinitionId { get; set; }
        
        public Dictionary<string, object> Filters { get; set; }
    
    }
}
