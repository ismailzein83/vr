using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;

namespace XBooster.PriceListConversion.Entities
{
    public class InputPriceListSettings
    {
        public int FileId { get; set; }
        public ExcelConversionSettings ExcelConversionSettings { get; set; }
    }
}
