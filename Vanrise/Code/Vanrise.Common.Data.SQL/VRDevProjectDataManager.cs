using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRDevProjectDataManager : BaseSQLDataManager, IVRDevProjectDataManager
    {
        #region Ctor/Properties

        public VRDevProjectDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        public List<VRDevProject> GetDevProjects()
        {
            return GetItemsSP("[common].[sp_VRDevProject_GetAll]", VRDevProjectMapper);
        }

        public bool AreVRDevProjectUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("common.VRDevProject", ref updateHandle);
        }

        public void UpdateProjectAssemblyId(Guid devProjectId, Guid assemblyId, bool updateProjectOnlyIfExistingAssemblyNotChanged, Guid? existingAssemblyId)
        {
            StringBuilder queryBuilder = new StringBuilder(
                @"UPDATE [common].[VRDevProject]
                 SET [AssemblyID] = @AssemblyID, LastModifiedTime = GETDATE()
                 WHERE [ID] = @DevProjectID #ADDITIONALFILTER#");

            bool addExistingAssemblyIdParameter = false;
            if (updateProjectOnlyIfExistingAssemblyNotChanged)
            {
                if (existingAssemblyId.HasValue)
                {
                    queryBuilder.Replace("#ADDITIONALFILTER#", $" AND [AssemblyID] = @ExistingAssemblyID");
                    addExistingAssemblyIdParameter = true;
                }
                else
                {
                    queryBuilder.Replace("#ADDITIONALFILTER#", " AND [AssemblyID] IS NULL");
                }
            }
            else
            {
                queryBuilder.Replace("#ADDITIONALFILTER#", "");
            }

            ExecuteNonQueryText(queryBuilder.ToString(),
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@DevProjectID", devProjectId));
                    cmd.Parameters.Add(new SqlParameter("@AssemblyID", assemblyId));
                    if (addExistingAssemblyIdParameter)
                        cmd.Parameters.Add(new SqlParameter("@ExistingAssemblyID", existingAssemblyId.Value));
                });
        }

        VRDevProject VRDevProjectMapper(IDataReader reader)
        {
            var projectDependencies = reader["ProjectDependencies"] as string;

            return new VRDevProject
            {
                VRDevProjectID = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                AssemblyId = GetReaderValue<Guid?>(reader, "AssemblyID"),
                AssemblyName = reader["AssemblyName"] as string,
                AssemblyCompiledTime = GetReaderValue<DateTime?>(reader, "AssemblyCompiledTime"),
                ProjectDependencies = !string.IsNullOrEmpty(projectDependencies) ? Vanrise.Common.Serializer.Deserialize<List<VRDevProjectDependency>>(projectDependencies) : null,
                CreatedTime = GetReaderValue<DateTime?>(reader, "CreatedTime"),
                LastModifiedTime = GetReaderValue<DateTime?>(reader, "LastModifiedTime")
            };
        }
    }
}
