﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
namespace Vanrise.Data.RDB
{
    public class RDBTableColumnDefinition
    {
        public string DBColumnName { get; set; }

        public RDBDataType DataType { get; set; }

        public int? Size { get; set; }

        public int? Precision { get; set; }
    }
}
