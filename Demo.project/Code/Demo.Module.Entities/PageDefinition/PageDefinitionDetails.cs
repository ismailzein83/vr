using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class PageDefinitionDetails
    {
        public int PageDefinitionId { get; set; }

        public string Name { get;set;}
        public List<Field> Fields { get; set; }
        public List<SubView> SubViews { get; set; }

    }
}
