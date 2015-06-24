using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.CDRImport.BP.Arguments
{
    public class SaveCDRToDBProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return "Save CDR To Database";
        }
    }
}
