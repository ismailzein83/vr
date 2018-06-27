using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class VRWorkflowDataManager : BaseSQLDataManager, IVRWorkflowDataManager
    {
        public VRWorkflowDataManager()
            : base(GetConnectionStringName("BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }

        #region public methods
        public List<VRWorkflow> GetVRWorkflows()
        {
            return GetItemsSP("bp.sp_VRWorkflow_GetAll", VRWorkflowMapper);
        }

        public bool InsertVRWorkflow(VRWorkflowToAdd vrWorkflow, Guid vrWorkflowId, int createdBy)
        {
            string serializedSettings = vrWorkflow.Settings != null ? Vanrise.Common.Serializer.Serialize(vrWorkflow.Settings) : null;
            return ExecuteNonQuerySP("[bp].[sp_VRWorkflow_Insert]", vrWorkflowId, vrWorkflow.Name, vrWorkflow.Title, serializedSettings, createdBy) > 0;
        }

        public bool UpdateVRWorkflow(VRWorkflowToUpdate vrWorkflow, int lastModifiedBy)
        {
            string serializedSettings = vrWorkflow.Settings != null ? Vanrise.Common.Serializer.Serialize(vrWorkflow.Settings) : null;
            return ExecuteNonQuerySP("[bp].[sp_VRWorkflow_Update]", vrWorkflow.VRWorkflowId, vrWorkflow.Name, vrWorkflow.Title, serializedSettings, lastModifiedBy) > 0;
        }

        public bool AreVRWorkflowsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[bp].[VRWorkflow]", ref updateHandle);
        }
        #endregion

        #region Mappers

        VRWorkflow VRWorkflowMapper(IDataReader reader)
        {
            var vrWorkflow = new VRWorkflow
            {
                VRWorkflowId = GetReaderValue<Guid>(reader, "ID"),
                Name = reader["Name"] as string,
                Title = reader["Title"] as string,
                CreatedBy = GetReaderValue<int>(reader, "CreatedBy"),
                CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime"),
                LastModifiedBy = GetReaderValue<int>(reader, "LastModifiedBy"),
                LastModifiedTime = GetReaderValue<DateTime>(reader, "LastModifiedTime")
            };
            string settings = reader["Settings"] as string;
            if (!String.IsNullOrEmpty(settings))
                vrWorkflow.Settings = Serializer.Deserialize<VRWorkflowSettings>(settings);

            return vrWorkflow;
        }
        #endregion
    }
}
