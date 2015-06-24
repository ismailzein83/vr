using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.CDRProcess.Arguments
{
    public class CDRImportProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public int SwitchID { get; set; }

        public override string GetTitle()
        {
            throw new NotImplementedException();
        }
    }
}
