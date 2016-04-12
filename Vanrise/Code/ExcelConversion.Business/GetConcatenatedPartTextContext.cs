﻿using ExcelConversion.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Business
{
    public class GetConcatenatedPartTextContext : IGetConcatenatedPartTextContext
    {
        public Aspose.Cells.Workbook Workbook { get; set; }

        public Aspose.Cells.Worksheet Sheet { get; set; }

        public Aspose.Cells.Row Row { get; set; }
    }
}
