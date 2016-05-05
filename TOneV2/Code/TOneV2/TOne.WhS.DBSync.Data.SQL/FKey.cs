using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class FKey
    {
        public string TableName { get; set; }
        public string KeyName { get; set; }
    }
}
