using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace QM.BusinessEntity.Business
{
    public class SupplierSyncTaskAction : SchedulerTaskAction
    {
        public override void Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            Console.WriteLine("SupplierSyncTaskAction Executed");
            //throw new NotImplementedException();
        }
    }
}
