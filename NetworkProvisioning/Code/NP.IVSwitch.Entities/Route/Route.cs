using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{

    public enum Trace { Disabled = 0, Enabled = 1  }
    public class Route
    {
        public int RouteId {get;set;}
        public int AccountId{get;set;}
        public String Description {get;set;}
        public int GroupId {get;set;}        
        public int TariffID {get;set;}
        public  String LogAlias {get;set;}
        public int CodecProfileId { get; set; }
        public int TransRuleId { get; set; }
        public State CurrentState { get; set; }
        public int ChannelsLimit {get;set;}
        public DateTime WakeUpTime {get;set;}
        public Trace EnableTrace { get; set; }
        public String Host{get;set;}
        public String Port { get; set; } 
        public int TransportModeId {get;set;}
        public int ConnectionTimeOut {get;set;}
        public int PScore {get;set;}
         
        // Custom Route Params ???
     
    }
}
