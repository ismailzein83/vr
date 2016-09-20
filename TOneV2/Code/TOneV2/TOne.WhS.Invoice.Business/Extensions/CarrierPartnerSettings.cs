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
        public override Guid ConfigId { get { return  new Guid("71F19A9B-0FAD-4370-B390-3F28137DE1EE"); } }
        public bool UseMaskInfo { get; set; }
    }
}
