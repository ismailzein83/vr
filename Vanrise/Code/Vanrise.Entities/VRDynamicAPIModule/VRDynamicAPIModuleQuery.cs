﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDynamicAPIModuleQuery
    {
        public string Name { get; set; }
        public List<Guid> DevProjectIds { get; set; }
    }
}
