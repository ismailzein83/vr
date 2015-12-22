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
        public override SchedulerTaskExecuteOutput Execute(SchedulerTask task, BaseTaskActionArgument taskActionArgument, Dictionary<string, object> evaluatedExpressions)
        {
            SupplierSyncTaskActionArgument supplierSyncTaskActionArgument = taskActionArgument as SupplierSyncTaskActionArgument;
            SourceSupplierSynchronizer sourceSupplierSynchronizer = new SourceSupplierSynchronizer(supplierSyncTaskActionArgument.SourceSupplierReader);
            sourceSupplierSynchronizer.Synchronize();
            Console.WriteLine("SupplierSyncTaskAction Executed");
            //throw new NotImplementedException();

            SchedulerTaskExecuteOutput output = new SchedulerTaskExecuteOutput()
            {
                Result = ExecuteOutputResult.Completed
            };
            return output;
        }
    }
}
