using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.Entities
{
    public enum RatePlanType { Normal, Gold, ADSL, ST_Employee, ISDN }
    public class RatePlan
    {
        //Used in CRM
        public string Id { get; set; }
        public string Name { get; set; }

        //Not used
        public string Description { get; set; }
        public RatePlanType Type { get; set; }

    }
}
