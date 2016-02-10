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
        DataRecordStorage _dataRecordStorage;

        public UpdateRecordStorageContext(DataRecordStorage dataRecordStorage)
        {
            _dataRecordStorage = dataRecordStorage;
        }

        public DataRecordStorage RecordStorage
        {
            get { return _dataRecordStorage; }
        }
        public Object RecordStorageState { get; set; }
    }
}
