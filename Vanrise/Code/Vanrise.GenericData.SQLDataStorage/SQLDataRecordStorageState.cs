using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.SQLDataStorage
{    
    public class SQLDataRecordStorageState : DataRecordStorageState
    {
        public new SQLDataRecordStorageSettings ExistingSettings { get; set; }
    }
}
