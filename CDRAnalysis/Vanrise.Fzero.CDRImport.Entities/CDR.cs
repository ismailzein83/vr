﻿using System;

namespace Vanrise.Fzero.CDRImport.Entities
{
    public class CDR
    {

        static CDR()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CDR),

                "MSISDN", "IMSI", "ConnectDateTime", "Destination", "DurationInSeconds",
                "DisconnectDateTime", "CallClass", "IsOnNet", "CallType", "SubType", "IMEI", "BTSId",
                "CellId", "UpVolume", "DownVolume", "CellLatitude", "CellLongitude", "InTrunkSymbol", "OutTrunkSymbol",
                "InTrunkId", "OutTrunkId", "ServiceType", "ServiceVASName", "SwitchId", "ReleaseCode", "MSISDNAreaCode", "DestinationAreaCode"

                );
        }

        public string MSISDN { get; set; }
        public string IMSI { get; set; }
        public DateTime? ConnectDateTime { get; set; }
        public string Destination { get; set; }
        public decimal? DurationInSeconds { get; set; }
        public DateTime? DisconnectDateTime { get; set; }
        public string CallClass { get; set; }
        public Int16? IsOnNet { get; set; }
        public CallType? CallType { get; set; }
        public string SubType { get; set; }
        public string IMEI { get; set; }
        public int? BTSId { get; set; }
        public string CellId { get; set; }
        public decimal? UpVolume { get; set; }
        public decimal? DownVolume { get; set; }
        public decimal? CellLatitude { get; set; }
        public decimal? CellLongitude { get; set; }
        public string InTrunkSymbol { get; set; }
        public string OutTrunkSymbol { get; set; }
        public int? InTrunkId { get; set; }
        public int? OutTrunkId { get; set; }
        public int? ServiceType { get; set; }
        public string ServiceVASName { get; set; }
        public int? SwitchId { get; set; }
        public string ReleaseCode { get; set; }
        public string MSISDNAreaCode { get; set; }
        public string DestinationAreaCode { get; set; }
    }
}




