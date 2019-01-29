using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.SMSBusinessEntity.BP.Activities
{
    public sealed class CodeActivity1 : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Text { get; set; }

        [RequiredArgument]
        public OutArgument<string> Text2 { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {

            throw new NotImplementedException();
        }
    }
}