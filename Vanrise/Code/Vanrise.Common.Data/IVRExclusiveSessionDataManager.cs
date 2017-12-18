using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRExclusiveSessionDataManager : IDataManager
    {
        void InsertIfNotExists(Guid sessionTypeId, string targetId);

        void TryTakeSession(Guid sessionTypeId, string targetId, int userId, int timeoutInSeconds, out int takenByUserId);

        void TryKeepSession(Guid sessionTypeId, string targetId, int userId, int timeoutInSeconds, out int takenByUserId);

        void ReleaseSession(Guid sessionTypeId, string targetId, int userId);

        List<VRExclusiveSession> GetAllVRExclusiveSessions(int timeOutInSeconds, List<Guid> sessionTypeIds);
    }
}
