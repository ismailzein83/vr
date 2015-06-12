using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{

    public sealed class GetRouteCodesFromRules : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> LastRun { get; set; }
        [RequiredArgument]
        public OutArgument<CodeCustomers> CodeCustomers { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime lastRun = DateTime.Now.AddMonths(-1);// context.GetValue(this.LastRun);
            IRouteRulesDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteRulesDataManager>();
            CodeCustomers codeCustomers = dataManager.GetRulesRouteCodes(lastRun);
            this.CodeCustomers.Set(context, codeCustomers);
        }
    }
}
