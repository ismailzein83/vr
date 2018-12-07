using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.RDBDataStorage
{
    public class RDBTempStorageInformation : TempStorageInformation
    {
        public string Schema { get; set; }

        public string TableName { get; set; }
    }
}
