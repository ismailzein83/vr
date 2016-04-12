using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.ExcelConversion.Entities
{
    public class ExcelPageQuery
    {
        public long FileId { get; set; }

        public int SheetIndex { get; set; }

        public int From { get; set; }

        public int To { get; set; }
    }
}
