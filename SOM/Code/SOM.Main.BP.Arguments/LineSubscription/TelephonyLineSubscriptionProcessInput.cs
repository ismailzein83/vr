using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOM.Main.Entities;

namespace SOM.Main.BP.Arguments
{
    public class TelephonyLineSubscriptionProcessInput : BaseSOMRequestBPInputArg
    {
        public string PhoneNumber { get; set; }
        public string RatePlanId { get; set; }
        public List<string> Services { get; set; }
        public override string GetTitle()
        {
            return String.Format("Telephony Line Subscription: {0}", this.PhoneNumber);
        }
    }

    public class TelephonyLineSubscriptionSomRequestSetting : SOMRequestExtendedSettings
    {
        public string PhoneNumber { get; set; }
        public string RatePlanId { get; set; }
        public List<string> Services { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("238ACF29-254E-494C-881C-F87D981A7830"); }
        }

        public override BaseSOMRequestBPInputArg ConvertToBPInputArgument(ISOMRequestConvertToBPInputArgumentContext context)
        {
            return new TelephonyLineSubscriptionProcessInput
            {
                PhoneNumber = this.PhoneNumber,
                RatePlanId = this.RatePlanId,
                Services = this.Services
            };
        }
    }
}
