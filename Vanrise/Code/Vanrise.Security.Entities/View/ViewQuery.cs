﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class ViewQuery
    {
        public int? ModuleId { get; set; }
        public List<int> ViewTypes { get; set; }
        public string Name { get; set; }
    }
}
