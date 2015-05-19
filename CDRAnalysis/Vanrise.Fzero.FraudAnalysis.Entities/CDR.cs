using System;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class CDR
    {


        public int Id { get; set; }
        public string MSISDN { get; set; }
        public string IMSI { get; set; }
        public DateTime? ConnectDateTime { get; set; }
        public  string Destination { get; set; }
        public  decimal? DurationInSeconds { get; set; }
        public  DateTime? DisconnectDateTime { get; set; }
        public  Enums.CallClass CallClass { get; set; }
        public  Int16? IsOnNet { get; set; }
        public Enums.CallType? CallType { get; set; }
        public  string SubType { get; set; }
        public  string IMEI { get; set; }
        public  int? BTSId { get; set; }
        public  string CellId { get; set; }
        public  int? SwitchRecordId { get; set; }
        public  decimal? UpVolume { get; set; }
        public  decimal? DownVolume { get; set; }
        public  decimal? CellLatitude { get; set; }
        public  decimal? CellLongitude { get; set; }
        public  string InTrunk { get; set; }
        public  string OutTrunk { get; set; }
        public  int? ServiceType { get; set; }
        public string ServiceVASName { get; set; }

    }
}
