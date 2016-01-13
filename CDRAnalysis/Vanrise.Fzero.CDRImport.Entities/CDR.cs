using System;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class CDR
    {

        static CDR()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDR),

                "MSISDN", "IMSI", "ConnectDateTime", "Destination", "DurationInSeconds",
                "DisconnectDateTime", "CallClassId", "IsOnNet", "CallType", "SubscriberType", "IMEI", "BTS",
                "Cell", "UpVolume", "DownVolume", "CellLatitude", "CellLongitude", 
                "InTrunkId", "OutTrunkId", "ServiceTypeId", "ServiceVASName", "SwitchId", "ReleaseCode", "MSISDNAreaCode", "DestinationAreaCode"

                );
        }

        public string MSISDN { get; set; }
        public string IMSI { get; set; }
        public DateTime ConnectDateTime { get; set; }
        public string Destination { get; set; }
        public decimal DurationInSeconds { get; set; }
        public DateTime? DisconnectDateTime { get; set; }
        public int? CallClassId { get; set; }
        public bool IsOnNet { get; set; }
        public CallType CallType { get; set; }
        public SubscriberType? SubscriberType { get; set; }
        public string IMEI { get; set; }
        public string BTS { get; set; }
        public string Cell { get; set; }
        public decimal? UpVolume { get; set; }
        public decimal? DownVolume { get; set; }
        public decimal? CellLatitude { get; set; }
        public decimal? CellLongitude { get; set; }
        public int? InTrunkId { get; set; }
        public int? OutTrunkId { get; set; }
        public int? ServiceTypeId { get; set; }
        public string ServiceVASName { get; set; }
        public int? SwitchId { get; set; }
        public string ReleaseCode { get; set; }
        public string MSISDNAreaCode { get; set; }
        public string DestinationAreaCode { get; set; }
    }
}




