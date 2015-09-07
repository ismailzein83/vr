using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public enum CarrierType
    {
        Customer = 0,
        Exchange = 1,
        Supplier = 2
    }

    public enum Change : short
    {
        None = 0,
        Increase = 1,
        Decrease = -1,
        New = 2
    }

    public enum ToDRateType : byte
    {
        Normal = 0,
        OffPeak = 1,
        Weekend = 2,
        Holiday = 4
    }

    public enum ConnectionType 
    {
        VoIP=0,
        TDM=1
    }
    public enum IsEffective
    {
        Y=0,
        N=1
    }
    public enum IsCeiling
    {
        N = 0,
        Y = 1,
        Null=2
    }
}
