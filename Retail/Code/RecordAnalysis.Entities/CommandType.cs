using System;

namespace RecordAnalysis.Entities
{
    public enum CommandType
    {
        BlockIP = 1,
        BlockOriginationNumberOnMSC = 2,
        BlockDestinationNumberOnMSC = 3,
        BlockInterconnection = 4,
        BlockNumber = 5
    }
}