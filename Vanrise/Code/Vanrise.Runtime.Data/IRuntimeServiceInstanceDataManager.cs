using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface IRuntimeServiceInstanceDataManager : IDataManager
    {
        void AddService(RuntimeServiceInstance serviceInstance);

        bool AreServiceInstancesUpdated(ref object updateHandle);

        List<RuntimeServiceInstance> GetServices();

        void DeleteByProcessId(int runtimeProcessId);
    }
}
