using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public class ExcelTemplateRecord
    {
        public string Zone { get; set; }
        public string Code { get; set; }
        public decimal Rate { get; set; }
        public DateTime BED { get; set; }
    }
}
