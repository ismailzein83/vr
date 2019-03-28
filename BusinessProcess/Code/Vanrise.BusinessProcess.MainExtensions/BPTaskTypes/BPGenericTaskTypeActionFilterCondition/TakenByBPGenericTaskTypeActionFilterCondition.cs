using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions
{
    public class TakenByBPGenericTaskTypeActionFilterCondition : BPGenericTaskTypeActionFilterCondition
    {
        public override Guid ConfigId { get { return new Guid("9B435BE4-43CD-4942-B5D3-95C8927D6885"); } }

        public override bool IsFilterMatch(IBPGenericTaskTypeActionFilterConditionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
