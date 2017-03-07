using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public interface IVRActionLogger : IBEManager
    {
        void LogGetFilteredAction(VRLoggableEntityBase loggableEntity, DataRetrievalInput dataRetrievalInput);

        void LogObjectCustomAction(VRLoggableEntityBase loggableEntity, string action, bool isObjectUpdated, Object obj, string actionDescription);

        void LogObjectViewed(VRLoggableEntityBase loggableEntity, Object obj);

        void TrackAndLogObjectAdded(VRLoggableEntityBase loggableEntity, Object obj);

        void TrackAndLogObjectUpdated(VRLoggableEntityBase loggableEntity, Object obj);

        void TrackAndLogObjectDeleted(VRLoggableEntityBase loggableEntity, Object obj);
    }
}
