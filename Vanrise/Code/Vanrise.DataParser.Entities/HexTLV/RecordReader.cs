﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities.HexTLV
{
    public abstract class RecordReader
    {
        public abstract Guid ConfigId { get; }

    }    
}
