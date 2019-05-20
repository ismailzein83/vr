﻿using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BaseVRWorkflow
    {
        public string Name { get; set; }

        public string Title { get; set; }
        public Guid? DevProjectId { get; set; }
        public VRWorkflowSettings Settings { get; set; }
    }

    public class VRWorkflow : BaseVRWorkflow
    {
        public Guid VRWorkflowId { get; set; }

        public DateTime CreatedTime { get; set; }

        public int CreatedBy { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public int LastModifiedBy { get; set; }
    }

    public class VRWorkflowSettings
    {
        public VRWorkflowArgumentCollection Arguments { get; set; }

        public VRWorkflowClassMembers ClassMembers { get; set; }

        public VRWorkflowActivity RootActivity { get; set; }
    }

    public class VRWorkflowToAdd : BaseVRWorkflow
    {

    }

    public class VRWorkflowToUpdate : BaseVRWorkflow
    {
        public Guid VRWorkflowId { get; set; }
    }

    public class VRWorkflowEditorRuntime
    {
        public VRWorkflow Entity { get; set; }

        public Dictionary<Guid, VRWorkflowArgumentEditorRuntime> VRWorkflowArgumentEditorRuntimeDict { get; set; }
    }

    public class VRWorkflowArgumentEditorRuntime
    {
        public string VRWorkflowVariableTypeDescription { get; set; }
    }

    public class VRWorkflowClassMembers
    {
        public string ClassMembersCode { get; set; }
    }

    //public enum VRWorkflowTimeUnit
    //{
    //    [VRWorkflowTimeUnit(1)]
    //    Seconds = 0,
    //    [VRWorkflowTimeUnit(60)]
    //    Minutes = 1,
    //    [VRWorkflowTimeUnit(3600)]
    //    Hours = 2
    //}

    //public class VRWorkflowTimeUnitAttribute : Attribute
    //{
    //    public int TimeInSeconds { get; set; }
    //    public VRWorkflowTimeUnitAttribute(int timeInSeconds)
    //    {
    //        TimeInSeconds = timeInSeconds;
    //    }
    //}
}