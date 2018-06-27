using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data
{
    public interface IVRWorkflowDataManager : IDataManager
    {
        List<VRWorkflow> GetVRWorkflows();

        bool AreVRWorkflowsUpdated(ref object updateHandle);

        bool InsertVRWorkflow(VRWorkflowToAdd vrWorkflow, Guid vrWorkflowId, int createdBy);

        bool UpdateVRWorkflow(VRWorkflowToUpdate vrWorkflow, int lastModifiedBy);
    }
}
