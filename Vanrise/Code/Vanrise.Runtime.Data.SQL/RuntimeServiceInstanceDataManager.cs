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
    public class RuntimeServiceInstanceDataManager : BaseSQLDataManager, IRuntimeServiceInstanceDataManager
    {
        public RuntimeServiceInstanceDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {
        }

        public void AddService(Entities.RuntimeServiceInstance serviceInstance)
        {
            ExecuteNonQuerySP("runtime.sp_RuntimeServiceInstance_Insert", serviceInstance.ServiceInstanceId, serviceInstance.ServiceTypeId, serviceInstance.ProcessId, serviceInstance.InstanceInfo != null ? Common.Serializer.Serialize(serviceInstance.InstanceInfo) : null);
        }

        public bool AreServiceInstancesUpdated(ref object updateHandle)
        {
            return IsDataUpdated("[runtime].[RuntimeServiceInstance]", ref updateHandle);
        }

        public List<RuntimeServiceInstance> GetServices()
        {
            return GetItemsSP("runtime.sp_RuntimeServiceInstance_GetAll", ServiceInstanceMapper);
        }

        public void DeleteByProcessId(int runtimeProcessId)
        {
            ExecuteNonQuerySP("[runtime].[sp_RuntimeServiceInstance_DeleteByProcess]", runtimeProcessId);
        }

        #region Mappers

        private RuntimeServiceInstance ServiceInstanceMapper(IDataReader reader)
        {
            RuntimeServiceInstance instance = new RuntimeServiceInstance
            {
                ServiceInstanceId = (Guid)reader["ID"],
                ServiceTypeId = (int)reader["ServiceTypeID"],
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
