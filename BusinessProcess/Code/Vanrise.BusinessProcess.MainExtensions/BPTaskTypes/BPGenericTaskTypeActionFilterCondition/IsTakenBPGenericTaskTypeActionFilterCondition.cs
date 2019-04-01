using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Security.Business;

namespace Vanrise.BusinessProcess.MainExtensions.BPTaskTypes
{
    public class IsTakenBPGenericTaskTypeActionFilterCondition : BPGenericTaskTypeActionFilterCondition
    {
        public override Guid ConfigId { get { return new Guid("9B435BE4-43CD-4942-B5D3-95C8927D6885"); } }
        public bool TakenByLoggedInUser { get; set; }
        public override bool IsFilterMatch(IBPGenericTaskTypeActionFilterConditionContext context)
        {
            if (TakenByLoggedInUser)
            {
                if (!context.Task.TakenBy.HasValue)
                    return false;
                if (context.Task.TakenBy.HasValue)
                {
                    int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
                    return loggedInUserId == context.Task.TakenBy.Value;
                }
            }
            else
            {
                return !context.Task.TakenBy.HasValue;
            }
            return false;
        }
    }
}
