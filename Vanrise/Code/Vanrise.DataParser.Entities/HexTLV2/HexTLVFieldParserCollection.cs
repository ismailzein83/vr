﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class HexTLVFieldParserCollection
    {
        public Dictionary<string, HexTLVFieldParser> FieldParsersByTag { get; set; }
    }
}
