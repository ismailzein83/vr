using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOM.Main.Entities;

namespace SOM.Main.BP.Arguments
{
    public class LineSubscriptionTerminationProcessInput : BaseSOMRequestBPInputArg
    {
        public string PhoneNumber { get; set; }
        public string SwitchId { get; set; }
        public string CabinetId { get; set; }
        public override string GetTitle()
        {
            return String.Format("Terminate Line Subscription: {0}", this.PhoneNumber);
        }
    }

    public class LineSubscriptionTerminationProcessRequest : SOMRequestExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("461B7474-9B19-4B90-AEAB-63BA37245E53"); }
        }
        public string PhoneNumber { get; set; }
        public string SwitchId { get; set; }
        public string CabinetId { get; set; }
        public override BaseSOMRequestBPInputArg ConvertToBPInputArgument(ISOMRequestConvertToBPInputArgumentContext context)
        {
            return new LineSubscriptionTerminationProcessInput
            {
                PhoneNumber = this.PhoneNumber,
                CabinetId = this.CabinetId,
                SwitchId = this.SwitchId
            };
        }
    }
}
