using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.Data.SQL
{
    public abstract class BaseBulkInsertInfo
    {
        public string TableName { get; set; }

        public bool TabLock { get; set; }

        public bool KeepIdentity { get; set; }

        public char FieldSeparator { get; set; }

        public abstract string GetDataFilePath();

        public IEnumerable<string> ColumnNames { get; set; }
    }

    public class StreamBulkInsertInfo : BaseBulkInsertInfo
    {
        public StreamForBulkInsert Stream { get; set; }

        public override string GetDataFilePath()
        {
            return Stream.FilePath;
        }
    }

    public class BulkInsertInfo : BaseBulkInsertInfo
    {
        public string DataFilePath { get; set; }

        public override string GetDataFilePath()
        {
            return DataFilePath;
        }
    }

}
