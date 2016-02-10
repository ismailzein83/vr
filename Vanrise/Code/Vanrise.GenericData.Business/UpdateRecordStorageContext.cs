using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class UpdateRecordStorageContext : IUpdateRecordStorageContext
    {
        public DataStore DataStore { get; set; }

        public DataRecordStorage RecordStorage { get; set; }

        public DataRecordStorageSettings ExistingRecordSettings { get; set; }

        public DataRecordStorageState RecordStorageState { get; set; }
    }
}
