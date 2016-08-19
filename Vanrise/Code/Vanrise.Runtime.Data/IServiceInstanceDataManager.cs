using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface IServiceInstanceDataManager : IDataManager
    {
        void AddService(ServiceInstance serviceInstance);

        bool AreServiceInstancesUpdated(Guid serviceType, ref object updateHandle);

        List<ServiceInstance> GetServices(Guid serviceType);

        void Delete(Guid serviceInstanceId);
    }
}
