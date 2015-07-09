using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Business;
using Vanrise.Integration.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Integration.Extensions
{
    public class DSSchedulerTaskAction : SchedulerTaskAction
    {
        public override void Execute(SchedulerTask task, Dictionary<string, object> evaluatedExpressions)
        {
            DataSourceManager dataManager = new DataSourceManager();
            Vanrise.Integration.Entities.DataSource dataSource = dataManager.GetDataSourcebyTaskId(task.TaskId);

            dataSource.Settings.Adapter.ImportData(ReceiveData);
        }

        private void ReceiveData(IImportedData data)
        {

        }
    }
}
