using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRComponentTypeDataManager : IDataManager
    {
        List<VRComponentType> GetComponentTypes();
        bool Insert(VRComponentType componentType);
        bool Update(VRComponentType componentType);
        bool AreVRComponentTypeUpdated(ref object updateHandle);
    }
}
