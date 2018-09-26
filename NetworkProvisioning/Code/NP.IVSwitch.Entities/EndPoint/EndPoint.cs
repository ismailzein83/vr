using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public enum RtpMode { AdvancedProxying = 1, PassthruProxying = 2, NoProxying = 3 }

    public enum UserType
    {
        VendroTermRoute = 2,
        ACL = 3,
        SIP = 4
    }
    public class EndPoint
    {
        public int EndPointId { get; set; } // user_id

        public UserType EndPointType { get; set; }
        public Int16 CliRouting { get; set; }
        public int DstRouting { get; set; }
        public int? RouteTableId { get; set; }
        public String Host { get; set; }
        public Int16 DomainId { get; set; }
        public String TechPrefix { get; set; }
        public int AccountId { get; set; }
        public String Description { get; set; }
        public int? TransRuleId { get; set; }
        public State CurrentState { get; set; }
        public int ChannelsLimit { get; set; }
        public int ChannelsActive { get; set; }
        public String LogAlias { get; set; }

        public Trace EnableTrace { get; set; }
        public int CodecProfileId { get; set; }
        public RtpMode RtpMode { get; set; }
        public int MaxCallDuration { get; set; }
        public String TracePattern { get; set; }
        public String TraceId { get; set; }
        public String SipLogin { get; set; } //SIP
        public String SipPassword { get; set; }  //SIP 

        //   svc_handler_id smallint, ??
        public bool RouteTableBasedRule { get; set; }
    }
}
