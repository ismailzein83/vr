using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.ExcelConversion.Entities
{
    public class ReadConditionsFromFileInput
    {
        public long FileId { get; set; }
        public ExcelConversionSettings ConversionSettings { get; set; }
    }
}
