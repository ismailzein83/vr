using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRActionExpressionContext : IVRExpressionContext
    {
        public VRActionExpressionContext(IVRActionContext actionContext)
        {
            if (actionContext == null)
                throw new ArgumentNullException("actionContext");
            _eventPayload = actionContext.EventPayload;
            _numberOfExecutions = actionContext.NumberOfExecutions;
        }

        IVRActionEventPayload _eventPayload;
        public IVRActionEventPayload EventPayload
        {
            get
            {
                return _eventPayload;
            }
        }

        int _numberOfExecutions;
        public int NumberOfExecutions
        {
            get
            {
                return _numberOfExecutions;
            }
        }
    }
}
