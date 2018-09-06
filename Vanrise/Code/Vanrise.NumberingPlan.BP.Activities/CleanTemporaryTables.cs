using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.NumberingPlan.Business;

namespace Vanrise.NumberingPlan.BP.Activities
{
	public class CleanTemporaryTables : CodeActivity
	{
		protected override void Execute(CodeActivityContext context)
		{
			long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
			CodePreparationManager codePreparationManager = new CodePreparationManager();
			codePreparationManager.CleanTemporaryTables(processInstanceId);
		}
	}
}
