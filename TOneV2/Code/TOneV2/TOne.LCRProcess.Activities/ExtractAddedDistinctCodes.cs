using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.Entities;

namespace TOne.LCRProcess.Activities
{

    public sealed class ExtractAddedDistinctCodes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<string>> DistinctCodesFromCode { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<string>> DistinctCodesFromCodeMatch { get; set; }

        public OutArgument<CodeList> AddedDistinctCodes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var distinctCodesFromCode = this.DistinctCodesFromCode.Get(context);
            var distinctCodesFromCodeMatch = new HashSet<string>(this.DistinctCodesFromCodeMatch.Get(context));
            List<string> addedCodes = new List<string>();
            foreach(var c in distinctCodesFromCode)
            {
                if (!distinctCodesFromCodeMatch.Contains(c))
                    addedCodes.Add(c);
            }
            this.AddedDistinctCodes.Set(context, new CodeList(addedCodes));
        }
    }
}
