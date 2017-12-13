using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class SessionLockSettings : SettingData
    {
        public int TimeOutInSeconds { get; set; }
        public int HeartbeatIntervalInSeconds { get; set; }
    }
}
