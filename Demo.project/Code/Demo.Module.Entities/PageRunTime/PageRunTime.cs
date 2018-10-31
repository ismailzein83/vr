using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class PageRunTime
    {
        public int PageRunTimeId { get; set; }
        public int PageDefinitionId { get; set; }
        public FieldDetails Details { get; set; }
    }

    public class FieldDetails
    {
        public Dictionary<string, object> FieldValues { get; set; }
    }
  
    public class FieldValue
    {
        public string Value { get; set; }

    }
}