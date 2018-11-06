using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class PageRunTimeDetails
    {
        public int PageRunTimeId { get; set; }

        public int PageDefinitionId { get; set; }
        public Dictionary<string, object> FieldValues { get; set; }

    }
}
