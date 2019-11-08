using System;

namespace RecordAnalysis.Entities
{
    public enum C4CommandType
    {
        BlockIP = 1,
        BlockOriginationNumberOnMSC = 2,
        BlockDestinationNumberOnMSC = 3,
        BlockInterconnection = 4
    }

    public enum C5CommandType
    {
        BlockNumber = 1
    }
}