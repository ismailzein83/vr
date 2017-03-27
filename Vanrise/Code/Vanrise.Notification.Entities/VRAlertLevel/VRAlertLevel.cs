﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public class VRAlertLevel
    {
        public Guid VRAlertLevelId { get; set; }
        public string Name { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }
        public VRAlertLevelSettings Settings { get; set; }
    }
    public class VRAlertLevelSettings
    {
        public Guid StyleDefinitionId { get; set; }
        public int Weight { get; set; }
    }
}
