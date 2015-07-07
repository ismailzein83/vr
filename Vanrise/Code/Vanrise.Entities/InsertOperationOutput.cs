﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public enum InsertOperationResult { Succeeded = 0, Failed = 1, SameExists = 2 }
    public class InsertOperationOutput<T>
    {
        public InsertOperationResult Result { get; set; }

        public T InsertedObject { get; set; }
    }

    public class TempTableName
    {
        public string Key { get; set; }

        public string TableName { get; set; }
    }



    public class BigResult<T>
    {
        public string ResultKey { get; set; }

        public IEnumerable<T> Data { get; set; }

        public int TotalCount { get; set; }
    }
}
