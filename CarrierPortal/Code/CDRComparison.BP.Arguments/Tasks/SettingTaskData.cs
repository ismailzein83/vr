using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace CDRComparison.BP.Arguments
{
    public class SettingTaskData : BPTaskData
    {
        public string TableKey { get; set; }
        public int? PartnerCDRSourceConfigId { get; set; }
    }
}
