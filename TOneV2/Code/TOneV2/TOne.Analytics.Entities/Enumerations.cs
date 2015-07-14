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
        Switch = 0,
        PortIn = 1,
        PortOut = 2,
        CustomerId = 3,
        OurZone = 4,
        OriginatingZoneId = 5,
        SupplierId = 6,
        SupplierZoneId = 7,
        CodeGroup = 8
    }

    public enum TrafficStatisticMeasures
    {
        FirstCDRAttempt = 0,
        LastCDRAttempt = 1,
        Attempts = 2,
        DeliveredAttempts = 3,
        SuccessfulAttempts = 4,
        DurationsInMinutes = 5,
        MaxDurationInMinutes = 6,
        PDDInSeconds = 7,
        UtilizationInSeconds = 8,
        NumberOfCalls = 9,
        DeliveredNumberOfCalls = 10,
        PGAD = 11//,
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

    public enum VolumeReportsOptions { 
    
    TrafficVolumes= 0,
    CompareInOutTraffic=1,
    DestinationTrafficVolumes=2
    }
}