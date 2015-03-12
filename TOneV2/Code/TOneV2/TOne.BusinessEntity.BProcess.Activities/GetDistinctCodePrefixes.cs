using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.BusinessEntity.Data;

namespace TOne.BusinessEntity.BProcess.Activities
{

    public sealed class GetDistinctCodePrefixes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> CodePrefixLength { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveTime { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public OutArgument<List<string>> CodePrefixes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            ICodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ICodeDataManager>();
            var codePrefixes = dataManager.GetDistinctCodePrefixes(this.CodePrefixLength.Get(context), this.EffectiveTime.Get(context), this.IsFuture.Get(context));
            this.CodePrefixes.Set(context, codePrefixes);
        }
    }
}
