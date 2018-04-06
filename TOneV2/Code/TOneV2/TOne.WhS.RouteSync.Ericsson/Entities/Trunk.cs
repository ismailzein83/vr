using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Ericsson
{
    public abstract class BaseTrunk
    {
        public Guid TrunkId { get; set; }

        public string TrunkName { get; set; }

        public TrunkType TrunkType { get; set; }

        public bool IsRouting { get; set; }
    }

    public class InTrunk : BaseTrunk
    {

    }

    public class OutTrunk : BaseTrunk
    {
        public string NationalCountryCode { get; set; }
    }
}