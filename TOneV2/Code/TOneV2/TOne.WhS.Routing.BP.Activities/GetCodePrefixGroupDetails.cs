using System.Activities;
using System.Collections;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using System.Linq;
using System.Text;
using System;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class GetCodePrefixGroupDetails : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<CodePrefix>> CodePrefixGroup { get; set; }

        [RequiredArgument]
        public OutArgument<string> CodePrefixGroupDescription { get; set; }

        [RequiredArgument]
        public OutArgument<int> CodePrefixGroupCount { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<CodePrefix> codePrefixGroup = this.CodePrefixGroup.Get(context);

            string codePrefixGroupDescription = String.Join<CodePrefix>(",", codePrefixGroup);

            this.CodePrefixGroupDescription.Set(context, codePrefixGroupDescription);
            this.CodePrefixGroupCount.Set(context, codePrefixGroup.Sum(itm => itm.CodeCount));
        }
    }
}