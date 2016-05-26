using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class ResultProcessingHandler<T>
    {
        public ExcelExportHandler<T> ExportExcelHandler { get; set; }
    }
}
