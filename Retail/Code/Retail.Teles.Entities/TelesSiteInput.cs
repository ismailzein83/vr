using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Entities
{
    public class TelesSiteInput
    {
        public string TelesEnterpriseId { get; set; }
        public Guid VRConnectionId { get; set; }
        public string CentrexFeatSet { get; set; }
        public string TemplateName { get; set; }
        public Site Site { get; set; }
              
    }
    public class Site
    {
        public string name { get; set; }
        public string description { get; set; }
        public int maxCalls { get; set; }
        public int maxCallsPerUser { get; set; }
        public int maxRegistrations { get; set; }
        public int maxRegsPerUser { get; set; }
        public int maxSubsPerUser { get; set; }
        public int maxBusinessTrunkCalls { get; set; }
        public int maxUsers { get; set; }
        public string ringBackUri { get; set; }
        public bool registrarEnabled { get; set; }
        public bool registrarAuthRequired { get; set; }
        public bool presenceEnabled { get; set; }

    }
}
