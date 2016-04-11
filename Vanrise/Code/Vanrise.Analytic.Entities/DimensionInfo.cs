﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DimensionInfo<T>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public T Configuration { get; set; }
    }
}
