using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRNamespaceItemDataManager : IDataManager
    {
        List<VRNamespaceItem> GetVRNamespaceItems();

        bool AreVRNamespaceItemUpdated(ref object updateHandle);

        bool Insert(VRNamespaceItem vrNamespaceItem);

        bool Update(VRNamespaceItem vrNamespaceItem);
    }
}
