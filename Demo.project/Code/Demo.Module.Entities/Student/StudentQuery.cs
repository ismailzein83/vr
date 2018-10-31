using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class StudentQuery
    {
        public string Name { get; set; }

        public int? Age { get; set; }
        public List<int> SchoolIds { get; set; }
        public List<int> DemoCountryIds { get; set; }
        public List<int> DemoCityIds { get; set; }


    }
}
