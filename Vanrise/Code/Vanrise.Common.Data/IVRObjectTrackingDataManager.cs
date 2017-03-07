using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data
{
    public interface IVRObjectTrackingDataManager : IDataManager
    {
        long Insert(int userId, int loggableEntityId, string objectId, object obj, int actionId, string actionDescription);
    }
}
