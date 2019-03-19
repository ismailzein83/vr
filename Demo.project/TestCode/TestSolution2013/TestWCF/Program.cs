using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TestWCF
{
    class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            //TestFileWCF();
            
            string text = Console.ReadLine();

            var actionWrapper = new ActionWrapper
            {
                Action = () =>
                {
                    Console.WriteLine(text);
                }
            };

            string serializedAction = Vanrise.Common.Serializer.Serialize(actionWrapper);

            DeserializeAndExecute(serializedAction);

            Console.ReadKey();
        }

        private class ActionWrapper
        {
            public Action Action { get; set; }
        }

        private static void DeserializeAndExecute(string serializedAction)
        {
            ActionWrapper actionWrapper = Vanrise.Common.Serializer.Deserialize(serializedAction) as ActionWrapper;
            actionWrapper.Action();
        }

        private static void TestFileWCF()
        {
            Timer timer = new Timer(1000);
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
            if (Console.ReadKey().KeyChar == 's')
            {
                string serviceURL;
                Vanrise.Common.ServiceHostManager.Current.CreateAndOpenTCPServiceHost(typeof(FileWCFService), typeof(IFileWCFService), null, null, out serviceURL);
                Console.WriteLine(serviceURL);
            }
            else
            {
                TestWCFClient form = new TestWCFClient();
                form.ShowDialog();
            }
        }

        static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            
            GC.Collect();
            Console.WriteLine("Memory Cleaned");
        }
    }
}
