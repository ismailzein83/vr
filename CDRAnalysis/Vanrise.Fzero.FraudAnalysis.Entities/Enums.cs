﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public enum PeriodEnum
    {
        Hourly = 1,
        Daily = 2
    };

    public enum SuspicionLevel
    {
        Suspicious = 2,
        Highly_Suspicious = 3,
        Fraud = 4
    };


    public enum SuspicionOccuranceStatus
    {
        Open = 0,
        Closed = 10,
        Deleted = 20
    }

    public enum CaseStatus
    {
        Open = 0,
        Pending = 2,
        ClosedFraud = 3,
        ClosedWhiteList = 4
    }
   
}
