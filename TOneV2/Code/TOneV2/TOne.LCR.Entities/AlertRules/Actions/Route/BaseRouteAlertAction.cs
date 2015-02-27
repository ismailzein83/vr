using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public abstract class BaseRouteAlertAction
    {
        public virtual void Execute(RouteDetail route)
        {
        }
    }
}
