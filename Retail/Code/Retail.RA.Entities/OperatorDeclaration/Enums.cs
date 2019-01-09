using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Business
{
    public enum TrafficType
    {
        OnNet = 0,
        OffNet = 1,
        International = 2
    }

    public enum TrafficDirection
    {
        IN = 0,
        OUT = 1
    }

    public enum CustomerDeliveryStatus
    {
        Delivered = 0,
        NotDelivered = 1
    }

    public enum VendorDeliveryStatus
    {
        Delivered = 0,
        NotDelivered = 1
    }

    public enum ServiceType
    {
        Voice = 0,
        SMS = 1
    }

}
