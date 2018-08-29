using System;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class VRWorkflowInputArgument : BaseProcessInputArgument
    {
        public abstract Dictionary<string, object> ConvertArgumentsToDictionary();
    }
}