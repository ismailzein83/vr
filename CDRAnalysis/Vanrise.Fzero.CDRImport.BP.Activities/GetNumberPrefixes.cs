using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Business;

namespace Vanrise.Fzero.CDRImport.BP.Activities
{
    public sealed class GetNumberPrefixes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> FromTime { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> ToTime { get; set; }

        public OutArgument<IEnumerable<string>> NumberPrefixes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            CDRManager cdrManager = new CDRManager();
            this.NumberPrefixes.Set(context, cdrManager.GetNumberPrefixes(this.FromTime.Get(context), this.ToTime.Get(context)));
        }
    }
}
