using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Arguments
{
    public class StoreMainCDRsInDBProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public int SwitchID { get; set; }

        public override string GetTitle()
        {
            return String.Format("Store Main CDRs for Switch {0}", SwitchID);
        }
    }
}
