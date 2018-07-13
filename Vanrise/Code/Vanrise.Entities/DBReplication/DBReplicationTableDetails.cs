﻿using System;

namespace Vanrise.Entities
{
    public class DBReplicationTableDetails
    {
        public string TableName { get; set; }

        public string TableSchema { get; set; }

        public string SourceConnectionStringName { get; set; }

        public string TargetLinkedServerName { get; set; }

        public string TargetDatabaseName { get; set; }

        public string FilterDateTimeColumn { get; set; }

        public DBReplicationPreInsert DBReplicationPreInsert { get; set; }
    }
}
