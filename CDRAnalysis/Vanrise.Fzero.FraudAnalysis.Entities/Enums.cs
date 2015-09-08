using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public enum SuspicionLevel
    {
        Suspicious = 2,
        HighlySuspicious = 3,
        Fraud = 4,
    }

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

    public enum MyCallClassEnum
    {
        ZAINIQ = 1,
        VAS = 2,
        INV = 3,
        INT = 4,
        KOREKTEL = 5,
        ASIACELL = 6
    }

    public enum MyCallTypeEnum
    {
        OutgoingVoiceCall = 1,
        IncomingVoiceCall = 2,
        CallForward = 3,
        IncomingSMS = 4,
        OutgoingSMS = 5,
        RoamingCallForward = 6
    }

    public enum MySubTypeEnum
    {
        Prepaid = 1,
        Postpaid = 2
    }
}
