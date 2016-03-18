using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.GenericData.SQLDataStorage
{
    public interface IDynamicManager
    {
        string[] ColumnNames { get; }

        string ColumnNamesCommaDelimited { get; }

        void WriteRecordToStream(dynamic record, StreamForBulkInsert streamForBulkInsert);

        void FillDataRecordFromReader(dynamic dataRecord, IDataReader reader);

        DataTable ConvertDataRecordsToTable(IEnumerable<dynamic> dataRecords);
    }
}
