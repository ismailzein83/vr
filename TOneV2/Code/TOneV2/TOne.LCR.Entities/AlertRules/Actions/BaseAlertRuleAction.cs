﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public abstract class BaseAlertRuleAction
    {
        public virtual void Execute(RouteDetail route)
        {

        }
    }
}
