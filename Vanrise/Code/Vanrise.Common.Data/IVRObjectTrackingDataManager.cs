using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data
{
    public interface IVRObjectTrackingDataManager : IDataManager
    {
        long Insert(int userId, Guid loggableEntityId, string objectId, object obj, int actionId, string actionDescription);
    }
}
