using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SwitchDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "Name", "SourceID" };
        bool _UseTempTables;
        public SwitchDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_MigrationDBConnStringKey", "TOneV2MigrationDBConnString"))
        {
            _UseTempTables = useTempTables;
        }

        public void ApplySwitchesToTemp(List<Switch> switches)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var s in switches)
                {
                    wr.WriteLine(String.Format("{0}^{1}", s.Name, s.SwitchId));
                }
                wr.Close();
            }

            Object preparedSwitches = new BulkInsertInfo
            {
                TableName = "[TOneWhS_BE].[Switch" + (_UseTempTables ? Constants._Temp : "") + "]",
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedSwitches as BaseBulkInsertInfo);
        }
    }
}
