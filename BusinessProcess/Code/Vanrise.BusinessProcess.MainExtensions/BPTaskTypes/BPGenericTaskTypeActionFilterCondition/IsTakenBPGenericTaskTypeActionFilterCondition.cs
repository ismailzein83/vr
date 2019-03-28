using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.BPTaskTypes
{
    public class IsTakenBPGenericTaskTypeActionFilterCondition : BPGenericTaskTypeActionFilterCondition
    {
        public override Guid ConfigId { get { return new Guid("9B435BE4-43CD-4942-B5D3-95C8927D6885"); } }
        public bool IsTaken { get; set; }
        public override bool IsFilterMatch(IBPGenericTaskTypeActionFilterConditionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
