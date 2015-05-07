using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NormalCDR
    {


        public int id { get; set; }
        public string mSISDN { get; set; }
        public string iMSI { get; set; }
        public DateTime connectDateTime { get; set; }
        public  string destination { get; set; }
        public  decimal durationInSeconds { get; set; }
        public  DateTime disconnectDateTime { get; set; }
        public  string callClass { get; set; }
        public  Int16 isOnNet { get; set; }
        public  int callType { get; set; }
        public  string subType { get; set; }
        public  string iMEI { get; set; }
        public  int bTSId { get; set; }
        public  string cellId { get; set; }
        public  int switchRecordId { get; set; }
        public  decimal upVolume { get; set; }
        public  decimal downVolume { get; set; }
        public  decimal cellLatitude { get; set; }
        public  decimal cellLongitude { get; set; }
        public  string inTrunk { get; set; }
        public  string outTrunk { get; set; }
        public  int serviceType { get; set; }
        public string serviceVASName { get; set; }

    }
}
