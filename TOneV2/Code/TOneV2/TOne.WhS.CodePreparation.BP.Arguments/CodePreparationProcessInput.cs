using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.BP.Arguments
{
    public class CodePreparationProcessInput : BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return String.Format("CodePreparationProcessInput");
        }
    }
}
