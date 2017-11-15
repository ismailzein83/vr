using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Data;
using Vanrise.BusinessProcess;
namespace TOne.WhS.Sales.BP.Activities
{
   public class CleanTemporaryTables : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
       {
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            RatePlanManager ratePlanManager =  new RatePlanManager();
            ratePlanManager.CleanTemporaryTables(processInstanceId);
        }
    }
}
