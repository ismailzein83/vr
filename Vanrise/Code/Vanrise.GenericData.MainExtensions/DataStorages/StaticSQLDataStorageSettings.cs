using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataStorages
{
    public class StaticSQLDataStorageSettings : DataStoreSettings
    {
        public string ConnectionString { get; set; }

        public override void UpdateRecordStorage(IUpdateRecordStorageContext context)
        {
            var sqlRecordStorageSettings = context.RecordStorage.Settings as SQLDataRecordStorageSettings;
            var sqlRecordStorageState = context.RecordStorageState as SQLDataRecordStorageState;
        }
    }
}
