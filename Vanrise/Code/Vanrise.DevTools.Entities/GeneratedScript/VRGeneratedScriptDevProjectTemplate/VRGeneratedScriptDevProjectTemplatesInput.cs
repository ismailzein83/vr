﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DevTools.Entities
{
    public class VRGeneratedScriptDevProjectTemplatesInput
    {
        public Guid ConnectionId { get; set; }
        public Guid DevProjectId { get; set; }
        public List<string> TableNames { get; set; }
    }
}
