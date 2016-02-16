using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
   public class NationalNumberingPlanQuery
    {
       public List<int> NationalNumberingPlanIds { get; set; }

       public string Name { get; set; }

       public List<int> CountriesIds { get; set; }

       public string Company { get; set; }
    }
}
