using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataStorages
{
    public class GetRemoteRecordStorageDataManagerContext : IGetRemoteRecordStorageDataManagerContext
    {
        public Entities.DataStore DataStore { get; set; }

        public Entities.DataRecordStorage DataRecordStorage { get; set; }
    }
}
