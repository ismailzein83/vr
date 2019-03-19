using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading;

namespace WorkflowConsoleApplication1
{

    public sealed class InvokeActivity : CodeActivity
    {
        
        protected override void Execute(CodeActivityContext context)
        {
            ChildWF childWF = new ChildWF();
            WorkflowApplication wfApp = new WorkflowApplication(childWF);
            wfApp.Extensions.Add(context.GetExtension<ConsoleTracking>());
            wfApp.Run();
            
            bool isChildCompleted = false;
            wfApp.Completed = (e) =>
            {
                isChildCompleted = true;
            };
            while(!isChildCompleted)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
