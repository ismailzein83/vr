using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Data;
namespace TOne.LCRProcess.Activities
{

    public sealed class GetCodePrefixGroup : CodeActivity
    {
        /// <summary>
        /// Distinct Code Prefix Groups From RootCodePrefix
        /// </summary>
        [RequiredArgument]
        public OutArgument<List<string>> CodePrefixGroups { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            ICodePrefixGroupDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodePrefixGroupDataManager>();

            var codePrefixGroups = dataManager.GetCodePrefixGroups();
            this.CodePrefixGroups.Set(context, codePrefixGroups);
        }
    }
}
