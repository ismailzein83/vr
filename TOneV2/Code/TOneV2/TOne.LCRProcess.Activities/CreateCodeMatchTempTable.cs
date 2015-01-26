using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class CreateCodeMatchTempTable : CodeActivity
    {
        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            ICodeMatchDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            dataManager.CreateTempTable(this.IsFuture.Get(context));
        }
    }
}
