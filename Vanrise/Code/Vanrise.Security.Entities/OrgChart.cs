﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class OrgChart
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Hierarchy { get; set; }
    }
}
