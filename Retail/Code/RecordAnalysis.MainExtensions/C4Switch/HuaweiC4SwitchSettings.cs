using System;
using RecordAnalysis.Entities;

namespace RecordAnalysis.MainExtensions.C4Switch
{
    public class HuaweiC4SwitchSettings : C4SwitchSettings
    {
        public override Guid ConfigId { get { return new Guid("D5D7B8A3-3004-46FC-A22D-B060B9BD40D2"); } }

        public string IP { get; set; }
    }
}
