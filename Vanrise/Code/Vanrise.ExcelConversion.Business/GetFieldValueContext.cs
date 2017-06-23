using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;

namespace Vanrise.ExcelConversion.Business
{
    public class GetFieldValueContext : IGetFieldValueContext
    {
        public Aspose.Cells.Workbook Workbook { get; set; }

        public Aspose.Cells.Worksheet Sheet { get; set; }

        public Aspose.Cells.Row Row { get; set; }

        public Dictionary<string, object> FieldValueByFieldName { get; set; }
    }
}
