using Microsoft.CSharp.Activities;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Linq;

namespace TestRuntime.Tasks
{
    public class HaririTask : ITask
    {

        public void Execute()
        {
            CreateWFProgrammatically();

            Console.WriteLine("DONE");
            Console.ReadKey();
        }

        private void CreateWFProgrammatically()
        {
            var wf = new WF();
            Compile(wf);
            WorkflowInvoker.Invoke(wf);
            Console.WriteLine("ProvValue: {0}", wf.propValue);
            WorkflowInvoker.Invoke(wf);
            Console.WriteLine("ProvValue: {0}", wf.propValue);
            WorkflowInvoker.Invoke(wf);
            WorkflowInvoker.Invoke(wf);
            Console.ReadKey();
        }

        static void Compile(Activity dynamicActivity)
        {
            TextExpressionCompilerSettings settings = new TextExpressionCompilerSettings
            {
                Activity = dynamicActivity,
                Language = "C#",
                ActivityName = dynamicActivity.GetType().FullName.Split('.').Last() + "_CompiledExpressionRoot",
                ActivityNamespace = string.Join(".", dynamicActivity.GetType().FullName.Split('.').Reverse().Skip(1).Reverse()),
                RootNamespace = null,
                GenerateAsPartialClass = false,
                AlwaysGenerateSource = true,
            };

            TextExpressionCompilerResults results = new TextExpressionCompiler(settings).Compile();
            if (results.HasErrors)
            {
                throw new Exception("Compilation failed.");
            }

            ICompiledExpressionRoot compiledExpressionRoot = Activator.CreateInstance(results.ResultType, new object[] { dynamicActivity }) as ICompiledExpressionRoot;
            CompiledExpressionInvoker.SetCompiledExpressionRootForImplementation(dynamicActivity, compiledExpressionRoot);
        }
    }


    public class WF : Activity
    {
        public InArgument<string> InputText { get; set; }
        public string propValue;
        public WF()
        {
            var sequence = new Sequence();
            var variable1 = new Variable<string>("Variable1");
            sequence.Variables.Add(variable1);
            this.Implementation = () => new Sequence
            {
                Variables = 
                 {
                      new Variable<string>{  Name = "Variable1", Default = "Var 1"}, 
                      new Variable<string>{  Name = "Variable2"}, 
                      new Variable<List<string>>{  Name = "List1"}
                 },
                Activities = 
                 {
                     new Vanrise.BusinessProcess.Business.VRWorkflowManager().GetVRWorkflowActivity(Guid.NewGuid()),
                     new Vanrise.BusinessProcess.Business.VRWorkflowManager().GetVRWorkflowActivity(new Guid("9f474e13-4285-4049-baf3-e0a8793898a4")),
                     //new WriteLine { Text = "From WriteLine"},
                     new WriteLine { Text = new InArgument<string>(new CSharpValue<string>("Variable1"))},
                     new WriteLine { Text = new CSharpValue<string>("DateTime.Now.ToString()")},
                     new ReadFromConsole { Output = new CSharpReference<string>("Variable2")},
                     new WriteLine { Text = new CSharpValue<string>("String.Format(\" this text {0} is written at {1}\", Variable2, DateTime.Now)")},
                     new Assign { To = new OutArgument<string>(new CSharpReference<string>("Variable1")), Value = new InArgument<string>(new CSharpValue<string>("\"new value for variable 1 \" + DateTime.Now.ToString()"))  },
                     new WriteLine { Text = new InArgument<string>((ctx) => new PropertySetter(ctx).Property )},// new CSharpValue<string>("Variable1")},
                     new Assign { To = new OutArgument<List<string>>(new CSharpReference<List<string>>("List1")), Value = new InArgument<List<string>>(new CSharpValue<List<string>>("new List<string> { \"item1\", \"item2\", \"item3\" }"))  },
                     new Assign { To = new OutArgument<string>(new CSharpReference<string>("List1[1]")), Value = new InArgument<string>(new CSharpValue<string>("\" List Item2 value changed \" + DateTime.Now.ToString()"))  },
                     new WriteLine { Text = new CSharpValue<string>("List1[1]")},
                     new Assign { To = new OutArgument<string>((ctx) => new PropertySetter(ctx).Property), Value = new InArgument<string>(new CSharpValue<string>("List1[1] + \" value appended \" + DateTime.Now.ToString()"))  },                     
                     new Assign { To = new OutArgument<string>((ctx) => propValue), Value = new InArgument<string>(new CSharpValue<string>("List1[1] + \" value appended \" + DateTime.Now.ToString()"))  },
                     new WriteLine { Text = new CSharpValue<string>("List1[1]")},
                     new MyCustomActivity { InOutArg = new InOutArgument<string>((ctx) => new PropertySetter(ctx).Property)}
                 }
            };

        }
        public class MyCustomActivity : CodeActivity
        {
            public InOutArgument<string> InOutArg { get; set; }
            protected override void Execute(CodeActivityContext context)
            {
                Console.WriteLine("MyCustomActivity: {0}", this.InOutArg.Get(context));
                this.InOutArg.Set(context, "Value Changed");
            }
        }
    }

    public class PropertySetter
    {
        public PropertySetter(ActivityContext ctx)
        {

        }
        public string Property
        {
            get
            {

                return "In Value";
            }
            set
            {
                Console.WriteLine("Value is set here: {0}", value);
            }
        }
    }

    public class ReadFromConsole : CodeActivity
    {
        public OutArgument<string> Output { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            string line = Console.ReadLine();
            this.Output.Set(context, line);
        }
    }
}