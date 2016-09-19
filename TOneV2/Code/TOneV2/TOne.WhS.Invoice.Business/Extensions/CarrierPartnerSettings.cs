using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class CarrierPartnerSettings : PartnerSettings
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("71F19A9B-0FAD-4370-B390-3F28137DE1EE"); } }
        public bool UseMaskInfo { get; set; }
    }
}
