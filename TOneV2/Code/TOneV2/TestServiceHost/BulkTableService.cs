using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TOne.Data;
using TOne.Entities;

namespace TestServiceHost
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BulkTableService" in both code and config file together.
    public class BulkTableService : IBulkTableService
    {
        #region IBulkTableService Members

        public void WriteCodeMatchTable(System.Data.DataTable dt)
        {
            ICodeMatchDataManager dataManager = DataManagerFactory.GetDataManager<ICodeMatchDataManager>();
            dataManager.WriteCodeMatchTableToDB(dt);
        }

        #endregion
    }
}
