using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public interface IGetConcatenatedPartTextContext
    {
        Aspose.Cells.Workbook Workbook { get; }

        Aspose.Cells.Worksheet Sheet { get; }

        Aspose.Cells.Row Row { get; }
    }
}
