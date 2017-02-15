using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataStorages.DataRecordStorage
{
    public class VRRestAPIDataRecordStorageSettings : DataRecordStorageSettings
    {
        public Guid RemoteDataRecordTypeId { get; set; }

        public List<Guid> RemoteDataRecordStorageIds { get; set; }

        public VRRestAPIRecordQueryInterceptor VRRestAPIRecordQueryInterceptor { get; set; }
    }
}
