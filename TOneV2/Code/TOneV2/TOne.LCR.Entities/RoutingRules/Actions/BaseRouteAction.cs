using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class BaseRouteAction
    {
        public virtual RouteActionResult Execute(IRouteBuildContext context, object actionData, RouteRule rule)
        {
            return null;
        }

        public virtual Type GetActionDataType()
        {
            return null;
        }

        protected RouteActionResult InvalidActionData(string invalidMessage)
        {
            return new RouteActionResult
            {
                IsInvalid = true,
                ErrorMessage = invalidMessage
            };
        }
    }

    public class RouteActionResult
    {
        public bool IsInvalid { get; set; }

        public string ErrorMessage { get; set; }

        public Type NextActionType { get; set; }

        public object NextActionData { get; set; }
    }
}
