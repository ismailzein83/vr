using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.BusinessProcess.Business
{
    public class CustomCodeTaskBPSettings : DefaultBPDefinitionExtendedSettings
    {      
        public override bool CanCancelBPInstance(IBPDefinitionCanCancelBPInstanceContext context)
        {
            return true;
        }
    }
}
