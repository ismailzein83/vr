using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRMailMessageTypeDataManager : IDataManager
    {
        List<VRMailMessageType> GetMailMessageTypes();

        bool AreMailMessageTypeUpdated(ref object updateHandle);

        bool Insert(VRMailMessageType vrMailMessageTypeItem);

        bool Update(VRMailMessageType vrMailMessageTypeItem);
    }
}
