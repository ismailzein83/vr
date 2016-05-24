using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class DBSyncTaskActionArgument : Vanrise.Runtime.Entities.BaseTaskActionArgument
    {
        public string ConnectionString { get; set; }
        public bool UseTempTables { get; set; }
        public int DefaultSellingNumberPlanId { get; set; }
        public List<DBTableName> MigrationRequestedTables { get; set; }
    }
}
