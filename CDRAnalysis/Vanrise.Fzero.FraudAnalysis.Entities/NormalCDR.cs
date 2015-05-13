using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NormalCDR
    {


        public int Id { get; set; }
        public string MSISDN { get; set; }
        public string IMSI { get; set; }
        public DateTime ConnectDateTime { get; set; }
        public  string Destination { get; set; }
        public  decimal DurationInSeconds { get; set; }
        public  DateTime DisconnectDateTime { get; set; }
        public  string CallClass { get; set; }
        public  Int16 IsOnNet { get; set; }
        public  int CallType { get; set; }
        public  string SubType { get; set; }
        public  string IMEI { get; set; }
        public  int BTSId { get; set; }
        public  string CellId { get; set; }
        public  int SwitchRecordId { get; set; }
        public  decimal UpVolume { get; set; }
        public  decimal DownVolume { get; set; }
        public  decimal CellLatitude { get; set; }
        public  decimal CellLongitude { get; set; }
        public  string InTrunk { get; set; }
        public  string OutTrunk { get; set; }
        public  int ServiceType { get; set; }
        public string ServiceVASName { get; set; }

    }
}
