using System;
using System.Linq;
using System.Activities;
using System.Activities.Statements;

namespace WorkflowConsoleApplication1
{

    class Program
    {
        static void Main(string[] args)
        {
            Activity workflow1 = new Workflow1();
            WorkflowApplication wfApp = new WorkflowApplication(workflow1);
            wfApp.Extensions.Add(new ConsoleTracking());
            wfApp.Completed = (e) =>
                {
                    Console.WriteLine("Workflow Completed");
                };
            wfApp.Run();
            Console.ReadKey();
        }
    }
}
