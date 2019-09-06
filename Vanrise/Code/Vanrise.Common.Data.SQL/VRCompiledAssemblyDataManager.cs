using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRCompiledAssemblyDataManager : BaseSQLDataManager, IVRCompiledAssemblyDataManager
    {
        #region Ctor/Properties

        public VRCompiledAssemblyDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        public void AddAssembly(Guid assemblyId, string name, Guid devProjectId, byte[] assemblyContent, DateTime compiledTime)
        {
            ExecuteNonQuerySP("[common].[sp_VRCompiledAssembly_Insert]", assemblyId, name, devProjectId, assemblyContent, compiledTime);
        }

        public VRCompiledAssembly GetAssemblyByName(string assemblyName)
        {
            return GetItemSP("[common].[sp_VRCompiledAssembly_GetByName]", VRCompiledAssemblyMapper, assemblyName);
        }

        public List<VRCompiledAssembly> GetAssembliesEffectiveForDevProjects()
        {
            return GetItemsSP("[common].[sp_VRCompiledAssembly_GetEffectiveForDevProjects]", VRCompiledAssemblyMapper);
        }

        VRCompiledAssembly VRCompiledAssemblyMapper(IDataReader reader)
        {
            return new VRCompiledAssembly
            {
                VRCompiledAssemblyId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                DevProjectId = (Guid)reader["DevProjectID"],
                AssemblyContent = (byte[])reader["AssemblyContent"]
            };
        }
    }
}
