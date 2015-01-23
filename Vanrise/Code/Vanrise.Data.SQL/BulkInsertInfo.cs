using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vanrise.Data.SQL
{
    public class BulkInsertInfo
    {
        public string TableName { get; set; }
        public string DataFilePath { get; set; }

        public bool TabLock { get; set; }

        public bool KeepIdentity { get; set; }

        public char FieldSeparator { get; set; }
    }
}
