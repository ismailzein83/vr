﻿using System;
using System.Collections.Generic;

namespace Vanrise.BusinessProcess.Entities
{
	public class VRWorkflowActivity
	{
		public Guid VRWorkflowActivityId { get; set; }

		public VRWorkflowActivitySettings Settings { get; set; }
	}

	public class VRWorkflowActivityCollection : List<VRWorkflowActivity>
	{
	}

	public abstract class VRWorkflowActivitySettings
	{
		public abstract Guid ConfigId { get; }

		public abstract string Editor { get; }

		public abstract string Title { get; }

		public abstract string GenerateWFActivityCode(IVRWorkflowActivityGenerateWFActivityCodeContext context);
	}

	public interface IVRWorkflowActivityGenerateWFActivityCodeContext
	{
		void AddVariables(VRWorkflowVariableCollection variables);

		void AddVariable(VRWorkflowVariable variable);

		VRWorkflowVariable GetVariableWithValidate(string variableName);

		IEnumerable<VRWorkflowVariable> GetAllVariables();

		IEnumerable<VRWorkflowArgument> GetAllWorkflowArguments();

		string GenerateUniqueNamespace(string nmSpace);

		void AddFullNamespaceCode(string namespaceCode);

		void AddUsingStatement(string usingStatement);

		IVRWorkflowActivityGenerateWFActivityCodeContext CreateChildContext();
	}
}
