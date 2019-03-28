using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions
{
    public class FilterGroupBPGenericTaskTypeActionFilterCondition : BPGenericTaskTypeActionFilterCondition
    {
        public override Guid ConfigId { get { return new Guid("68DEA8B2-8817-40EE-977D-4EE683CE5092"); } }

        public override bool IsFilterMatch(IBPGenericTaskTypeActionFilterConditionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
