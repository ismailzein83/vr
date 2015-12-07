using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public enum CallTestStatus
    {
        New = 0,
        Initiated = 10,
        InitiationFailedWithRetry = 20,
        PartiallyCompleted = 30,
        GetProgressFailedWithRetry = 40,
        Completed = 50,
        InitiationFailedWithNoRetry = 60,
        GetProgressFailedWithNoRetry = 70
    }
}
