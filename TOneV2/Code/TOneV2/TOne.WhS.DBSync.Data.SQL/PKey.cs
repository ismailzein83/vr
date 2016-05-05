using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class PKey
    {
        public string KeyName { get; set; }
        public List<string> Fields { get; set; }
    }
}
