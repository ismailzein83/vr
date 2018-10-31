using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class EmployeeQuery
    {
        public string Name { get; set; }

      public List<int> SpecialityIds { get; set; }
        public List<int> DeskSizeIds { get; set; }
        public List<int> ColorIds { get; set; }

        
    }
}
