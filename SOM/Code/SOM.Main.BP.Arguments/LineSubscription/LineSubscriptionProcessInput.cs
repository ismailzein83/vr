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

        public string SwitchId { get; set; }

        public string CabinetId { get; set; }

        public string DPId { get; set; }

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

        public string SwitchId { get; set; }

        public string CabinetId { get; set; }

        public string DPId { get; set; }

        public override BaseSOMRequestBPInputArg ConvertToBPInputArgument(ISOMRequestConvertToBPInputArgumentContext context)
        {
            return new LineSubscriptionProcessInput
            {
                PhoneNumber = this.PhoneNumber,
                SwitchId = this.SwitchId,
                CabinetId = this.CabinetId,
                DPId = this.DPId
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
