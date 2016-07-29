using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Business;

namespace TOne.WhS.RouteSync.BP.Activities
{

    public sealed class GetSwitches : CodeActivity
    {        
        [RequiredArgument]
        public InArgument<List<string>> SwitchIds { get; set; }

        [RequiredArgument]
        public OutArgument<List<SwitchInfo>> Switches { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            var switchInfoGetter = new ConfigurationManager().GetSwitchInfoGetter();
            if (switchInfoGetter == null)
                throw new NullReferenceException("switchInfoGetter");
            List<SwitchInfo> switches = this.SwitchIds.Get(context).Select(switchId => switchInfoGetter.GetSwitchInfo(new SwitchInfoGetterContext { SwitchId = switchId })).ToList();
        }
    }
}
