﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.Entities
{
   public class VRAlertLevelInfoFilter
    {
        public List<IVRALertFilter> Filters { get; set; }
    }
}
