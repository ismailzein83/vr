﻿using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.BP.Arguments
{
    public class AssignStrategyCasesProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return String.Format("Assign Strategy Execution Cases ({0:dd-MMM-yy HH:mm})", DateTime.Now);
        }

    }
}
