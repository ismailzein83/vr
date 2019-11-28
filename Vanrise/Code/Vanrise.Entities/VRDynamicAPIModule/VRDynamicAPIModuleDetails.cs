﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDynamicAPIModuleDetails
    {
        public Guid VRDynamicAPIModuleId { get; set; }
        public string Name { get; set; }
        public Guid? DevProjectId { get; set; }
        public string DevProjectName { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedByDescription { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string LastModifiedByDescription { get; set; }

    }
}
