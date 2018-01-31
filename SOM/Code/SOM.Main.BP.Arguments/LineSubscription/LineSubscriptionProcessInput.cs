using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.BP.Arguments
{
    public class LineSubscriptionProcessInput : BaseSOMRequestBPInputArg
    {
        public string PhoneNumber { get; set; }

        public string PrimaryPort { get; set; }

        public string SecondaryPort { get; set; }

        public string DPId { get; set; }
        public string RatePlanId { get; set; }
        public List<string> Services { get; set; }
        public override string GetTitle()
        {
            return String.Format("Line Subscription: {0}", this.PhoneNumber);
        }
    }

    public class LineSubscriptionRequest : SOMRequestExtendedSettings
    {
        internal static Guid S_ConfigId = new Guid("31E1AFF4-D7F2-4C30-BDF1-BC7D965E8B20");

        public override Guid ConfigId
        {
            get { return S_ConfigId; }
        }

        public string PhoneNumber { get; set; }
        public string PrimaryPort { get; set; }
        public string SecondaryPort { get; set; }
        public string DPId { get; set; }
        public string RatePlanId { get; set; }
        public List<string> Services { get; set; }

        public override BaseSOMRequestBPInputArg ConvertToBPInputArgument(ISOMRequestConvertToBPInputArgumentContext context)
        {
            return new LineSubscriptionProcessInput
            {
                PhoneNumber = this.PhoneNumber,
                PrimaryPort = this.PrimaryPort,
                SecondaryPort = this.SecondaryPort,
                DPId = this.DPId,
                RatePlanId = this.RatePlanId,
                Services = this.Services
            };
        }
    }

    public class CreateLineSubscriptionInput
    {
        public Guid? SOMRequestId { get; set; }

        public string EntityId { get; set; }

        public string RequestTitle { get; set; }

        public LineSubscriptionRequest RequestDetails { get; set; }
    }
}
