﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class SystemAction
    {
        public int SystemActionId { get; set; }

        public string Name { get; set; }

        public string RequiredPermissions { get; set; }
    }
}
