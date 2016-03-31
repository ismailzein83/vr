using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public class ConvertedExcel
    {
        public ConvertedExcelFieldsByName Fields { get; set; }

        public ConvertedExcelListsByName Lists { get; set; }
    }
}
