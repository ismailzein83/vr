﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules
{
    public interface IRuleExecutionContext
    {
        IVRRule Rule { get; set; }
    }
}
