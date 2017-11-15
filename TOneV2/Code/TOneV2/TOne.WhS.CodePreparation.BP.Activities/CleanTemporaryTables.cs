using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Data;
using Vanrise.BusinessProcess;
namespace TOne.WhS.CodePreparation.BP.Activities
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
