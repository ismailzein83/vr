using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
   public class CodeGroupQuery
    {
       public string Code { get; set; }
       public List<int> CountriesIds { get; set; }
    }
}
