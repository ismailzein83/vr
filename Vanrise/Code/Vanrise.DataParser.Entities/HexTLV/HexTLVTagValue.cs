﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class HexTLVTagValue
    {
        public string Tag { get; set; }
        public int Length { get; set; }
        public byte[] Value { get; set; }
    }
}
