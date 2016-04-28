using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class Table
    {
        public List<TableKey> foreignKeys { get; set; }

        public string Schema { get; set; }

        public string NamewithoutSchema { get; set; }

        public string CreateTempTableQuery { get; set; }

        public string Name
        {
            get { return Schema + "." + NamewithoutSchema; }
        }

        public string TempName
        {
            get { return Name + "Temp"; }
        }
    }
}
