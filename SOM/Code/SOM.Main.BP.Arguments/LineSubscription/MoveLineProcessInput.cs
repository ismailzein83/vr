using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM.Main.BP.Arguments
{
    public class MoveLineProcessInput : BaseSOMRequestBPInputArg
    {
        public string PhoneNumber { get; set; }
        public string SwitchId { get; set; }
        public string CabinetId { get; set; }
        public string NewSwitchId { get; set; }
        public string NewCabinetId { get; set; }

        public override string GetTitle()
        {
            return String.Format("Move Line: {0}", this.PhoneNumber);
        }
    }

    public class MoveLineRequest : SOMRequestExtendedSettings
    {
        internal static Guid S_ConfigId = new Guid("900B0866-A871-4974-ACA9-1D3FB16DCB45");

        public override Guid ConfigId
        {
            get { return S_ConfigId; }
        }

        public string PhoneNumber { get; set; }
        public string SwitchId { get; set; }
        public string CabinetId { get; set; }
        public string NewSwitchId { get; set; }
        public string NewCabinetId { get; set; }

        public override BaseSOMRequestBPInputArg ConvertToBPInputArgument(ISOMRequestConvertToBPInputArgumentContext context)
        {
            return new MoveLineProcessInput
            {
                PhoneNumber = this.PhoneNumber,
                SwitchId = this.SwitchId,
                CabinetId = this.CabinetId,
                NewCabinetId = this.NewCabinetId,
                NewSwitchId = this.NewSwitchId
            };
        }
    }

}
