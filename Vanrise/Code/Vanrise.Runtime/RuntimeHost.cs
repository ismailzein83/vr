using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Vanrise.Runtime
{
    public class RuntimeHost
    {
        public RuntimeHost(List<RuntimeService> services)
        {
            _services = services; 
            _timer = new Timer(1000);
            _timer.Elapsed += timer_Elapsed;
        }

        List<RuntimeService> _services;
        Timer _timer;
        public void Start()
        {            
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }


        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var service in _services)
                service.ExecuteIfIdleAndDue();
        }
    }
}
