using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.GenericData.SQLDataStorage
{
    internal class DynamicTypeGenerator
    {
        public IBulkInsertWriter GetBulkInsertWriter(int dataRecordType, SQLDataRecordStorageSettings dataRecordStorageSettings)
        {
            return null;
        }
    }

    public interface IBulkInsertWriter
    {
        void WriteRecordToStream(dynamic record, StreamForBulkInsert streamForBulkInsert);
    }
}
