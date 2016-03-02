//using System.Activities;
//using System.Collections.Generic;
//using Vanrise.Fzero.FraudAnalysis.Business;

//namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
//{

//    public sealed class GetNumberPrefixes : CodeActivity
//    {
//        public OutArgument<List<string>> NumberPrefixes { get; set; }

//        protected override void Execute(CodeActivityContext context)
//        {
//            List<string> prefixes = new List<string>();
//            NumberPrefixesManager manager = new NumberPrefixesManager();
//            prefixes = manager.GetLeafPrefixes(); 
//            this.NumberPrefixes.Set(context, prefixes);

//        }
//    }
//}
