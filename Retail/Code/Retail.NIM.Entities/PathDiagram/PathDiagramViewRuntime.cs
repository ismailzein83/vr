﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class PathDiagramViewRuntime
    {
        public long PathId { get; set; }
        public List<PathDiagramNode> PathDiagramData { get; set; }
    }
}
