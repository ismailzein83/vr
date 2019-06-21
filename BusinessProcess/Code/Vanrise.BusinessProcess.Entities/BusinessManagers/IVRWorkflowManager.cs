﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public interface IVRWorkflowManager : IBusinessManager
    {
        VRWorkflow GetVRWorkflow(Guid vrWorkflowId);
    }

    public interface IBPTaskTypeManager : IBusinessManager
    {
        BPTaskType GetBPTaskType(Guid taskTypeId);
    }
}
