using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Common.Business;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public sealed class GetNumberPrefixes : CodeActivity
    {
        public OutArgument<List<string>> NumberPrefixes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<string> prefixes = new List<string>();
            NumberPrefixesManager manager = new NumberPrefixesManager();
            prefixes = manager.GetPrefixes().Select(x => x.Prefix).ToList(); 
            this.NumberPrefixes.Set(context, prefixes);

        }
    }
}
