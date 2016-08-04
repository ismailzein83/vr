using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Arguments
{
    public class CreateActionProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return "Create Action Process";
        }
        public CreateVRActionInput CreateVRActionInput { get; set; }
    }
}
