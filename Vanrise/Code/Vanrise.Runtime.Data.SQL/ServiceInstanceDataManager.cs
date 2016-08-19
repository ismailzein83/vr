using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class ServiceInstanceDataManager : BaseSQLDataManager, IServiceInstanceDataManager
    {
        public ServiceInstanceDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {
        }

        public void AddService(Entities.ServiceInstance serviceInstance)
        {
            ExecuteNonQuerySP("runtime.sp_ServiceInstance_Insert", serviceInstance.ServiceInstanceId, serviceInstance.ServiceType, serviceInstance.ProcessId, serviceInstance.InstanceInfo != null ? Common.Serializer.Serialize(serviceInstance.InstanceInfo) : null);
        }

        public bool AreServiceInstancesUpdated(Guid serviceType, ref object updateHandle)
        {
            return IsDataUpdated<Guid>("[runtime].[ServiceInstance]", "ServiceType", serviceType, ref updateHandle);
        }

        public List<ServiceInstance> GetServices(Guid serviceType)
        {
            return GetItemsSP("runtime.sp_ServiceInstance_GetByType", ServiceInstanceMapper, serviceType);
        }

        public void Delete(Guid serviceInstanceId)
        {
            ExecuteNonQuerySP("runtime.sp_ServiceInstance_Delete", serviceInstanceId);
        }

        #region Mappers

        private ServiceInstance ServiceInstanceMapper(IDataReader reader)
        {
            ServiceInstance instance = new ServiceInstance
            {
                ServiceInstanceId = (Guid)reader["ServiceInstanceID"],
                ServiceType = (Guid)reader["ServiceType"],
                ProcessId = (int)reader["ProcessID"]
            };
            string serializedInfo = reader["ServiceInstanceInfo"] as string;
            if(serializedInfo != null)
                instance.InstanceInfo = Common.Serializer.Deserialize(serializedInfo) as ServiceInstanceInfo;
            return instance;
        }

        #endregion
    }
}
