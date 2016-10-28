using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CleanupSettings : SettingData
    {
        public const string SETTING_TYPE = "TOne_WhS_CleanupSettings";

        public IEnumerable<CleanupTask> Tasks { get; set; }
    }
}
