using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;

namespace TOne.LCRProcess.Activities
{

    public class GetCodeCustomersInput
    {
        public List<RouteRule> RouteRules { get; set; }

    }

    public class GetCodeCustomersOutput
    {
        public CodeCustomers CodeCustomers { get; set; }
    }

    public sealed class GetCodeCustomers : BaseAsyncActivity<GetCodeCustomersInput, GetCodeCustomersOutput>
    {
        [RequiredArgument]
        public InArgument<List<RouteRule>> RouteRules { get; set; }

        [RequiredArgument]
        public OutArgument<CodeCustomers> CodeCustomers { get; set; }

        protected override GetCodeCustomersOutput DoWorkWithResult(GetCodeCustomersInput inputArgument, AsyncActivityHandle handle)
        {
            CodeCustomers codeCustomers = new CodeCustomers();

            foreach (RouteRule rule in inputArgument.RouteRules)
            {
                CodeSelectionSet set = rule.CodeSet as CodeSelectionSet;

            }

            return new GetCodeCustomersOutput()
            {
                CodeCustomers = codeCustomers
            };
        }

        protected override GetCodeCustomersInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetCodeCustomersInput()
            {
                RouteRules = this.RouteRules.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetCodeCustomersOutput result)
        {
            this.CodeCustomers.Set(context, result.CodeCustomers);
        }
    }
}
