using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.CLITester.Entities
{
    public enum CallTestStatus
    {
        [Description("New")]
        New = 0,
        
        [Description("Initiated")]
        Initiated = 10,

        [Description("Initiation Failed With Retry")]
        InitiationFailedWithRetry = 20,

        [Description("Partially Completed")]
        PartiallyCompleted = 30,

        [Description("Call Failed")]
        GetProgressFailedWithRetry = 40,

        [Description("Completed")]
        Completed = 50,

        [Description("Initiation Failed With No Retry")]
        InitiationFailedWithNoRetry = 60,

        [Description("Call Failed")]
        GetProgressFailedWithNoRetry = 70
    }
}
