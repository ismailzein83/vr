﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.BusinessProcess
{
    public interface IBPWorkflow
    {
        string GetTitle(CreateProcessInput createProcessInput);
    }
}
