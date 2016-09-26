using System;
using System.Collections.Generic;
using System.IO;
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

        public BaseBulkInsertInfo CreateOutputCopy()
        {
            if (!this.Stream.IsClosed)
                throw new Exception("Stream should be closed before calling CopyOutput");

            string copiedFilePath = Path.Combine(Path.GetDirectoryName(this.Stream.FilePath), Guid.NewGuid().ToString());
            File.Copy(this.Stream.FilePath, copiedFilePath);
            return new BulkInsertInfo
            {
                DataFilePath = copiedFilePath,
                ColumnNames = this.ColumnNames,
                FieldSeparator = this.FieldSeparator,
                KeepIdentity = this.KeepIdentity,
                TableName = this.TableName,
                TabLock = this.TabLock
            };
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
