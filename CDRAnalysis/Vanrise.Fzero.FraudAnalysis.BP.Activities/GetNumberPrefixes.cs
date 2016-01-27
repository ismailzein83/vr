using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Fzero.FraudAnalysis.Data;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    public sealed class GetNumberPrefixes : CodeActivity
    {
        public OutArgument<List<string>> NumberPrefixes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            List<string> prefixes = new List<string>();
            prefixes.Add("00961");
            this.NumberPrefixes.Set(context, prefixes);

        }
    }
}
