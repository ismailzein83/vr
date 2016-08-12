using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;
using Vanrise.BusinessProcess;

namespace Vanrise.BEBridge.BP.Activities
{
    public sealed class ConvertSourceToTargetBEs : CodeActivity
    {
        public InArgument<SourceBEBatch> SourceBEBatch { get; set; }
        public InArgument<TargetBEConvertor> TargetConverter { get; set; }
        public InOutArgument<List<ITargetBE>> TargetBEs { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            TargetBEConvertorConvertSourceBEsContext targetBEConvertorContext = new TargetBEConvertorConvertSourceBEsContext();
            TargetConverter.Get(context).ConvertSourceBEs(targetBEConvertorContext);
            TargetBEs.Set(context, targetBEConvertorContext.TargetBEs);
        }

        private class TargetBEConvertorConvertSourceBEsContext : ITargetBEConvertorConvertSourceBEsContext
        {
            public SourceBEBatch SourceBEBatch
            {
                set;
                get;
            }

            public List<ITargetBE> TargetBEs
            {
                set;
                get;
            }
        }

    }

}
