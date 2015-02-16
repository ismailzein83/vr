using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class Route
    {
        public string CustomerID { get; set; }

        public string Code { get; set; }        

        public int SuppliersOptionId { get; set; }

        public int SuppliersOrderOptionId { get; set; }

        public int PercentagesOptionId { get; set; }
    }
}
