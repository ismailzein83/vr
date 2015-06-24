using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Arguments
{
    public class RawCDRsProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public int SwitchID { get; set; }

        public Guid CacheManagerId { get; set; }


        public override string GetTitle()
        {
            return String.Format("Raw CDRs Process for Switch {0}", SwitchID);
        }
    }
}
