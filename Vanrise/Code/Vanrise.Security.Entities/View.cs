﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public enum ViewType  {  System = 0, Dynamic = 1 }

    public class View
    {
        public int ViewId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public int ModuleId { get; set; }

        public Dictionary<string, List<string>> RequiredPermissions { get; set; }

        public AudienceWrapper Audience { get; set; }
        public ViewType Type { get; set; }
    }

    public class AudienceWrapper
    {
        public List<int> Users { get; set; }
        
        public List<int> Groups { get; set; }
    }
}
