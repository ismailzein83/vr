using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL.Common;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SwitchDBSyncDataManager : BaseSQLDataManager
    {
        readonly string[] columns = { "Name", "SourceID" };
        string _TableName = Vanrise.Common.Utilities.GetEnumDescription(DBTableName.Switch);
        string _Schema = "TOneWhS_BE";
        bool _UseTempTables;
        public SwitchDBSyncDataManager(bool useTempTables) :
            base(GetConnectionStringName("TOneWhS_BE_MigrationDBConnStringKey", "TOneV2DBConnString"))
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
                TableName = MigrationUtils.GetTableName(_Schema, _TableName, _UseTempTables),
                DataFilePath = filePath,
                ColumnNames = columns,
                TabLock = true,
                KeepIdentity = true,
                FieldSeparator = '^',
            };

            InsertBulkToTable(preparedSwitches as BaseBulkInsertInfo);
        }
        public string GetConnection()
        {
            return base.GetConnectionString();
        }

        public string GetTableName()
        {
            return _TableName;
        }

        public string GetSchema()
        {
            return _Schema;
        }

    }
}
