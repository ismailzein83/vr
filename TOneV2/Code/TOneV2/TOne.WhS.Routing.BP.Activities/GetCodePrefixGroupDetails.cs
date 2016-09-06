using System.Activities;
using System.Collections;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using System.Linq;
using System.Text;

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

            StringBuilder strBuilder = new StringBuilder();
            foreach (CodePrefix codePrefix in codePrefixGroup)
            {
                strBuilder.Append(string.Format("{0}, ", codePrefix.Code));
            }
            strBuilder = strBuilder.Remove(strBuilder.Length - 2, 2);

            this.CodePrefixGroupDescription.Set(context, strBuilder.ToString());
            this.CodePrefixGroupCount.Set(context, codePrefixGroup.Sum(itm => itm.CodeCount));
        }
    }
}