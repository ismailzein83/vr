using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public interface IVRAlertLevelFilter
    {
        bool IsMatched(IVRAlertLevelFilterContext context);
    }
        public interface IVRAlertLevelFilterContext
        {
            VRAlertLevel VRAlertLevel { get; }
        }
    
}
