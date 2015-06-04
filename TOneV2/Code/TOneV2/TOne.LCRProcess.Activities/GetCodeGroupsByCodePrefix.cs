using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.BusinessEntity.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.LCRProcess.Activities
{

    public sealed class GetCodeGroupsByCodePrefix : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> CodePrefix { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<string, CodeGroupInfo>> CodeGroupInfos { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            CodeManager codeManager = new CodeManager();
            Dictionary<string, CodeGroupInfo> codeGroups = codeManager.GetCodeGroupsByCodePrefix(this.CodePrefix.Get(context));
            this.CodeGroupInfos.Set(context, codeGroups);
        }
    }
}
