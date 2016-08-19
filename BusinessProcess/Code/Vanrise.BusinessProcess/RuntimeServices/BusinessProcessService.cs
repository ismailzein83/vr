using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime;
using Vanrise.Runtime.Entities;

namespace Vanrise.BusinessProcess
{
    public class BusinessProcessService : RuntimeService
    {
        internal static Guid s_bpServiceInstanceType = new Guid("658F4B0D-B701-4EBF-A07A-5CE51B3F9DDF");
        protected override void OnStarted()
        {
            RegisterServiceInstance();
            base.OnStarted();
        }

        ServiceInstance _serviceInstance;
        private void RegisterServiceInstance()
        {
            _serviceInstance = new ServiceInstanceManager().RegisterServiceInstance(s_bpServiceInstanceType, new Runtime.Entities.ServiceInstanceInfo());
        }

        protected override void Execute()
        {
            int definitionId;
            while(PendingItemsHandler.Current.TryGetPendingDefinitionsToProcess(_serviceInstance.ServiceInstanceId, out definitionId))
            {
                BusinessProcessRuntime.Current.ExecutePendings(definitionId, _serviceInstance.ServiceInstanceId);
            }
            BusinessProcessRuntime.Current.TriggerPendingEventsIfIdleAsync();
        }
    }
}
