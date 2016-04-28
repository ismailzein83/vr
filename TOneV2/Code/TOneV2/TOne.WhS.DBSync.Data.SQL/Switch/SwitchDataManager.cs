using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SwitchDataManager : BaseSQLDataManager, ISwitchDataManager
    {
        public SwitchDataManager() :
            base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneV2DBConnString"))
        {

        }
        
        public void ApplySwitchesToDB(List<Switch> switches)
        {

            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var s in switches)
                {
                    wr.WriteLine(String.Format("0^{1}", s.Name));
                }
                wr.Close();
            }

            Object preparedSwitches = new BulkInsertInfo
            {
                TableName = "TOneWhS_BE.Switch",
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = '^'
            };

            InsertBulkToTable(preparedSwitches as BaseBulkInsertInfo);
        }

    }
}
