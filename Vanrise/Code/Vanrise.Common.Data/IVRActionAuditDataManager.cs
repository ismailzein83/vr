using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data
{
    public interface IVRActionAuditDataManager : IDataManager
    {
        void Insert(int? userId, int urlId, int moduleId, int entityId, int actionId, string objectId, string objectName, string actionDescription);
    }
}
