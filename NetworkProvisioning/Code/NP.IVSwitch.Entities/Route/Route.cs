using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{

    public enum Trace { Disabled = 0, Enabled = 1  }
   public enum TransportMode { UDP = 1, TCP = 2, TLS =3 }

    public class Route
    {
        public int RouteId {get;set;}
        public int AccountId{get;set;}
        public String Description {get;set;}
        public int GroupId {get;set;}        
        public int TariffId {get;set;}
        public  String LogAlias {get;set;}
        public int CodecProfileId { get; set; }
        public int TransRuleId { get; set; }
        public State CurrentState { get; set; }
        public int ChannelsLimit {get;set;}
        public DateTime WakeUpTime {get;set;}
        public String Host{get;set;}
        public String Port { get; set; }
  
        public TransportMode TransportModeId { get; set; }
        public int ConnectionTimeOut {get;set;}
          
        // Custom Route Params ???
     
    }
}
