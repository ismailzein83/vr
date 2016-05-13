using TOne.WhS.DBSync.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Business
{
    public class DBSyncTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public string ConnectionString { get; set; }
        public bool UseTempTables { get; set; }
    }
}
