﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelConversion.Entities
{
    public class ExcelConversionSettings
    {
        public List<FieldMapping> FieldMappings { get; set; }

        public List<ListMapping> ListMappings { get; set; }

        public string DateTimeFormat { get; set; }
    }
}
