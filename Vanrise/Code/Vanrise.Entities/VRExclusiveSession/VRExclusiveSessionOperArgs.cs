using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRExclusiveSessionTryTakeInput
    {
        public Guid SessionTypeId { get; set; }

        public string TargetId { get; set; }
    }

    public class VRExclusiveSessionTryTakeOutput
    {
        public bool IsSucceeded { get; set; }

        public string FailureMessage { get; set; }
    }

    public class VRExclusiveSessionTryKeepInput
    {
        public Guid SessionTypeId { get; set; }

        public string TargetId { get; set; }
    }

    public class VRExclusiveSessionTryKeepOutput
    {
        public bool IsSucceeded { get; set; }

        public string FailureMessage { get; set; }
    }

    public class VRExclusiveSessionReleaseInput
    {
        public Guid SessionTypeId { get; set; }

        public string TargetId { get; set; }
    }
}
