﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRDynamicAPIModule
    {
        public Guid VRDynamicAPIModuleId { get; set; }
        public string Name { get; set; }
        public Guid? DevProjectId { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public int LastModifiedBy { get; set; }
    }
}
