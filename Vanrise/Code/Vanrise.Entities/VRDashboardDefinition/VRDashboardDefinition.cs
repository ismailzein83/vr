﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDashboardDefinition
    {
        public Guid VRDashboardDefinitionId { get; set; }

        public string Name { get; set; }

        public VRDashboardSettings Settings { get; set; }
    }
}