using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Arguments
{
    public class StoreCDRsInDBProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public int SwitchID { get; set; }


        public override string GetTitle()
        {
            return String.Format("Store CDRs In DB for Switch {0}", SwitchID);
        }
    }
}
