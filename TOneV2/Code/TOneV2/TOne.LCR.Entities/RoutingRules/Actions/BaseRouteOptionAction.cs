using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class BaseRouteOptionAction
    {
        public virtual bool MatchRoute(IRouteOptionBuildContext context, object actionData)
        {
            return true;
        }

        public virtual RouteOptionActionResult Execute(IRouteOptionBuildContext context, object actionData)
        {
            return null;
        }

        public virtual Type GetActionDataType()
        {
            return null;
        }

        protected RouteOptionActionResult InvalidActionData(string invalidMessage)
        {
            return new RouteOptionActionResult
            {
                IsInvalid = true,
                ErrorMessage = invalidMessage
            };
        }

        public virtual bool IsImportant
        {
            get
            {
                return true;
            }
        }
    }

    public class RouteOptionActionResult
    {
        public bool IsInvalid { get; set; }

        public string ErrorMessage { get; set; }

        public bool NotMatchingTheRule { get; set; }

        public bool RemoveOption { get; set; }

        public bool BlockOption { get; set; }
    }
}
