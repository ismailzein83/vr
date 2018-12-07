using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.RDBDataStorage
{
    public interface IDynamicManager
    {
        void WriteFieldsToRecordStream(dynamic record, Vanrise.Data.RDB.RDBBulkInsertQueryWriteRecordContext bulkInsertRecordContext);

        void SetRDBInsertColumnsFromRecord(dynamic record, Vanrise.Data.RDB.RDBInsertQuery insertQuery);

        dynamic GetDynamicRecordFromReader(Vanrise.Data.RDB.IRDBDataReader reader);

        Vanrise.GenericData.Entities.DataRecord GetDataRecordFromReader(Vanrise.Data.RDB.IRDBDataReader reader, List<string> fieldNames);
    }
}
