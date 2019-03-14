﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.Entities
{
    public class VRWorkflowTaskAssigneesSettingConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "BP_VR_Workflow_TaskAssignees_Setting";

        public string Editor { get; set; }
    }
}
