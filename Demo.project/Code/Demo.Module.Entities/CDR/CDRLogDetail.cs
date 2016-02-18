using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
   public class CDRLogDetail
    {
       public CDRLog Entity { get; set; }

       public string DataSourceName { get; set; }

       public string DirectionDescription { get; set; }

       public string ServiceTypeDescription { get; set; }

       public string CDRTypeDescription { get; set; }
    }
}
