using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRActionAuditChangeInfoResolveChangeInfoContext : IVRActionAuditChangeInfoResolveChangeInfoContext
    {
        public object OldObjectValue { get; set; }

        public object NewObjectValue { get; set; }

        public string ChangeSummary { get; set; }
        public bool NothingChanged { get; set; }
    }
}
