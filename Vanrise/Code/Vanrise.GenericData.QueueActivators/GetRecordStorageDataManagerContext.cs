using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.QueueActivators
{
    public class GetRecordStorageDataManagerContext : IGetRecordStorageDataManagerContext
    {
        public DataStore DataStore
        {
            get;
            set;
        }

        public DataRecordStorage DataRecordStorage
        {
            get;
            set;
        }
    }
}
