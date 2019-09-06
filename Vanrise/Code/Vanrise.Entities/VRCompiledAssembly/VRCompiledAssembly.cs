﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRCompiledAssembly
    {
        public Guid VRCompiledAssemblyId { get; set; }

        public string Name { get; set; }

        public Guid DevProjectId { get; set; }

        public byte[] AssemblyContent { get; set; }
    }
}
