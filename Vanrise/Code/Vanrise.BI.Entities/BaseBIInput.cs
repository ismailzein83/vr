using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public class BaseBIInput
    {
       
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public  string TimeEntityName { get; set; }

        public string[] MeasureTypesNames { get; set; }
         

    }
}
