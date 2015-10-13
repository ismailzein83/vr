﻿using System;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class StagingCDR
    {

        static StagingCDR()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(StagingCDR), "CGPN", "CDPN", "SwitchID", "InTrunkSymbol", "OutTrunkSymbol", "DurationInSeconds", 
"DisconnectDateTime", "ConnectDateTime", "InTrunkId", "OutTrunkId");
        }

        public string CGPN { get; set; }
        public string CDPN { get; set; }
        public int? SwitchID { get; set; }
        public string InTrunkSymbol { get; set; }
        public string OutTrunkSymbol { get; set; }
        public decimal? DurationInSeconds { get; set; }
        public DateTime? DisconnectDateTime { get; set; }
        public DateTime? ConnectDateTime { get; set; }
        public int? InTrunkId { get; set; }
        public int? OutTrunkId { get; set; }

    }
}
