﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.ExcelConversion.Entities
{
    public class ExcelWorksheet
    {
        public string Name { get; set; }

        public int NumberOfColumns { get; set; }

        public List<ExcelRow> Rows { get; set; }

        public List<MergedCell> MergedCells { get; set; }
    }
}
