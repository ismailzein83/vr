using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IGetRemoteRecordStorageDataManagerContext
    {
        DataStore DataStore { get; }

        DataRecordStorage DataRecordStorage { get; }
    }
}
