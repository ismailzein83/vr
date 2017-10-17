using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.ExcelConversion.Entities
{
    public interface IGetFieldValueContext
    {
        Aspose.Cells.Workbook Workbook { get; }

        Aspose.Cells.Worksheet Sheet { get; }

        Aspose.Cells.Row Row { get; }

        ExcelConversionExtendedSettings ExtendedSettings { get; set; }

        Dictionary<string, Object> FieldValueByFieldName { get; }
    }
}
