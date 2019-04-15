﻿using System;

namespace Retail.RA.Entities
{
    public enum TrafficType
    {
        International = 0,
        Interconnect = 1,
    }

    public enum TrafficDirection
    {
        IN = 0,
        OUT = 1
    }

    public enum CustomerDeliveryStatus
    {
        Delivered = 0,
        Failed = 1,
        Submitted = 2
    }

    public enum VendorDeliveryStatus
    {
        Delivered = 0,
        Failed = 1
    }

    public enum ServiceType
    {
        Voice = 0,
        SMS = 1
    }

    public enum SubscriberType
    {
        Postpaid = 1,
        Prepaid = 2
    }
    public enum Scope
    {
        Total = 0,
        OnNet = 1,
        OffNet = 2,
        International = 3
    }
}
