using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.SQLDataStorage
{
    public class SQLTempStorageInformation : TempStorageInformation
    {
        public string Schema { get; set; }

        public string TableName { get; set; }

        public string TableNameWithSchema
        {
            get
            {
                return Schema != null ? string.Format("{0}.{1}", Schema, TableName) : string.Format("dbo.{0}", TableName);
            }
        }
    }
}
