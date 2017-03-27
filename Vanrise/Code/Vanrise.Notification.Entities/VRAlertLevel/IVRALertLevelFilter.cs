using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.Entities
{
    public interface IVRALertFilter
    {
        bool IsMatched(IVRALertLevelFilterContext context);
    }
        public interface IVRALertLevelFilterContext
        {
            VRAlertLevel VRAlertLevel { get; }
        }
    
}
