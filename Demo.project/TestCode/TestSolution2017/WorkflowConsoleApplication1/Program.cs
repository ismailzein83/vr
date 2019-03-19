using System;
using System.Linq;
using System.Activities;
using System.Activities.Statements;
using Vanrise.BusinessProcess.Entities;
using System.Collections.Generic;

namespace WorkflowConsoleApplication1
{

    class Program
    {
        static void Main(string[] args)
        {
            ServiceReference1.getFXRatesRequest getFXRatesRequest = new ServiceReference1.getFXRatesRequest
            {
                getFXRates = new ServiceReference1.GetFXRatesType
                {
                    body = new ServiceReference1.GetFXRatesTypeBody
                    {
                        fromCurrencyList = new ServiceReference1.ISO4217CurrencyType[]
                        {
                            new ServiceReference1.ISO4217CurrencyType { code = "USD" },
                            new ServiceReference1.ISO4217CurrencyType { code = "EUR" }
                        }
                    }
                }
            };
            Activity workflow1 = new Workflow1();
            WorkflowInvoker.Invoke(workflow1);

            var seq = new System.Activities.Statements.Sequence
            {
                Activities =
                {
                    new Assign<string>{ Value = new InArgument<string>("f"), To = new OutArgument<string>()},
                    new Vanrise.BusinessProcess.WFActivities.AssignTask<BPGenericTaskExecutionInformation>
                    {
                         TaskData = new InArgument<BPTaskData>(new BPGenericTaskData()),
                         ExecutedTask = new OutArgument<BPTask>()
                    }
                }
            };
        }
    }

    public class BPGenericTaskData : BPTaskData
    {
        public Dictionary<string, Object> FieldValues { get; set; }
    }

    public class BPGenericTaskExecutionInformation : BPTaskExecutionInformation
    {

    }
}
