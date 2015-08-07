using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public enum AlertLevel : byte
    {
        Low,
        Medium,
        High,
        Urgent,
        Critical
    }

    public enum AlertProgress : short
    {
        None = 0,
        Positive = 1,
        Negative = -1
    }

    public enum TrafficStatisticGroupKeys
    {
        OurZone=0,
        SupplierZoneId=1,
        CustomerId=2,
        SupplierId=3,
        CodeGroup=4,
        Switch=5,
        GateWayIn=6,
        GateWayOut=7,
        PortIn=8,
        PortOut=9,
    }

    public enum TrafficStatisticMeasures
    {
        FirstCDRAttempt = 0,
        Attempts = 2,
        SuccessfulAttempts = 3,
        FailedAttempts = 4,
        DurationsInMinutes = 5,
        CeiledDuration=6,
        ACD=7,
         PDDInSeconds = 8,
         PGAD = 9,
        MaxDurationInMinutes = 10,
       
        LastCDRAttempt = 11,
        NumberOfCalls = 12,
        DeliveredNumberOfCalls = 13,
        UtilizationInSeconds=14
       //,
        //CeiledDuration = 12,
        //ReleaseSourceAParty = 13
    }
    public enum BillingCDRMeasures
    {
         SwitchName= 0,
         OurZoneName=  1, 
         OriginatingZoneName=2, 
         Attempt= 3, 
         CustomerInfo=  4, 
         CGPN= 5,
         PDD= 6, 
         CDPN= 7,
         CDPNOut=  8, 
         DurationInSeconds=  9,
         ReleaseCode=  10, 
         ReleaseSource=  11, 
         SupplierName= 12, 
         SupplierZoneName=  13, 
    }
    public enum BillingCDROptionMeasures
    {
        Successful = 0,
        Invalid = 1,
        All = 2
    }

    public enum RateType : byte
    {
        Normal = 0,
        OffPeak = 1,
        Weekend = 2
    }
    public enum Change : short
    {
        None = 0,
        Increase = 1,
        Decrease = -1,
        New = 2
    }

    
    public enum VolumeReportsTimePeriod
    {

        None = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }
}