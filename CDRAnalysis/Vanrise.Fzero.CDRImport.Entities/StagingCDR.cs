using System;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class StagingCDR
    {

        static StagingCDR()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(StagingCDR), "CGPN", "CDPN", "SwitchId", "ConnectDateTime","DurationInSeconds","DisconnectDateTime",  "InTrunkId", "OutTrunkId", "CGPNAreaCode", "CDPNAreaCode");
        }

        public string CGPN { get; set; }
        public string CDPN { get; set; }
        public int? SwitchId { get; set; }
        public DateTime ConnectDateTime { get; set; }
        public decimal DurationInSeconds { get; set; }
        public DateTime? DisconnectDateTime { get; set; }
        public int? InTrunkId { get; set; }
        public int? OutTrunkId { get; set; }
        public string CGPNAreaCode { get; set; }
        public string CDPNAreaCode { get; set; }

    }
}
